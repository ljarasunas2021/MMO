using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MMO.UI
{
    /// <summary> UI for equipping </summary>
    public class Equip : MonoBehaviour
    {
        // is an item equipped
        public bool equipped;
        // equip button
        public TextMeshProUGUI buttonText;

        /// <summary> Change equipped automatically </summary>
        void Start()
        {
            ChangeEquipped(equipped);
        }

        /// <summary> Toggle equipped </summary>
        public void ChangeEquipped()
        {
            equipped = !equipped;
            buttonText.text = (equipped) ? "Unequip" : "Equip";
        }

        /// <summary> Change equipped variable </summary>
        /// <param name="equipped"> new value of equipped variable </param>
        public void ChangeEquipped(bool equipped)
        {
            this.equipped = equipped;
            buttonText.text = (equipped) ? "Unequip" : "Equip";
        }
    }
}
