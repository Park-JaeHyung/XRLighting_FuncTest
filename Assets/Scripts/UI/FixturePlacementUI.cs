using UnityEngine;
using UnityEngine.UIElements;

public class FixturePlacementUI : MonoBehaviour
{
    private VisualElement _panel;
    private PCPlacementInput _input;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _panel = root.Q("fixture-placement");

        _panel.Q<Button>("btn-done-placement").clicked += () => _input?.SetPlacementMode(false);

        _panel.RegisterCallback<MouseEnterEvent>(_ => AppController.IsPointerOverUI = true);
        _panel.RegisterCallback<MouseLeaveEvent>(_ => AppController.IsPointerOverUI = false);

        _input = FindFirstObjectByType<PCPlacementInput>();
        _input.OnPlacementRequested += OnPlacementRequested;

        PopulateProfiles();

        AppController.Instance.OnStateChanged += OnStateChanged;
        Show(false);
    }

    void OnDestroy()
    {
        if (_input != null) _input.OnPlacementRequested -= OnPlacementRequested;
        if (AppController.Instance != null) AppController.Instance.OnStateChanged -= OnStateChanged;
    }

    private void PopulateProfiles()
    {
        var list = _panel.Q("fixture-list");
        list.Clear();

        var profiles = Resources.LoadAll<FixtureProfile>("FixtureProfiles");
        foreach (var profile in profiles)
        {
            var p = profile;
            var btn = new Button { text = p.fixtureName };
            btn.style.marginBottom = 4;
            btn.clicked += () => _input?.SetPlacementMode(true, p);
            list.Add(btn);
        }
    }

    private void OnPlacementRequested(Vector3 position)
    {
        FixtureManager.Instance.PlaceFixture(GetSelectedProfile(), position);
    }

    private FixtureProfile GetSelectedProfile()
    {
        var profiles = Resources.LoadAll<FixtureProfile>("FixtureProfiles");
        return profiles.Length > 0 ? profiles[0] : null;
    }

    private void OnStateChanged(AppController.AppState state)
    {
        Show(state == AppController.AppState.Staging);
        if (state != AppController.AppState.Staging)
            _input?.SetPlacementMode(false);
    }

    private void Show(bool visible) =>
        _panel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
}
