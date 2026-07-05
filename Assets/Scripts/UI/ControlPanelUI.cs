using UnityEngine;
using UnityEngine.UIElements;

public class ControlPanelUI : MonoBehaviour
{
    private VisualElement _panel;
    private Label _fixtureLabel;
    private Slider _sliderR, _sliderG, _sliderB;
    private Slider _sliderIntensity, _sliderPan, _sliderTilt, _sliderBeam;
    private bool _updating;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _panel = root.Q("control-panel");

        _fixtureLabel = _panel.Q<Label>("lbl-fixture-name");
        _sliderR = _panel.Q<Slider>("slider-r");
        _sliderG = _panel.Q<Slider>("slider-g");
        _sliderB = _panel.Q<Slider>("slider-b");
        _sliderIntensity = _panel.Q<Slider>("slider-intensity");
        _sliderPan = _panel.Q<Slider>("slider-pan");
        _sliderTilt = _panel.Q<Slider>("slider-tilt");
        _sliderBeam = _panel.Q<Slider>("slider-beam");

        _sliderR.RegisterValueChangedCallback(_ => ApplyToFixture());
        _sliderG.RegisterValueChangedCallback(_ => ApplyToFixture());
        _sliderB.RegisterValueChangedCallback(_ => ApplyToFixture());
        _sliderIntensity.RegisterValueChangedCallback(_ => ApplyToFixture());
        _sliderPan.RegisterValueChangedCallback(_ => ApplyToFixture());
        _sliderTilt.RegisterValueChangedCallback(_ => ApplyToFixture());
        _sliderBeam.RegisterValueChangedCallback(_ => ApplyToFixture());

        _panel.Q<Button>("btn-record-cue").clicked += () => CueManager.Instance.RecordCue();
        _panel.Q<Button>("btn-edit-cuesheet").clicked += () => AppController.Instance.EnterCueSheet();

        _panel.RegisterCallback<MouseEnterEvent>(_ => AppController.IsPointerOverUI = true);
        _panel.RegisterCallback<MouseLeaveEvent>(_ => AppController.IsPointerOverUI = false);

        FixtureManager.Instance.OnFixtureSelected += OnFixtureSelected;
        AppController.Instance.OnStateChanged += OnStateChanged;
        Show(false);
    }

    void OnDestroy()
    {
        if (FixtureManager.Instance != null) FixtureManager.Instance.OnFixtureSelected -= OnFixtureSelected;
        if (AppController.Instance != null) AppController.Instance.OnStateChanged -= OnStateChanged;
    }

    private void OnFixtureSelected(FixtureInstance fixture)
    {
        if (fixture == null) return;
        _fixtureLabel.text = $"{fixture.Profile.fixtureName} #{fixture.Id}";
        _updating = true;
        var p = fixture.GetParameters();
        _sliderR.value = p.color.r;
        _sliderG.value = p.color.g;
        _sliderB.value = p.color.b;
        _sliderIntensity.value = p.intensity;
        _sliderPan.value = p.pan;
        _sliderTilt.value = p.tilt;
        _sliderBeam.value = p.beamAngle;
        _updating = false;
    }

    private void ApplyToFixture()
    {
        if (_updating) return;
        var fixture = FixtureManager.Instance.Selected;
        if (fixture == null) return;
        fixture.ApplyParameters(new LightParameters
        {
            color = new Color(_sliderR.value, _sliderG.value, _sliderB.value),
            intensity = _sliderIntensity.value,
            pan = _sliderPan.value,
            tilt = _sliderTilt.value,
            beamAngle = _sliderBeam.value
        });
    }

    private void OnStateChanged(AppController.AppState state) =>
        Show(state == AppController.AppState.Staging);

    private void Show(bool visible) =>
        _panel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
}
