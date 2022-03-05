using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShopManager : MonoBehaviour
{
    public MoneyManager moneyManager;

    public int[] timerUpgradeCost;
    public int secondsToDecrease;

    public Text decreaseTimerCostText;
    public GameObject decreaseTimerButton;


    void Start()
    {

        moneyManager = FindObjectOfType<MoneyManager>();
        HeartManager heartManager = FindObjectOfType<HeartManager>();

        heartManager.UpdateTimer(secondsToDecrease * GameData.gameData.saveData.purchasedTimeUpgrades);

        if (GameData.gameData.saveData.purchasedTimeUpgrades == timerUpgradeCost.Length)
        {
            decreaseTimerButton.SetActive(false);
            decreaseTimerCostText.text = "MAX";
        }
        else
        {
            decreaseTimerCostText.text = "$" + timerUpgradeCost[GameData.gameData.saveData.purchasedTimeUpgrades].ToString();
        }
    }

  
    void Update()
    {
        
    }



    public void Buy1Heart()
    {
        HeartManager heartManager = FindObjectOfType<HeartManager>();
        
        if (MoneyManager.Money >= 150 && heartManager.currentAmountOfHearts < heartManager.maxHeartsAllowed)
        {
           
            MoneyManager.GoldCount(-150);
            heartManager.LivesCount(1);


        }



    }


    public void BuyMaxHearts()
    {

    }

    public void BuyDecreaseTimer()
    {
        HeartManager heartManager = FindObjectOfType<HeartManager>();

        if (MoneyManager.Money >= timerUpgradeCost[GameData.gameData.saveData.purchasedTimeUpgrades] && GameData.gameData.saveData.purchasedTimeUpgrades < timerUpgradeCost.Length)
        {
            MoneyManager.GoldCount(-timerUpgradeCost[GameData.gameData.saveData.purchasedTimeUpgrades]);
            GameData.gameData.saveData.purchasedTimeUpgrades++;

            heartManager.UpdateTimer(secondsToDecrease);


            if (GameData.gameData.saveData.purchasedTimeUpgrades == timerUpgradeCost.Length) 
            {
                decreaseTimerButton.SetActive(false);
                decreaseTimerCostText.text = "MAX";
            }
            else
            {
                decreaseTimerCostText.text = "$" + timerUpgradeCost[GameData.gameData.saveData.purchasedTimeUpgrades].ToString();
            }


        }

        
        



    }


}
