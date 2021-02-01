using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Manages the health bar UI</summary>
public class HealthBar : MonoBehaviour
{
    // singleton
    public static HealthBar instance;
    // red and green images for health bar
    public Image red, green;
    // max health for health bar, starting width of green image
    private float maxHealth, startingWidth;
    // player gameobject
    private GameObject player;
    // rect transform of green image
    private RectTransform greenRect;

    /// <summary> Init vars, make singleton </summary>
    private void Start() {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("There already exists another health bar script");
        }

        greenRect = green.GetComponent<RectTransform>();
        startingWidth = greenRect.rect.width;
    }

    /// <summary> Init the player vars, called when local player starts their game</summary>
    /// <param name="player"> local player gameobject </param>
    /// <param name="maxHealth"> maximum health of player </param>
    public void Initialize(GameObject player, float maxHealth) {
        this.player = player;
        this.maxHealth = maxHealth;
    }

    /// <summary> Set the health of the health bar </summary>
    /// <param name="health"> health for the health bar </param>
    public void SetHealth(float health) {
        greenRect.sizeDelta = new Vector2((health / maxHealth) * startingWidth, greenRect.rect.height);
    }
}
