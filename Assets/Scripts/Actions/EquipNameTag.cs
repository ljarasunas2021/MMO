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
    public GameObject dialogueBox;

    void Start()
    {
        equip = FindObjectOfType<Equip>();
    }

    public override IEnumerator Execute()
    {
        dialogueBox.SetActive(false);
        UIManager.LockCursor(false);

        equipButtonInstant = Instantiate(equipButton, canvas.transform);
        unEquipButtonInstant = Instantiate(unEquipButton, canvas.transform);

        clicked = false;

        equipButtonInstant.onClick.AddListener(() => { ButtonClicked(0); });
        unEquipButtonInstant.onClick.AddListener(() => { ButtonClicked(1); });

        while (!clicked) yield return 0;

        UIManager.LockCursor(true);
        dialogueBox.SetActive(true);
    }

    private void ButtonClicked(int index)
    {
        Destroy(equipButtonInstant.gameObject);
        Destroy(unEquipButtonInstant.gameObject);
        clicked = true;

        equip.ChangeEquipped((index == 0) ? true : false);
    }
}
