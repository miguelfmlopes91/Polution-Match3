using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public enum GameType
{
    Moves,
    Time
}


[System.Serializable]
public class EndGameRequirments
{
    public GameType gameType;
    public int counterValue;
}
public class EndGameManager : MonoBehaviour
{
    
    public GameObject movesLabel;
    public GameObject timeLabel;
    public GameObject youWinPanel;
    public GameObject tryAgainPanel;
    public Text counter;
    public EndGameRequirments requirements;
    public int currentCounterValue;
    private Board board;
    private float timerSeconds;
    private ScoreManager scoreManager;

    bool finished = false;
    
    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        board = FindObjectOfType<Board>();
        SetGameType();
        SetupGame();  
    }

   
    void SetGameType()
    {
        if(board.world != null)
        {
            if (board.level < board.world.levels.Length)
            {
                if (board.world.levels[board.level] != null)
                {
                   requirements = board.world.levels[board.level].endGameRequirments;
                }
            }
                
            
                
            
        }
    }
    
    
    
    void SetupGame()
    {
        finished = false;
        currentCounterValue = requirements.counterValue;
        if(requirements.gameType == GameType.Moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else
        {
            timerSeconds = 1;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }

        counter.text = "" + currentCounterValue;
    
    
    }
    
    
    public void DecreaseCounterValue()
    {
        if(board.currentState != Gamestate.pause)
        {
            currentCounterValue--;
            counter.text = "" + currentCounterValue;
            if (currentCounterValue <= 0)
            {
                LoseGame();
            }
        }
        
     
    }
    
    public void WinGame()
    {
        if (finished)
            return;

        youWinPanel.SetActive(true);
        board.currentState = Gamestate.win;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
        scoreManager.EndScreenStars(board.scoreGoals);

        finished = true;
    }

    public void LoseGame()
    {
        tryAgainPanel.SetActive(true);
        board.currentState = Gamestate.lose;
        Debug.Log("You Lose!");
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }
    void Update()
    {
        if(requirements.gameType == GameType.Time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;
            if(timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }
}
