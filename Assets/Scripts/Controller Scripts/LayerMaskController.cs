using UnityEngine;

/// <summary> Static class that contains references to all layers </summary>
public static class LayerMaskController
{
    // contains the layermask for the ____ layer
    public static LayerMask environment = 10, player = 9, item = 11, playerNonRagdoll = 13;
}
