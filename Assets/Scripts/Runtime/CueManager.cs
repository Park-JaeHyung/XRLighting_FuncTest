using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueManager : MonoBehaviour
{
    public static CueManager Instance { get; private set; }

    private readonly CueSheet _cueSheet = new();
    private int _nextCueNumber = 1;
    private bool _isPlaying;

    public CueSheet CueSheet => _cueSheet;

    public event Action OnCueSheetChanged;
    public event Action<int> OnCueActivated;

    void Awake() => Instance = this;

    public void RecordCue(float fadeTime = 1f)
    {
        var cue = new Cue
        {
            cueNumber = _nextCueNumber++,
            label = $"Cue {_cueSheet.cues.Count + 1}",
            fadeTime = fadeTime
        };

        foreach (var fixture in FixtureManager.Instance.Fixtures)
        {
            cue.snapshots.Add(new Cue.FixtureSnapshot
            {
                fixtureId = fixture.Id,
                parameters = fixture.GetParameters()
            });
        }

        _cueSheet.cues.Add(cue);
        OnCueSheetChanged?.Invoke();
    }

    public void PlayAll()
    {
        if (_isPlaying) return;
        StartCoroutine(PlaySequence());
    }

    private IEnumerator PlaySequence()
    {
        _isPlaying = true;
        for (int i = 0; i < _cueSheet.cues.Count; i++)
        {
            OnCueActivated?.Invoke(i);
            yield return StartCoroutine(PlayCue(_cueSheet.cues[i]));
            yield return new WaitForSeconds(0.3f);
        }
        _isPlaying = false;
    }

    private IEnumerator PlayCue(Cue cue)
    {
        var fromParams = new Dictionary<int, LightParameters>();
        foreach (var snap in cue.snapshots)
        {
            var f = FixtureManager.Instance.GetById(snap.fixtureId);
            if (f != null) fromParams[snap.fixtureId] = f.GetParameters();
        }

        float elapsed = 0f;
        float duration = Mathf.Max(cue.fadeTime, 0.001f);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            foreach (var snap in cue.snapshots)
            {
                var f = FixtureManager.Instance.GetById(snap.fixtureId);
                if (f == null || !fromParams.ContainsKey(snap.fixtureId)) continue;
                f.ApplyParameters(CueInterpolator.Interpolate(fromParams[snap.fixtureId], snap.parameters, t));
            }
            yield return null;
        }

        foreach (var snap in cue.snapshots)
            FixtureManager.Instance.GetById(snap.fixtureId)?.ApplyParameters(snap.parameters);
    }
}
