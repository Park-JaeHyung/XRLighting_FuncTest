using System;
using UnityEngine;

public interface IPlacementInput
{
    event Action<Vector3> OnPlacementRequested;
    void SetPlacementMode(bool active, FixtureProfile profile = null);
}
