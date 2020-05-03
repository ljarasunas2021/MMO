using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipNameTag : Action1
{
    public Canvas canvas;
    public Button equipButton, unEquipButton;
    private Button equipButtonInstant, unEquipButtonInstant;
    private bool clicked = false;
    private Equip equip;

    void Start()
    {
        equip = FindObjectOfType<Equip>();
    }

    public new IEnumerator Execute()
    {
        equipButtonInstant = Instantiate(equipButton, canvas.transform);
        unEquipButtonInstant = Instantiate(unEquipButton, canvas.transform);

        clicked = false;

        equipButtonInstant.onClick.AddListener(() => { ButtonClicked(0); });
        unEquipButtonInstant.onClick.AddListener(() => { ButtonClicked(1); });

        while (!clicked) yield return 0;
    }

    private void ButtonClicked(int index)
    {
        Destroy(equipButtonInstant);
        Destroy(unEquipButtonInstant);
        clicked = true;

        equip.ChangeEquipped((index == 0) ? true : false);
    }
}
