using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Money : MonoBehaviour
{
    public static Money instance;
    public float money = 0;
    public Text moneyText;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    public void IncreaseMoney(float amt)
    {
        money += amt;
        moneyText.text = "" + money;
    }
}
