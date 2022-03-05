using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class BackToSelect : MonoBehaviour
{

    public string sceneToLoad;
    private GameData gameData;
    private Board board;
    private HeartManager heartManager;
    public EndGameManager endgameManager;
    
    void start()
    {
        endgameManager = FindObjectOfType<EndGameManager>();
    }

    

   
        
    
    public void WinOk()
    {
        if(gameData != null)
        {
            gameData.saveData.isActive[board.level + 1] = true;
            gameData.Save();
        }
        SceneManager.LoadScene(sceneToLoad);
    }
    
    
    public void LoseOK()
    {
        if (gameData != null)
        {
           
            gameData.saveData.amountOfLives -= 1;
            gameData.Save();

            Time.timeScale = 1f;
        }

        SceneManager.LoadScene(sceneToLoad);

        


    }


    public void ActuallyNoThankYou()
    {
        SceneManager.LoadScene(sceneToLoad);
    }



    void Start()
    {
        gameData = FindObjectOfType<GameData>();
        board = FindObjectOfType<Board>();
        heartManager = FindObjectOfType<HeartManager>();
    }

    
    void Update()
    {
        
    }
}
