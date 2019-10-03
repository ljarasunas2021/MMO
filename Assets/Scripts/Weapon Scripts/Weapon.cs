using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponType weaponType;

    public double damage;

    [Header("Melee Weapon Settings")]
    public int speed;

    [Header("Ranged Weapon Settings")]
    public int reloadTime;

    [Header("Collectable Weapon Settings")]
    public int maxCount;

    void Start()
    {

    }

    void Update()
    {

    }
}

[System.Serializable]
public enum WeaponType
{
    melee,
    ranged,
    collectable
}
