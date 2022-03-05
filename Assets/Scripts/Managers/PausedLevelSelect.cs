using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PausedLevelSelect : MonoBehaviour
{
    public GameObject pausedUI;


    public void ClosePauseMenu()
    {
        pausedUI.gameObject.SetActive(false);
    }

    public void OpenPauseMenu()
    {
        pausedUI.gameObject.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }



}
