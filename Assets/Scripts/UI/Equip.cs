using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// equip ui
public class Equip : MonoBehaviour
{
    // item is equipped
    public bool equipped;
    // equip button
    public TextMeshProUGUI buttonText;

    // Change equipped
    void Start()
    {
        ChangeEquipped(equipped);
    }

    // toggle equipped
    public void ChangeEquipped()
    {
        equipped = !equipped;
        buttonText.text = (equipped) ? "Unequip" : "Equip";
    }

    // change the equipped
    public void ChangeEquipped(bool equipped)
    {
        this.equipped = equipped;
        buttonText.text = (equipped) ? "Unequip" : "Equip";
    }
}
