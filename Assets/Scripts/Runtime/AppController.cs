using UnityEngine;

public class AppController : MonoBehaviour
{
    public static AppController Instance { get; private set; }
    public static bool IsPointerOverUI { get; set; }

    public enum AppState { SpaceSelection, Staging, CueSheet }

    public AppState State { get; private set; } = AppState.SpaceSelection;

    public event System.Action<AppState> OnStateChanged;

    void Awake() => Instance = this;

    void Start()
    {
        EnsureFloor();
    }

    private void EnsureFloor()
    {
        if (GameObject.Find("Floor") != null) return;
        var floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
        floor.name = "Floor";
        floor.transform.localScale = new Vector3(5f, 1f, 5f);
        floor.GetComponent<MeshRenderer>().material.color = new Color(0.15f, 0.15f, 0.15f);
    }

    public void EnterStaging(string spaceName)
    {
        State = AppState.Staging;
        OnStateChanged?.Invoke(State);
    }

    public void EnterCueSheet()
    {
        State = AppState.CueSheet;
        OnStateChanged?.Invoke(State);
    }

    public void ExitCueSheet()
    {
        State = AppState.Staging;
        OnStateChanged?.Invoke(State);
    }
}
