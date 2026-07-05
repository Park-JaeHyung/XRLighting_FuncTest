using UnityEngine;

public static class CueInterpolator
{
    public static LightParameters Interpolate(LightParameters from, LightParameters to, float t)
    {
        return new LightParameters
        {
            color = LerpColor(from.color, to.color, t),
            intensity = Mathf.Lerp(from.intensity, to.intensity, t),
            pan = LerpAngle(from.pan, to.pan, t),
            tilt = LerpAngle(from.tilt, to.tilt, t),
            beamAngle = Mathf.Lerp(from.beamAngle, to.beamAngle, t)
        };
    }

    private static Color LerpColor(Color a, Color b, float t)
    {
        Color.RGBToHSV(a, out float aH, out float aS, out float aV);
        Color.RGBToHSV(b, out float bH, out float bS, out float bV);
        float hDiff = bH - aH;
        if (hDiff > 0.5f) hDiff -= 1f;
        else if (hDiff < -0.5f) hDiff += 1f;
        return Color.HSVToRGB(Mathf.Repeat(aH + hDiff * t, 1f), Mathf.Lerp(aS, bS, t), Mathf.Lerp(aV, bV, t));
    }

    private static float LerpAngle(float a, float b, float t)
    {
        float diff = b - a;
        if (diff > 180f) diff -= 360f;
        else if (diff < -180f) diff += 360f;
        return a + diff * t;
    }
}
