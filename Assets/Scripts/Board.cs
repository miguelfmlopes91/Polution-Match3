using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


public enum Gamestate
{
    Wait,
    Move,
    CheckMatches,
    MatchesChecked,
    Win,
    Lose,
    Pause,
    Swapping,
    None
}

public enum TileKind
{
    Breakable,
    Blank,
    Normal
}

[Serializable]
public class TileType
{
    public int x;
    public int y;
    public TileKind tileKind;
}

public class Board : MonoBehaviour
{
    [Header("Scriptable Object stuff")]
    public World world;
    public int level;


    public Gamestate currentState = Gamestate.Move;
    private Gamestate lastState = Gamestate.None;

    [Header("Board Dimenstions")]
    public int width;
    public int height;
    public int offSet;

    [Header("Prefabs")]
    public GameObject tilePrefab;
    public GameObject breakableTilePrefab;
    public GameObject[] dots;
    public GameObject destroyEffect;

    [Header("Layout")]
    public TileType[] boardLayout;
    private bool[,] blankSpaces;
    private BackgroundTile[,] breakableTiles;
    public GameObject[,] allDots;
    public Dot currentDot;
    private FindMatches findMatches;
    public int basePieceValue = 20;
    private int streakValue = 1;
    private ScoreManager scoreManager;
    private SoundManager soundManager;
    private GoalManager goalManager;
    private const float RefillDelay = 0.6f;
    public int[] scoreGoals;


    private void Awake()
    {
        if(PlayerPrefs.HasKey("Current Level"))
        {
            level = PlayerPrefs.GetInt("Current Level");
        }
        if(world != null)
        {
            if (level < world.levels.Length)
            {
                if (world.levels[level] != null)
                {
                    width = world.levels[level].width;
                    height = world.levels[level].height;
                    dots = world.levels[level].dots;
                    scoreGoals = world.levels[level].scoreGoals;
                    boardLayout = world.levels[level].boardLayout;
                }
            }
            
        }
    }
    
    private void Start()
    {
        goalManager = FindObjectOfType<GoalManager>();
        soundManager = FindObjectOfType<SoundManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        breakableTiles = new BackgroundTile[width, height];
        findMatches = FindObjectOfType<FindMatches>();
        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];
        SetUp();
        currentState = Gamestate.Pause;

        FindObjectOfType<UiManager>().UpdateLevelText(level);
    }

    private void Update()
    {
        if (lastState != currentState)
        {
            Debug.Log($"Current State : {currentState.ToString()}");
        }
        if (currentState == Gamestate.CheckMatches)
        {
            findMatches.FindAllMatches();
            currentState = Gamestate.MatchesChecked;
        }

        if (currentState == Gamestate.MatchesChecked && findMatches.currentMatches.Count > 0)
        {
            DestroyMatches();
            StartCoroutine(FillBoardCo());
            currentState = Gamestate.Wait;
        }

        lastState = currentState; //debug use
    }

    private void GenerateBlankSpaces()
    {
        for(int i = 0; i < boardLayout.Length; i++)
        {
            if(boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    private void GenerateBreakableTiles()
    {
        //Look at all the tiles in the layout
        for (int i = 0; i < boardLayout.Length; i ++)
        {
            //If a tile is a "Oil" tile
            if(boardLayout[i].tileKind == TileKind.Breakable)
            {
                //Create a "oil" tile at that position;
                Vector2 tempPosition = new Vector2(boardLayout[i].x, boardLayout[i].y);
                GameObject tile = Instantiate(breakableTilePrefab,tempPosition, Quaternion.identity);
                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTile>();

            }
        }
    }
    
    private void SetUp()
    {
        GenerateBlankSpaces();
        GenerateBreakableTiles();

        for (int i = 0; i < width; i ++)
        {
          
            for(int j = 0; j < height; j++) 
            {
                if(!blankSpaces[i,j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    Vector2 tilePosition = new Vector2(i, j);
                    GameObject backgroundTile = Instantiate(tilePrefab, tilePosition, Quaternion.identity) as GameObject;
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "(" + i + ", " + j + " )";
                    int dotToUse = Random.Range(0, dots.Length);
                    int maxIterations = 0;
                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                    }
                    var dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;

                    dot.transform.parent = transform;
                    dot.name = "(" + i + ", " + j + " ) - " + dot.tag;
                    allDots[i, j] = dot;
                }
             

            }
        
        }
    }
    
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if(allDots[column - 1, row] != null && allDots[column - 2, row] != null)
            {
                if (allDots[column - 1, row].CompareTag(piece.tag) && allDots[column - 2, row].CompareTag(piece.tag))
                {
                    return true;
                }
            }
          
            if(allDots[column, row - 1] != null && allDots[column, row - 2] != null)
            {
                if (allDots[column, row - 1].CompareTag(piece.tag) && allDots[column, row - 2].CompareTag(piece.tag))
                {
                    return true;
                }
            }
        }else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if(allDots[column, row - 1] != null && allDots[column, row - 2] != null)
                {
                    if (allDots[column, row - 1].CompareTag(piece.tag) && allDots[column, row - 2].CompareTag(piece.tag))
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if(allDots[column - 1, row] != null && allDots[column - 2, row] != null)
                {
                    if (allDots[column - 1, row].CompareTag(piece.tag) && allDots[column - 2, row].CompareTag(piece.tag))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private bool ColumnOrRow()
    {
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = findMatches.currentMatches[0].GetComponent<Dot>();
        if(firstPiece != null)
        {
            foreach (var dot in findMatches.currentMatches.Select(currentPiece => currentPiece.GetComponent<Dot>()))
            {
                if(dot.row == firstPiece.row)
                {
                    numberHorizontal++;
                }
                
                if(dot.column == firstPiece.column)
                {
                    numberVertical++;
                }
            }
        }
        return (numberVertical == 5 || numberHorizontal == 5);

    }
    
    private void CheckToMakeBombs()
    {
        if(findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
        {
            findMatches.CheckBombs();
        }
    
        if(findMatches.currentMatches.Count ==5 || findMatches.currentMatches.Count ==8)
        {
            if (ColumnOrRow())
            {
                //Make a color bomb
                //Is the current dot matched
                if (currentDot == null) return;
                if (currentDot.isMatched && !currentDot.isColorBomb)
                {
                    currentDot.isMatched = false;
                    currentDot.MakeColorBomb();
                }
                else
                {
                    if (currentDot.otherDot == null) return;
                    Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                    if (otherDot.isMatched && !otherDot.isColorBomb)
                    {
                        otherDot.isMatched = false;
                        otherDot.MakeColorBomb();
                    }
                }
            }
            else
            {
                //Make a Adjacent bomb
                //Is the current dot matched
                if (currentDot == null) return;
                if (currentDot.isMatched && !currentDot.isAdjacentBomb)
                {
                    currentDot.isMatched = false;
                    currentDot.MakeAdjacentBomb();
                }
                else
                {
                    if (currentDot.otherDot == null) return;
                    Dot otherDot = currentDot.otherDot.GetComponent<Dot>();
                    if (otherDot.isMatched && !otherDot.isAdjacentBomb)
                    {
                        otherDot.isMatched = false;
                        otherDot.MakeAdjacentBomb();
                    }
                }
            }
        }
    }
    
    private void DestroyMatchesAt(int column, int row)
    {
        if (!allDots[column, row].GetComponent<Dot>().isMatched) return;
        //How many elements are in the match pieces list from find matches
        if(findMatches.currentMatches.Count >= 4)
        {
            CheckToMakeBombs();
        }
            
        //Does a tile need to break?
        if(breakableTiles[column, row]!=null)
        {
            //If it does give one damage
            breakableTiles[column, row].TakeDamage(1);
            if(breakableTiles[column, row].hitPoints <= 0)
            {
                breakableTiles[column, row] = null;
            }
        }
            
        if(goalManager != null)
        {
            goalManager.CompareGoal(allDots[column, row].tag);
            goalManager.UpdateGoal();
        }
            
        //Does the sound manager exist?
        if(soundManager != null)
        {
            soundManager.PlayRandomDestroyNoise();
        }
            
        var particle = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);
        Destroy(particle, .5f);
        Destroy(allDots[column, row]);
        scoreManager.IncreaseScore(basePieceValue * streakValue, scoreGoals);
        allDots[column, row] = null;
    }

    private void DestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
    }

    private IEnumerator DecreaseRowCo()
    {
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j ++)
            {
                //If the current spot isn't blank or empty. . .
                if(!blankSpaces[i,j] && allDots[i,j] == null)
                {
                    //Loop from the space above to the top of the column
                    for(int k = j + 1; k < height; k++)
                    {
                        //If a dot is found . . .
                        if(allDots[i, k] != null)
                        {
                            //Move that dot to this empty space
                            allDots[i, k].GetComponent<Dot>().row = j;
                            //Set that spot to be Null
                            allDots[i, k] = null;
                            //Break out of the loop;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(RefillDelay);
    }
    
    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null && !blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotsToUse = Random.Range(0, dots.Length);
                    int maxInterations = 0;
                    
                    while(MatchesAt(i, j, dots[dotsToUse]) && maxInterations < 100)
                    {
                        maxInterations++;
                        dotsToUse = Random.Range(0, dots.Length);
                    }

                    var piece = Instantiate(dots[dotsToUse], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                    piece.transform.parent = transform;
                    piece.name = "(" + i + ", " + j + " ) - " + piece.tag;
                }
            }
        }
    }

    private bool MatchesInBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var dot = allDots[i, j];
                if (dot != null && dot.GetComponent<Dot>().isMatched)
                    return true;
            }
        }
        return false;
    }

    private bool CheckPiecesNull()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                var dot = allDots[i, j];
                if (dot == null && !blankSpaces[i,j])
                    return true;
            }
        }
        return false;
    }
    
    private IEnumerator FillBoardCo()
    {
        yield return DecreaseRowCo();
        RefillBoard();
        yield return new WaitForSeconds(RefillDelay);
        findMatches.currentMatches.Clear();
        findMatches.FindAllMatches();
        do
        {
            streakValue ++;
            if (findMatches.currentMatches.Count > 0)
            {
                DestroyMatches();
                yield return DecreaseRowCo();
                RefillBoard();
                yield return new WaitForSeconds(RefillDelay);
                findMatches.currentMatches.Clear();
                findMatches.FindAllMatches();
            }
           
        } while (MatchesInBoard());
        findMatches.currentMatches.Clear();
        currentDot = null;
        
        if (IsDeadlocked())
        {
            ShuffleBoard();
        }
        yield return new WaitForSeconds(RefillDelay);
        currentState = Gamestate.Move;
        streakValue = 1;
    }

    private void SwitchPieces(int column, int row, Vector2 direction)
    {

        //Take the second piece and save it in a holder
        GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
        //Switching the first dot to be the second position
        allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
        //Set the first dot to be the second dot.
        allDots[column, row] = holder;


    }
    
    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i ++)
        {
            for(int j = 0; j < height; j ++)
            {
                if(allDots[i,j]!= null)
                {
                    //Make sure that one and two to the right are in the board.
                    if(i < width - 2)
                    {
                        //Check if the dots do the right and two to the right exist.
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].CompareTag(allDots[i, j].tag) && allDots[i + 2, j].CompareTag(allDots[i, j].tag))
                            {
                                return true;
                            }
                        }
                    }
                    
                    if(j < height - 2)
                    {
                        //Check if the dots above exist.
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].CompareTag(allDots[i, j].tag) && allDots[i, j + 2].CompareTag(allDots[i, j].tag))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);
        if (CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }

        SwitchPieces(column, row, direction);
        return false;
    
    
    }

    private bool IsDeadlocked()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j ++)
            {
                if(allDots[i,j]!= null)
                {
                    if(i < width - 1)
                    {
                        if(SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }
                    if(j < height - 1)
                    {
                        if (SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }
        return true;
    }
    
    private void ShuffleBoard()
    {
        //Create a list of game objects
        var newBoard = new List<GameObject>();
        //Add every piece to this list.
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i,j]!= null)
                {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }
        //For every spot on the board . . .
        for (int i = 0; i < width; i ++)
        {
            for (int j = 0; j < height; j ++)
            {
                //If this spot shouldn't be blank
                if (blankSpaces[i, j]) continue;
                //Pick a random number
                int pieceToUse = Random.Range(0, newBoard.Count);
                    
                //Assign the column to the piece.
                int maxIterations = 0;
                    
                while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                {
                    pieceToUse = Random.Range(0, newBoard.Count);
                    maxIterations++;
                }
                //Make a container for the piece.
                Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                piece.column = i;
                //Assign the row to the piece.
                piece.row = j;
                //Fill in the dots array with this new piece.
                allDots[i, j] = newBoard[pieceToUse];
                //Remove it from the list.
                newBoard.Remove(newBoard[pieceToUse]);
            }
        }
        //Check if its still deadlocked.
        if (IsDeadlocked())
        {
            ShuffleBoard();
        }
    }
}
