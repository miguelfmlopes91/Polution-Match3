using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainPauseMenu : MonoBehaviour
{
    public GameObject PauseMenuPanel;





    public void OpenMenu()
    {
        PauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;

    }

    public void CloseMenu()
    {
        PauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }


    public void QuitGame()
    {
        Application.Quit();
    }


}
