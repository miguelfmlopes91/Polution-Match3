using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "World", menuName = "Level")]
public class Level : ScriptableObject
{
    [Header("Board Dimensions")]
    public int width;
    public int height;

    [Header("Starting Tiles")]
    public TileType[] boardLayout;

    [Header("Available Dots")]
    public GameObject[] dots;

    [Header("ScoreGoals")]
    public int[] scoreGoals;

    [Header("EndGame Requirements")]
    public EndGameRequirments endGameRequirments;
    public BlankGoal[] levelGoals;

    


}
