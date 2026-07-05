using UnityEngine;

[CreateAssetMenu(fileName = "FixtureProfile", menuName = "XRLighting/Fixture Profile")]
public class FixtureProfile : ScriptableObject
{
    public string fixtureName = "Generic Fixture";
    public bool supportsColor = true;
    public bool supportsPanTilt = true;
    public bool supportsBeamAngle = true;
    public float intensityMax = 5f;
    public float beamAngleMin = 5f;
    public float beamAngleMax = 60f;
}
