using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    public AudioSource Music;
    public Image MusicImage;




    public Text frontMoneyMenu;


    public Sprite on, off;

    public Text levelText1, levelText2;
    public string prefix = "Level: ";


    [Header("SoundEffects")]
    
    public Image audioSoundEffectImage;

    public AudioSource[] SoundEffects;

    public Sprite working, notworking;





    public void MuteMusic()
    {
        if (Music.isPlaying)
            Music.Pause();
        else
            Music.Play();

        MusicImage.sprite = (Music.isPlaying) ? on : off;
    }

    public void UpdateLevelText(int levelNumber)
    {
        levelNumber += 1; 
        levelText1.text = prefix + levelNumber;
        levelText2.text =  levelNumber.ToString();
    }

    private void Start()
    {
        frontMoneyMenu.text = "$" + GameData.gameData.saveData.currentMoney.ToString();
    }

    public void MuteSoundEffects()
    {

        foreach (var Sourcy in SoundEffects)
        {
            audioSoundEffectImage.sprite = (Sourcy.enabled) ? working : notworking;
            Sourcy.enabled = !Sourcy.enabled;
        }


       


    }


}
