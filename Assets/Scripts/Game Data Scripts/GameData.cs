using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class SaveData
{
    public bool[] isActive;
    public int[] highScores;

    public int amountOfLives = 5;
    public int currentMoney;

    public int purchasedTimeUpgrades = 0;
    
}

public class GameData : MonoBehaviour
{
    public static GameData gameData;
    public SaveData saveData;

    public World world;
    
    public Sprite[] starsImages;
    
    void Awake()
    {
        if(gameData == null)
        {
            DontDestroyOnLoad(this.gameObject);
            gameData = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        Load();

    }

    private void Start()
    {
       
    }

    public void Save()
    {
        CreateSaveFile(Application.persistentDataPath + "/player.data");
        
        //Create a binary formatter which can read binary files
        BinaryFormatter formatter = new BinaryFormatter();
        
        //Create a route from the program to the file
        FileStream file = File.Open(Application.persistentDataPath + "/player.data", FileMode.Open);

        //Create a copy of save data
        SaveData data = new SaveData();
        data = saveData;
        
        //Actually save the data in the file
        formatter.Serialize(file, data);
        
        //Close the Data stream
        file.Close();
        
        Debug.Log("Saved");
    }

    public void Load()
    {
        //Check if the save game file exists
        if(File.Exists(Application.persistentDataPath + "/player.data"))
        {
            //Create Binary Formatter
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/player.data", FileMode.Open);
            saveData = formatter.Deserialize(file) as SaveData;
            file.Close();
            saveData.isActive[0] = true;
            Debug.Log("Loaded");
        }
        else
        {
            saveData = new SaveData();
            saveData.isActive = new bool[15];
            saveData.highScores = new int[15];

            

            saveData.isActive[0] = true;
        }
    }

    private void OnDisable()
    {
        Save();
    }

    void Update()
    {
        
    }


    void CreateSaveFile(string filepath)
    {
        if (!File.Exists(filepath))
        {
            
            var file = File.Create(filepath);
            file.Close();
            
        }
        
    }


     public static int CalStars(int index)
    {
        if (index < gameData.world.levels.Length)
        {

            float highscore = gameData.saveData.highScores[index];
            int numberofstars = 0;
            foreach (var threshold in gameData.world.levels[index].scoreGoals)
            {
                if (highscore >= threshold)
                {
                    numberofstars++;
                }
            }
            return numberofstars;
        }

        return 0;
    }

}
