using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image red, green;
    private float maxHealth, startingWidth;
    private GameObject player;
    private RectTransform greenRect;

    private void Start() {
        greenRect = green.GetComponent<RectTransform>();
        startingWidth = greenRect.rect.width;
    }

    public void Initialize(GameObject player, float maxHealth) {
        this.player = player;
        this.maxHealth = maxHealth;
    }

    public void TakeDamage(float health) {
        greenRect.sizeDelta = new Vector2((health / maxHealth) * startingWidth, greenRect.rect.height);
    }
}
