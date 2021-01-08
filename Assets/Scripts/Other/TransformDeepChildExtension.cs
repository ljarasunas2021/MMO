using System.Collections.Generic;
using UnityEngine;

/// <summary> Find a child of a gameobject with a certain name </summary>
public static class TransformDeepChildExtension
{
    /// <summary> Find a child of a gameobject with a certain name </summary>
    /// <param name="aParent"> the transform of the parent that this function should search children of </param>
    /// <param name="aName"> the name of the child this function should search for </param>
    /// <returns> the transform of the child </returns>
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == aName)
                return c;
            foreach (Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }
}