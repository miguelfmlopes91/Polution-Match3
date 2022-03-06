using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched;

    private EndGameManager endGameManager;
    private HintManager hintManager;
    private FindMatches findMatches;
    private Board board;
    public GameObject otherDot;
    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;
    
    
    [Header ("Swipe Stuff")]
    public float swipeAngle;
    public float swipeResist = 1f;
    private const float SwapAnimationTime = .6f;


    [Header("Power up Stuff")]
    public bool isColorBomb;
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isAdjacentBomb;
    public GameObject adjacentMarker;
    public GameObject RowArrow;
    public GameObject ColumnArrow;
    public GameObject ColorBomb;
    

    private void Start()
    {
        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAdjacentBomb = false;
        
        endGameManager = FindObjectOfType<EndGameManager>();
        hintManager = FindObjectOfType<HintManager>();
        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
    }
    
    private void Update()
    {
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1)
        {
            //move towards the target
            var position = transform.position;
            tempPosition = new Vector2(targetX, position.y);
            position = Vector2.Lerp(position, tempPosition, SwapAnimationTime);
            transform.position = position;
            if(board.allDots != null && board.allDots[column, row] != gameObject)
            {
                board.allDots[column, row] = gameObject;
            }
            board.currentState = Gamestate.Swapping;
        } 
        else
        {
            //directly move towards the target
            var transform1 = transform;
            tempPosition = new Vector2(targetX, transform1.position.y);
            transform1.position = tempPosition;
            
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            var position = transform.position;
            tempPosition = new Vector2(position.x, targetY);
            position = Vector2.Lerp(position, tempPosition, SwapAnimationTime);
            transform.position = position;
            if (board.allDots != null && board.allDots[column, row] != gameObject)
            {
                board.allDots[column, row] = gameObject;
            }
            board.currentState = Gamestate.Swapping;
        }
        else
        {
            var transform1 = transform;
            tempPosition = new Vector2(transform1.position.x, targetY);
            transform1.position = tempPosition;
        }
    }

    private IEnumerator CheckMoveCo()
    {
        if (board.currentState == Gamestate.Swapping)
        {
            yield return new WaitForSeconds(SwapAnimationTime);
        }
        if (isColorBomb)
        {
            //This Piece is a color bomb, and the other is the color to destroy
            findMatches.MatchPiecesOfColor(otherDot.tag);
            isMatched = true;
        }else if (otherDot.GetComponent<Dot>().isColorBomb)
        {
            //The other piece is a color bomb, and this peice has the color to destroy
            findMatches.MatchPiecesOfColor(gameObject.tag);
            otherDot.GetComponent<Dot>().isMatched = true;
        }

        board.currentState = Gamestate.CheckMatches;

        yield return new WaitUntil(() => board.currentState != Gamestate.CheckMatches);
        
        if (otherDot == null) yield break;
        if(!isMatched && !otherDot.GetComponent<Dot>().isMatched)
        {
            otherDot.GetComponent<Dot>().row = row;
            otherDot.GetComponent<Dot>().column = column;
            row = previousRow;
            column = previousColumn;
            yield return new WaitForSeconds(.5f);
            board.currentDot = null;
            board.currentState = Gamestate.Move;
        }
        else
        {
            if(endGameManager != null && endGameManager.requirements.gameType == GameType.Moves)
                endGameManager.DecreaseCounterValue();
        }
    }
    
    private void OnMouseDown()
    {
        if (board.currentState != Gamestate.Move) return;
        //Destroy the hint.
        if(hintManager != null)
        {
            hintManager.DestoryHint();
        }
        if(board.currentState == Gamestate.Move)
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }  
    }

    private void OnMouseUp()
    {
        if (board.currentState != Gamestate.Move) return;
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }
    
    private void CalculateAngle()
    {
        if(Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            board.currentState = Gamestate.Swapping;
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentDot = this;
        }
        else 
        {
            board.currentState = Gamestate.Move;
        }
    }

    private void MovePiecesActual(Vector2 direction)
    {
        otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column;
        if(otherDot != null)
        {
            otherDot.GetComponent<Dot>().column += -1 * (int)direction.x;
            otherDot.GetComponent<Dot>().row += -1 * (int)direction.y;
            column += (int)direction.x;
            row += (int)direction.y;
            StartCoroutine(CheckMoveCo());
        }
        else
        {
            board.currentState = Gamestate.Move;
        }
    }

    private void MovePieces()
    {
        if (swipeAngle > -45 && swipeAngle <= 45 && column < board.width-1)
        {
            MovePiecesActual(Vector2.right);
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height-1)
        {
            MovePiecesActual(Vector2.up);
        }
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            MovePiecesActual(Vector2.left);
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            MovePiecesActual(Vector2.down);
        }
        else
        {
            board.currentState = Gamestate.Move;
        }
    }
    
    public void MakeRowBomb()
    {
        isRowBomb = true;
        GameObject arrow = Instantiate(RowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void MakeColumnBomb()
    {
        isColumnBomb = true;
        GameObject arrow = Instantiate(ColumnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void MakeColorBomb()
    {
        isColorBomb = true;
        GameObject color = Instantiate(ColorBomb, transform.position, Quaternion.identity);
        color.transform.parent = this.transform;
        this.gameObject.tag = "Color";

    }

    public void MakeAdjacentBomb()
    {
        isAdjacentBomb = true;
        GameObject marker = Instantiate(adjacentMarker, transform.position, Quaternion.identity);
        marker.transform.parent = this.transform;
    }

}