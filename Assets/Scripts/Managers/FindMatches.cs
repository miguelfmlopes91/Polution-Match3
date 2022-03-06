using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class FindMatches : MonoBehaviour
{
    private Board board;
    public List<GameObject> currentMatches = new List<GameObject>();
    
    private void Start()
    {
        board = FindObjectOfType<Board>();
    }

    public void FindAllMatches()
    {
        StartCoroutine(FindAllMatchesCo());
    }
    
    
    private IEnumerable<GameObject> IsAdjacentBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        var currentDots = new List<GameObject>();
        if (dot1.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot1.column, dot1.row));
        }

        if (dot2.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot2.column, dot2.row));
        }

        if (dot3.isAdjacentBomb)
        {
            currentMatches.Union(GetAdjacentPieces(dot3.column, dot3.row));
        }
        return currentDots;
    }


    private void IsRowBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        if (dot1.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot1.row));
        }

        if (dot2.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot2.row));
        }

        if (dot3.isRowBomb)
        {
            currentMatches.Union(GetRowPieces(dot3.row));
        }
    }

    private void IsColumnBomb(Dot dot1, Dot dot2, Dot dot3)
    {
        if (dot1.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot1.column));
        }

        if (dot2.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot2.column));
        }

        if (dot3.isColumnBomb)
        {
            currentMatches.Union(GetColumnPieces(dot3.column));
        }
    }

    private void AddToListAndMatch(GameObject dot)
    {
        if (!currentMatches.Contains(dot))
        {
            currentMatches.Add(dot);
        }
        dot.GetComponent<Dot>().isMatched = true;
    }
    private void GetNearbyPieces(GameObject dot1, GameObject dot2, GameObject dot3)
    {
        AddToListAndMatch(dot1);
        AddToListAndMatch(dot2);
        AddToListAndMatch(dot3);
        
    }

    private IEnumerator FindAllMatchesCo()
    {
        yield return new WaitForSeconds(.2f);
        for (int i = 0; i < board.width; i++)
        {
            for(int j = 0; j < board.height; j++)
            {
                GameObject currentDot = board.allDots[i, j];

                if (currentDot == null) continue;
                Dot currentDotDot = currentDot.GetComponent<Dot>();
                if (i > 0 && i < board.width - 1)
                {
                    var leftDot = board.allDots[i - 1, j];
                    var rightDot = board.allDots[i + 1, j];
                    if(leftDot != null && rightDot != null)
                    {
                        Dot rightDotDot = rightDot.GetComponent<Dot>();
                        Dot leftDotDot = leftDot.GetComponent<Dot>();
                        if (leftDotDot != null && rightDotDot != null)
                        {
                            if (leftDot.CompareTag(currentDot.tag) && rightDot.CompareTag(currentDot.tag))
                            {
                                IsRowBomb(leftDotDot, currentDotDot, rightDotDot);
                                IsColumnBomb(leftDotDot, currentDotDot, rightDotDot);
                                IsAdjacentBomb(leftDotDot, currentDotDot, rightDotDot);
                                GetNearbyPieces(leftDot, currentDot, rightDot);
                            }
                        }
                    }
                }
                if (j > 0 && j < board.height - 1)
                {
                    var upDot = board.allDots[i, j + 1];
                    var downDot = board.allDots[i, j - 1];
                        
                    if (upDot == null || downDot == null) continue;
                    Dot downDotDot = downDot.GetComponent<Dot>();
                    Dot upDotDot = upDot.GetComponent<Dot>();
                        
                    if (downDotDot == null || upDotDot == null) continue;
                    if (upDot.CompareTag(currentDot.tag) && downDot.CompareTag(currentDot.tag))
                    {
                        IsColumnBomb(upDotDot, currentDotDot, downDotDot);
                        IsRowBomb(upDotDot, currentDotDot, downDotDot);
                        IsAdjacentBomb(upDotDot, currentDotDot, downDotDot);
                        GetNearbyPieces(upDot, currentDot, downDot);
                    };
                }
            }
        }
    }

    private IEnumerable<GameObject> GetAdjacentPieces(int column, int row)
    {
        var dots = new List<GameObject>();
        for(int i = column - 1; i <= column + 1; i++)
        {
            for(int j = row - 1; j <= row + 1; j++)
            {
                //Check if the piece is inside the board
                if(i >= 0 && i < board.width && j >= 0 && j < board.height)
                {
                    if(board.allDots[i,j]!= null)
                    {
                        dots.Add(board.allDots[i, j]);
                        board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                    }
                }
            }
        }
        return dots;
    }

    private IEnumerable<GameObject> GetColumnPieces(int column)
    {
        var dots = new List<GameObject>();
        for(int i = 0; i < board.height; i ++)
        {
            if(board.allDots[column, i] != null)
            {
                Dot dot = board.allDots[column, i].GetComponent<Dot>();
                if (dot.isRowBomb)
                {
                    dots.Union(GetRowPieces(i)).ToList();
                }

                dots.Add(board.allDots[column, i]);
                dot.isMatched = true;
            }
        }
        return dots;
    }
    
    private IEnumerable<GameObject> GetRowPieces(int row)
    {
        List<GameObject> dots = new List<GameObject>();
        for (int i = 0; i < board.width; i++)
        {
            if (board.allDots[i, row] != null)
            {
                Dot dot = board.allDots[i, row].GetComponent<Dot>();
                if (dot.isColumnBomb)
                {
                    dots.Union(GetColumnPieces(i)).ToList();
                }

                dots.Add(board.allDots[i, row]);
                dot.isMatched = true;
            }
        }
        return dots;
    }

    public void CheckBombs()
    {
        //Did the player move something?
        if (board.currentDot == null) return;
        //Is the piece they moved matched?
        if (board.currentDot.isMatched)
        {
            //Make it unmatched
            board.currentDot.isMatched = false;
            if((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) ||(board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
            {
                board.currentDot.MakeRowBomb();
            }
            else
            {
                board.currentDot.MakeColumnBomb();
            }

        }
        //Is the other piece matched?
        else if (board.currentDot.otherDot != null) 
        {
            Dot otherDot = board.currentDot.otherDot.GetComponent<Dot>();
            //Is the other Dot Matched?
            if (!otherDot.isMatched) return;
            //make it unmatched
            otherDot.isMatched = false;
            if ((board.currentDot.swipeAngle > -45 && board.currentDot.swipeAngle <= 45) || (board.currentDot.swipeAngle < -135 || board.currentDot.swipeAngle >= 135))
            {
                otherDot.MakeRowBomb();
            }
            else
            {
                otherDot.MakeColumnBomb();
            }
        }
    }
    
    public void MatchPiecesOfColor(string color)
    {
        for(int i = 0; i < board.width; i ++)
        {
            for(int j = 0; j < board.height; j ++)
            {
                //Check if that Piece exists
                if (board.allDots[i, j] == null) continue;
                //Check the Tag on that Dot
                if(board.allDots[i, j].CompareTag(color))
                {
                    //set that dot to be matched
                    board.allDots[i, j].GetComponent<Dot>().isMatched = true;
                }
            }
        }
    }
}
