using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FixtureManager : MonoBehaviour
{
    public static FixtureManager Instance { get; private set; }

    private readonly List<FixtureInstance> _fixtures = new();
    private int _nextId;

    public FixtureInstance Selected { get; private set; }
    public IReadOnlyList<FixtureInstance> Fixtures => _fixtures;

    public event Action<FixtureInstance> OnFixtureAdded;
    public event Action<FixtureInstance> OnFixtureSelected;

    void Awake() => Instance = this;

    public FixtureInstance PlaceFixture(FixtureProfile profile, Vector3 position)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = profile.fixtureName;
        go.transform.position = position;
        go.transform.localScale = Vector3.one * 0.3f;

        // Remove default material color so it doesn't look odd
        var mr = go.GetComponent<MeshRenderer>();
        mr.material.color = new Color(0.8f, 0.8f, 0.8f);

        var instance = go.AddComponent<FixtureInstance>();
        instance.Initialize(_nextId++, profile);
        _fixtures.Add(instance);
        OnFixtureAdded?.Invoke(instance);
        return instance;
    }

    public void SelectFixture(FixtureInstance fixture)
    {
        Selected = fixture;
        OnFixtureSelected?.Invoke(fixture);
    }

    public FixtureInstance GetById(int id) => _fixtures.FirstOrDefault(f => f.Id == id);
}
