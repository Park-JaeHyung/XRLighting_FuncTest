using UnityEngine;

public class FixtureInstance : MonoBehaviour, ILightControllable
{
    public int Id { get; private set; }
    public FixtureProfile Profile { get; private set; }

    private Light _light;
    private LightParameters _parameters = new();

    public void Initialize(int id, FixtureProfile profile)
    {
        Id = id;
        Profile = profile;

        var lightGo = new GameObject("Light");
        lightGo.transform.SetParent(transform);
        lightGo.transform.localPosition = Vector3.zero;
        lightGo.transform.localRotation = Quaternion.identity;
        _light = lightGo.AddComponent<Light>();
        _light.type = LightType.Spot;
        _light.range = 20f;

        ApplyParameters(_parameters);
    }

    public LightParameters GetParameters() => _parameters.Clone();

    public void ApplyParameters(LightParameters p)
    {
        _parameters = p.Clone();
        if (_light == null) return;
        _light.color = p.color;
        _light.intensity = p.intensity;
        _light.spotAngle = Mathf.Clamp(p.beamAngle, 1f, 179f);
        transform.rotation = Quaternion.Euler(-p.tilt, p.pan, 0f);
    }
}
