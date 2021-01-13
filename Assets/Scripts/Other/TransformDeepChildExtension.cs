using System.Collections.Generic;
using UnityEngine;

namespace MMO
{
    /// <summary> Find a child of a gameobject with a certain name </summary>
    public static class TransformDeepChildExtension
    {
        /// <summary> Find a child of a gameobject with a certain name </summary>
        /// <param name="parent"> the transform of the parent that this function should search children of </param>
        /// <param name="name"> the name of the child this function should search for </param>
        /// <returns> the transform of the child </returns>
        public static Transform FindDeepChild(this Transform parent, string name)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(parent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == name) return c;
                foreach (Transform t in c) queue.Enqueue(t);
            }
            return null;
        }
    }
}
