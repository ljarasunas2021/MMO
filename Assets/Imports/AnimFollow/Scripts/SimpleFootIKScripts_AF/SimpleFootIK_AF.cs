using System.Collections;
using UnityEngine;

public partial class SimpleFootIK_AF : MonoBehaviour
{
    void Awake()
    {
        Awake2();
    }

    void FixedUpdate()
    {
        deltaTime = Time.fixedDeltaTime;
        DoSimpleFootIK();
    }

    void DoSimpleFootIK()
    {
        ShootIKRays();

        PositionFeet();

        animatorFollow.AnimFollow();
    }
}
