using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;
    public int widht;
    public int height;
    public int offSet;
    public GameObject tilePrefab;
    private BackGround[,] allTiles;
    private FindMatches findMatches;
    public GameObject[] dots;
    public GameObject[,] allDots;
    public Piece currentPiece;

    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new BackGround[widht, height];
        allDots = new GameObject[widht, height];
        Setup();
    }
    private void Setup()
    {
        for (int i = 0; i < widht; i++) //x
        {
            for (int j = 0; j < height; j++) //y
            {
                Vector2 tempPosition = new Vector2(i, j + offSet);
                GameObject background = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                background.transform.parent = this.transform;
                background.name = "( " + i + ", " + j + " )";

                int dotToUse = Random.Range(0, dots.Length);
                int maxIterations = 0;
                while(MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                    Debug.Log(maxIterations);
                }
                maxIterations = 0;

                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.GetComponent<Piece>().row = j;
                dot.GetComponent<Piece>().column = i;
                dot.transform.parent = this.transform;
                dot.name = "( " + i + ", " + j + " )";
                allDots[i, j] = dot;
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject piece) //disallows starting matches
    {
        if(column > 1 && row > 1)
        {
            if(allDots[column -1, row].tag == piece.tag && allDots[column -2, row].tag == piece.tag)
            {
                return true;
            }
            if (allDots[column, row -1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if (allDots[column, row -1].tag == piece.tag && allDots[column, row -2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allDots[column -1, row].tag == piece.tag && allDots[column -2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }
    private void DestroyMatchesAt(int column, int row)
    {
        if (allDots[column, row].GetComponent<Piece>().isMatched)
        {
            if(findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
            {
                findMatches.CheckBombs();
            }
            findMatches.currentMatches.Remove(allDots[column, row]);
            Destroy(allDots[column, row]);
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
       for(int i = 0; i < widht; i ++)
        {
            for(int j = 0; j < height; j ++)
            {
                if(allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRowCo());
    }
    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        for (int i = 0; i < widht; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i, j] == null)
                {
                    nullCount++;
                }
                else if(nullCount > 0)
                {
                    allDots[i, j].GetComponent<Piece>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }
    private void RefillBoard() //refills the board after match
    {
        for (int i = 0; i < widht; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i,j] == null)
                {
                    Vector2 tempPos = new Vector2(i, j + offSet);
                    int pieceToUse = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[pieceToUse], tempPos, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Piece>().row = j;
                    piece.GetComponent<Piece>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int i = 0; i < widht; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if(allDots[i, j].GetComponent<Piece>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while(MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        currentPiece = null;
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;
    }
}
