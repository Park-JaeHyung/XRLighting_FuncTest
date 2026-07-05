using System.Collections.Generic;

[System.Serializable]
public class Cue
{
    public int cueNumber;
    public string label;
    public float fadeTime = 1f;
    public List<FixtureSnapshot> snapshots = new();

    [System.Serializable]
    public class FixtureSnapshot
    {
        public int fixtureId;
        public LightParameters parameters;
    }
}
