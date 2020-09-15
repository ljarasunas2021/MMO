using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// manages health bar
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

    // get correct variables
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

    // intialize player vars
    public void Initialize(GameObject player, float maxHealth) {
        this.player = player;
        this.maxHealth = maxHealth;
    }

    // take damage appropriately
    public void TakeDamage(float health) {
        greenRect.sizeDelta = new Vector2((health / maxHealth) * startingWidth, greenRect.rect.height);
    }
}
