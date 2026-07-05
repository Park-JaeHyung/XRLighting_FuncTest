using UnityEngine;
using UnityEngine.UIElements;

public class SpaceSelectionUI : MonoBehaviour
{
    private VisualElement _panel;

    void Start()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        _panel = root.Q("space-selection");

        _panel.Q<Button>("btn-concert").clicked += () => AppController.Instance.EnterStaging("콘서트홀");
        _panel.Q<Button>("btn-theater").clicked += () => AppController.Instance.EnterStaging("극장");
        _panel.Q<Button>("btn-studio").clicked += () => AppController.Instance.EnterStaging("스튜디오");

        _panel.RegisterCallback<MouseEnterEvent>(_ => AppController.IsPointerOverUI = true);
        _panel.RegisterCallback<MouseLeaveEvent>(_ => AppController.IsPointerOverUI = false);

        AppController.Instance.OnStateChanged += OnStateChanged;
        Show(AppController.Instance.State == AppController.AppState.SpaceSelection);
    }

    void OnDestroy()
    {
        if (AppController.Instance != null)
            AppController.Instance.OnStateChanged -= OnStateChanged;
    }

    private void OnStateChanged(AppController.AppState state) =>
        Show(state == AppController.AppState.SpaceSelection);

    private void Show(bool visible) =>
        _panel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
}
