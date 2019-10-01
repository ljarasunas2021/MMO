using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    [SerializeField] private Canvas pauseMenu;

    public bool togglePauseMenu;

    void Start() {
        pauseMenu.enabled = false;
    }
    public void TogglePauseMenu() {
        togglePauseMenu = !togglePauseMenu;
        pauseMenu.enabled = !pauseMenu.enabled;
    }

    public void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
