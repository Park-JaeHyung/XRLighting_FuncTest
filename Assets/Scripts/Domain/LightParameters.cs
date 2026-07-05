using UnityEngine;

[System.Serializable]
public class LightParameters
{
    public Color color = Color.white;
    public float intensity = 1f;
    public float pan;
    public float tilt;
    public float beamAngle = 30f;

    public LightParameters Clone() => new LightParameters
    {
        color = color,
        intensity = intensity,
        pan = pan,
        tilt = tilt,
        beamAngle = beamAngle
    };
}
