using UnityEngine;
using UnityEngine.UIElements;

public class CueSheetEditorUI : MonoBehaviour
{
    private VisualElement _panel;
    private ScrollView _cueList;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _panel = root.Q("cue-sheet-editor");
        _cueList = _panel.Q<ScrollView>("cue-list");

        _panel.Q<Button>("btn-play-all").clicked += () => CueManager.Instance.PlayAll();
        _panel.Q<Button>("btn-back-from-cuesheet").clicked += () => AppController.Instance.ExitCueSheet();

        _panel.RegisterCallback<MouseEnterEvent>(_ => AppController.IsPointerOverUI = true);
        _panel.RegisterCallback<MouseLeaveEvent>(_ => AppController.IsPointerOverUI = false);

        CueManager.Instance.OnCueSheetChanged += RefreshList;
        CueManager.Instance.OnCueActivated += HighlightCue;
        AppController.Instance.OnStateChanged += OnStateChanged;
        Show(false);
    }

    void OnDestroy()
    {
        if (CueManager.Instance != null)
        {
            CueManager.Instance.OnCueSheetChanged -= RefreshList;
            CueManager.Instance.OnCueActivated -= HighlightCue;
        }
        if (AppController.Instance != null)
            AppController.Instance.OnStateChanged -= OnStateChanged;
    }

    private void RefreshList()
    {
        _cueList.Clear();
        var cues = CueManager.Instance.CueSheet.cues;
        for (int i = 0; i < cues.Count; i++)
        {
            var cue = cues[i];
            var row = new Label($"Cue {cue.cueNumber}  {cue.label}  (fade {cue.fadeTime:F1}s)");
            row.name = $"cue-row-{i}";
            row.style.color = Color.white;
            row.style.paddingTop = 5;
            row.style.paddingBottom = 5;
            row.style.paddingLeft = 8;
            _cueList.Add(row);
        }
    }

    private void HighlightCue(int index)
    {
        for (int i = 0; i < CueManager.Instance.CueSheet.cues.Count; i++)
        {
            var row = _cueList.Q<Label>($"cue-row-{i}");
            if (row == null) continue;
            row.style.backgroundColor = i == index
                ? new Color(0.2f, 0.5f, 0.2f)
                : Color.clear;
        }
    }

    private void OnStateChanged(AppController.AppState state)
    {
        Show(state == AppController.AppState.CueSheet);
        if (state == AppController.AppState.CueSheet)
            RefreshList();
    }

    private void Show(bool visible) =>
        _panel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
}
