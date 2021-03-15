using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MMO.Player;

public class PotionUI : MonoBehaviour
{
    public Text healthPotionText;
    private float healthPotionAmt = 0;
    public static PotionUI instance;

    private void Start()
    {
        instance = this;
    }

    public void UpdateHealthPotionText(float count)
    {
        healthPotionAmt = count;
        healthPotionText.text = "x " + healthPotionAmt;
    }

    public void IncreaseHealthPotionText(float count)
    {
        float newCount = healthPotionAmt + count;
        if (newCount < 0) newCount = 0;
        UpdateHealthPotionText(newCount);
    }

    public void UsePotion(PotionTypes potionType)
    {
        if (potionType == PotionTypes.Health)
        {
            if (healthPotionAmt > 0)
            {
                foreach (PlayerHealth ph in FindObjectsOfType<PlayerHealth>())
                {
                    if (ph.isLocalPlayer)
                    {
                        ph.SubtractHealth(-25);
                    }
                }
                IncreaseHealthPotionText(-1);
            }
        }
    }
}

public enum PotionTypes {Health, None}

