using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UILevelSelect : MonoBehaviour
{
    public AudioSource Music;
    public Image MusicImage;

    public Sprite on, off;

    public GameObject Page2;
    public GameObject Page2Text;
    public GameObject Page1Text;


    public GameObject shopUIMenu;
    public GameObject shopOpenButton;

    public GameObject helpPageMenu;


    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void MuteMusic()
    {
       

        if (Music.isPlaying)
            Music.Pause();


        else
            Music.Play();

        MusicImage.sprite = (Music.isPlaying) ? on : off;
    }


    public void NextPage()
    {
        Page2.SetActive(true);
        Page2Text.SetActive(true);
        Page1Text.SetActive(false);
    }

    public void BackPage()
    {
        Page2.SetActive(false);
        Page2Text.SetActive(false);
        Page1Text.SetActive(true);
    }

    private void Quit()
    {
        Application.Quit();
    }

    public void OpenShop()
    {
        shopUIMenu.SetActive(true);
        shopOpenButton.SetActive(false);
    }


    public void CloseShop()
    {
        shopUIMenu.SetActive(false);
        shopOpenButton.SetActive(true);
    }


    public void OpenHelpMenu()
    {
        helpPageMenu.SetActive(true);
    }

    public void CloseHelpMenu()
    {
        helpPageMenu.SetActive(false);
    }



}



