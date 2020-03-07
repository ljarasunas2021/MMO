using UnityEngine;

/// <summary> Contains references to the layer masks </summary>
public static class LayerMaskController
{
    // contains the layermask for the ____ layer
    // environment
    public static LayerMask environment = 10;
    // player
    public static LayerMask player = 9;
    // item
    public static LayerMask item = 11;
    public static LayerMask playerNonRagdoll = 13;
}
