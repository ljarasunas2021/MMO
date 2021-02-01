using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScenes : MonoBehaviour
{
    public string sceneName;
    public void SwitchGame()
    {
        SceneManager.LoadScene(sceneName);
    }
}
