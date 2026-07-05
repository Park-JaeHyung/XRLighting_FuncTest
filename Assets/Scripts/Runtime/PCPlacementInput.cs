using System;
using UnityEngine;

public class PCPlacementInput : MonoBehaviour, IPlacementInput
{
    public event Action<Vector3> OnPlacementRequested;

    private bool _placementActive;
    private Camera _camera;

    void Start() => _camera = Camera.main;

    public void SetPlacementMode(bool active, FixtureProfile profile = null)
    {
        _placementActive = active;
    }

    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        if (AppController.IsPointerOverUI) return;
        if (_camera == null) return;

        var ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (_placementActive)
        {
            // Place fixture on anything that's not another fixture
            if (Physics.Raycast(ray, out var hit) && hit.collider.GetComponent<FixtureInstance>() == null)
                OnPlacementRequested?.Invoke(hit.point + Vector3.up * 2f);
        }
        else
        {
            // Select existing fixture
            if (Physics.Raycast(ray, out var hit))
            {
                var fixture = hit.collider.GetComponent<FixtureInstance>();
                if (fixture != null)
                    FixtureManager.Instance.SelectFixture(fixture);
            }
        }
    }
}
