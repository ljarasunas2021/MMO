using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Equip : MonoBehaviour
{
    public bool equipped;
    public TextMeshProUGUI buttonText;

    void Start()
    {
        ChangeEquipped(equipped);
    }

    public void ChangeEquipped()
    {
        equipped = !equipped;
        buttonText.text = (equipped) ? "Unequip" : "Equip";
    }

    public void ChangeEquipped(bool equipped)
    {
        this.equipped = equipped;
        buttonText.text = (equipped) ? "Unequip" : "Equip";
    }
}
