using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class ScoreManager : MonoBehaviour
{

    private Board board;
    public Text scoreText;
    public int score;
    public Image scoreBar;
    public Sprite[] starsSprite;
    public SpriteRenderer starImage;
    public Image endStarImage;
    public Text moneyEarnedText;
    public Text totalMoney;

    private GameData gameData;

    void Start()
    {
        board = FindObjectOfType<Board>();
        gameData = FindObjectOfType<GameData>();
    }

    
    void Update()
    {
        scoreText.text = "" + score;
    }


    public void IncreaseScore(int amountToIncrease, int []scoreGoals)
    {
        score += amountToIncrease;
        if (gameData != null)
        {
            int highScore = gameData.saveData.highScores[board.level];
            if(score > highScore)
            {
                gameData.saveData.highScores[board.level] = score;
            }
            gameData.Save();
        }
        if(board != null && scoreBar != null)
        {
            int length = board.scoreGoals.Length;
            scoreBar.fillAmount = (float)score / (float)board.scoreGoals[length - 1];

        }
        int numberOfStars = 0;
        for (int i = 0; i < scoreGoals.Length; i++)
        {
            if (score >= scoreGoals[i])
            {
                numberOfStars++;

            }
        }

        starImage.sprite = starsSprite[numberOfStars];
    }



    public void EndScreenStars(int[] scoreGoals)
    {
        int numberOfStars = 0;
        for (int i = 0; i < scoreGoals.Length; i++)
        {
            if (score >= scoreGoals[i])
            {
                numberOfStars++;

            }
        }

        endStarImage.sprite = starsSprite[numberOfStars];

        moneyEarnedText.text = MoneyManager.AddCoins(numberOfStars).ToString();

        totalMoney.text = "$" + MoneyManager.Money.ToString();
    }



}
