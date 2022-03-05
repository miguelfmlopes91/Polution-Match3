using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MoneyManager : MonoBehaviour
{
    static MoneyManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public Threshold[] thresholds;

    public static int Money = 0;

    private void Start()
    {
       Money = GameData.gameData.saveData.currentMoney;
    }

    public static int AddCoins(int NumberOfStars)
    {
        int amount = 0;

        foreach(var threshold in instance.thresholds)
        {
            if(NumberOfStars == threshold.numberOfStarsNeeded)
            {
                amount = Random.Range(threshold.minMoney, threshold.maxMoney + 1);

                Money += amount;

                GameData.gameData.saveData.currentMoney = Money;

            }
        }
        Debug.Log(amount);
        return amount;
    }


    public static void  GoldCount(int NumberOfGold)
    {
        Money += NumberOfGold;

        GameData.gameData.saveData.currentMoney = Money;
    }








}







[System.Serializable]
public struct Threshold
{
    public int numberOfStarsNeeded;
    public int minMoney;
    public int maxMoney;
}
