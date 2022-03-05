using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HeartManager : MonoBehaviour
{
    

    public Text amountOfHeartsText;
    public Text countdownTimeText;


    // Needs to be converted into 10:00 mins
    public float maxTimer = 180;
    public float currentTimer = 180;


    public int currentAmountOfHearts = 5;
    public int maxHeartsAllowed = 5;


    public Text mainMoneyMenu;








    void Start()
    {
        currentAmountOfHearts = GameData.gameData.saveData.amountOfLives;
        SetTextUILol();

        mainMoneyMenu.text = GameData.gameData.saveData.currentMoney.ToString();

    }

   
    void Update()
    {

        

        if (currentAmountOfHearts < maxHeartsAllowed)
        {
            
            currentTimer -= Time.deltaTime;
            if (currentTimer <= 0)
            {
                currentAmountOfHearts++;
                currentTimer = maxTimer;

                GameData.gameData.saveData.amountOfLives = currentAmountOfHearts;
            }

            SetTextUILol();

        }


    }



    void SetTextUILol()
    {
        TimeSpan t = TimeSpan.FromSeconds(currentTimer);

        string answer = string.Format("{0:D2}:{1:D2}",
                        
                        t.Minutes,
                        t.Seconds);


        amountOfHeartsText.text = currentAmountOfHearts.ToString();
        countdownTimeText.text = answer;

        if(currentAmountOfHearts == maxHeartsAllowed)
        {
            countdownTimeText.text = "Full!";
        }

    }



    public void LivesCount(int NumberOfHearts)
    {
        currentAmountOfHearts += NumberOfHearts;

        GameData.gameData.saveData.amountOfLives = currentAmountOfHearts;
        mainMoneyMenu.text = GameData.gameData.saveData.currentMoney.ToString();
        SetTextUILol();
    }


    public void UpdateTimer(float timerDecrease)
    {
        maxTimer -= timerDecrease;
        Debug.Log(maxTimer);
        if (currentTimer > maxTimer)
            currentTimer = maxTimer;

        mainMoneyMenu.text = GameData.gameData.saveData.currentMoney.ToString();
    }


}
