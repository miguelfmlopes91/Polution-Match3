using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmPanel : MonoBehaviour
{

    public string levelToLoad;
    public int level;
    private GameData gameData;
    private int highScore;

    public Text LevelTextSetting;

    public Text highScoreText;

    public HeartManager heartManager;


    public StarManager starManager;
    void OnEnable()
    {
        heartManager = FindObjectOfType<HeartManager>();
        gameData = FindObjectOfType<GameData>();
        LoadData();
        SetText();
        LevelSetText();
    }

    void LoadData()
    {
        if(gameData != null)
        {
            highScore = gameData.saveData.highScores[level - 1];
        }
    }
    
    void SetText()
    {
        highScoreText.text = "" + highScore;
    }
    
    
    void Update()
    {
        
    }



    public void Cancel()
    {
        this.gameObject.SetActive(false);

    }

    public void Play()
    {
        if(heartManager.currentAmountOfHearts >= 1)
        {
            PlayerPrefs.SetInt("Current Level", level - 1);
            SceneManager.LoadScene(levelToLoad);
        }
        
    }


    void LevelSetText()
    {
        LevelTextSetting.text = level.ToString();
        starManager.index = level -1;
        starManager.UpdateImage();
    }



}
