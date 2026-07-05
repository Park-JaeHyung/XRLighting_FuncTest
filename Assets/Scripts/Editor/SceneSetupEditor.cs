using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

public static class SceneSetupEditor
{
    [MenuItem("XRLighting/Setup Scene")]
    public static void SetupScene()
    {
        CreateDirectories();
        CreateFixtureProfiles();
        SetupManagers();
        SetupUIGameObject();
        SetupCamera();

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        AssetDatabase.Refresh();
        Debug.Log("[XRLighting] Scene setup complete. Press Play to run.");
    }

    private static void CreateDirectories()
    {
        var dirs = new[]
        {
            "Assets/Resources/FixtureProfiles",
            "Assets/UI",
            "Assets/ScriptableObjects"
        };
        foreach (var d in dirs)
        {
            if (!AssetDatabase.IsValidFolder(d))
            {
                var parts = d.Split('/');
                var parent = string.Join("/", parts[..^1]);
                AssetDatabase.CreateFolder(parent, parts[^1]);
            }
        }
    }

    private static void CreateFixtureProfiles()
    {
        CreateProfile("MovingHead", intensityMax: 8f, beamMin: 5f, beamMax: 40f);
        CreateProfile("PAR", intensityMax: 5f, beamMin: 20f, beamMax: 60f, panTilt: false);
        CreateProfile("Spot", intensityMax: 6f, beamMin: 8f, beamMax: 30f);
    }

    private static void CreateProfile(string profileName, float intensityMax, float beamMin, float beamMax, bool panTilt = true)
    {
        var path = $"Assets/Resources/FixtureProfiles/{profileName}.asset";
        if (AssetDatabase.LoadAssetAtPath<FixtureProfile>(path) != null) return;

        var profile = ScriptableObject.CreateInstance<FixtureProfile>();
        profile.fixtureName = profileName;
        profile.intensityMax = intensityMax;
        profile.beamAngleMin = beamMin;
        profile.beamAngleMax = beamMax;
        profile.supportsPanTilt = panTilt;
        AssetDatabase.CreateAsset(profile, path);
    }

    private static void SetupManagers()
    {
        // Remove old managers if exist
        foreach (var name in new[] { "Managers" })
        {
            var old = GameObject.Find(name);
            if (old != null) Object.DestroyImmediate(old);
        }

        var managers = new GameObject("Managers");
        managers.AddComponent<AppController>();
        managers.AddComponent<FixtureManager>();
        managers.AddComponent<CueManager>();
        managers.AddComponent<PCPlacementInput>();
    }

    private static void SetupUIGameObject()
    {
        var old = GameObject.Find("UI");
        if (old != null) Object.DestroyImmediate(old);

        var uiGo = new GameObject("UI");

        // PanelSettings
        var psPath = "Assets/UI/PanelSettings.asset";
        var ps = AssetDatabase.LoadAssetAtPath<PanelSettings>(psPath);
        if (ps == null)
        {
            ps = ScriptableObject.CreateInstance<PanelSettings>();
            ps.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            ps.referenceResolution = new Vector2Int(1920, 1080);
            ps.screenMatchMode = PanelScreenMatchMode.MatchWidthOrHeight;
            AssetDatabase.CreateAsset(ps, psPath);
        }

        var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UI/MainUI.uxml");
        if (uxml == null)
        {
            Debug.LogError("[XRLighting] Assets/UI/MainUI.uxml not found. Make sure the file exists.");
            return;
        }

        var doc = uiGo.AddComponent<UIDocument>();
        doc.panelSettings = ps;
        doc.visualTreeAsset = uxml;
        doc.sortingOrder = 0;

        uiGo.AddComponent<SpaceSelectionUI>();
        uiGo.AddComponent<FixturePlacementUI>();
        uiGo.AddComponent<ControlPanelUI>();
        uiGo.AddComponent<CueSheetEditorUI>();
    }

    private static void SetupCamera()
    {
        var cam = Camera.main;
        if (cam == null)
        {
            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            cam = camGo.AddComponent<Camera>();
            camGo.AddComponent<AudioListener>();
        }
        cam.transform.position = new Vector3(0f, 8f, -12f);
        cam.transform.rotation = Quaternion.Euler(30f, 0f, 0f);
        cam.backgroundColor = new Color(0.05f, 0.05f, 0.05f);
        cam.clearFlags = CameraClearFlags.SolidColor;
    }
}
