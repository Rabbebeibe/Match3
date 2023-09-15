using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [Header ("Board Variables")]
    public int column;
    public int row;
    public int preColumn;
    public int preRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private HintManager hintManager;
    private FindMatches findMatches;
    public GameObject otherPiece;
    private Board board;
    private Vector2 firstTouchPos;
    private Vector2 finalTouchPos;
    private Vector2 tempPos;

    [Header("Swipe")]
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    [Header("PowerUps")]
    public bool isColorBomb;
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isAreaBomb;
    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colorBomb;
    public GameObject areaBomb;
    void Start()
    {
        isColumnBomb = false;
        isRowBomb = false;
        isColorBomb = false;
        isAreaBomb = false;

        board = FindObjectOfType<Board>();
        findMatches = FindObjectOfType<FindMatches>();
        hintManager = FindObjectOfType<HintManager>();
        //targetX = (int)transform.position.x;
        //targetY = (int)transform.position.y;
        //row = targetY;
        //column = targetX;
        //preColumn = column;
        //preRow = row;
    }

    private void OnMouseOver() //debug test
    {
        if(Input.GetMouseButtonDown(1))
        {
            isAreaBomb = true;
            GameObject marker = Instantiate(areaBomb, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
        }
    }
    void Update()
    {
        //FindMatches();
        //if(isMatched)
        //{
        //    SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        //    mySprite.color = new Color(0f, 0f, 0f);
        //}
        targetX = column;
        targetY = row;
        if(Mathf.Abs(targetX - transform.position.x) > .1) //move towards target
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPos, .6f);
            if(board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPos = new Vector2(targetX, transform.position.y);
            transform.position = tempPos;
            board.allDots[column, row] = this.gameObject;
        }
        if (Mathf.Abs(targetY - transform.position.y) > .1) //move towards target
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPos, .6f);
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {
            tempPos = new Vector2(transform.position.x, targetY);
            transform.position = tempPos;
        }
    }

    public IEnumerator CheckMoveCo()
    {
        if(isColorBomb) //this piece is a color bomb, and the other piece is the color to destroy
        {
            findMatches.MatchPiecesOfColor(otherPiece.tag);
            isMatched = true;
        }
        else if(otherPiece.GetComponent<Piece>().isColorBomb) //the other piece is a color bomb, and this piece hhas the color to destroy
        {
            findMatches.MatchPiecesOfColor(this.gameObject.tag);
            otherPiece.GetComponent<Piece>().isMatched = true;
        }
        yield return new WaitForSeconds(.5f);
        if(otherPiece != null)
        {
            if(!isMatched && !otherPiece.GetComponent<Piece>().isMatched)
            {
                otherPiece.GetComponent<Piece>().row = row;
                otherPiece.GetComponent<Piece>().column = column;
                row = preRow;
                column = preColumn;
                yield return new WaitForSeconds(.5f);
                board.currentPiece = null;
                board.currentState = GameState.move;
            }
            else
            {
                board.DestroyMatches();
            }
           // otherPiece = null;
        }
    }

    private void OnMouseDown()
    {
        if(hintManager != null)
        {
            hintManager.DestroyHint();
        }
        if(board.currentState == GameState.move)
        {
            firstTouchPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }
    }
    private void OnMouseUp()
    {
        if(board.currentState == GameState.move)
        {
            finalTouchPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            CalculateAngle();
            board.currentState = GameState.wait;
            board.currentPiece = this;
        }
        else
        {
            board.currentState = GameState.move;
        }
    }
    void CalculateAngle()
    {
        //if(Mathf.Abs(finalTouchPos.y - firstTouchPos.y) > swipeResist ||  Mathf.Abs(finalTouchPos.x - firstTouchPos.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPos.y - firstTouchPos.y, finalTouchPos.x - firstTouchPos.x) * 180 / Mathf.PI;
            MovePiece();
        }
    }

    void MovePiece()
    {
        if (swipeAngle != 0) //clicking doesnt work
        {
            if (swipeAngle > -45 && swipeAngle <= 45 && column < board.widht - 1) //right
            {
                otherPiece = board.allDots[column + 1, row];
                preColumn = column;
                preRow = row;
                otherPiece.GetComponent<Piece>().column -= 1;
                column += 1;
            }
            else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1) //up
            {
                otherPiece = board.allDots[column, row + 1];
                preColumn = column;
                preRow = row;
                otherPiece.GetComponent<Piece>().row -= 1;
                row += 1;
            }
            else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0) //left
            {
                otherPiece = board.allDots[column - 1, row];
                preColumn = column;
                preRow = row;
                otherPiece.GetComponent<Piece>().column += 1;
                column -= 1;
            }
            else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0) //down
            {
                otherPiece = board.allDots[column, row - 1];
                preColumn = column;
                preRow = row;
                otherPiece.GetComponent<Piece>().row += 1;
                row -= 1;
            }
        }
        StartCoroutine(CheckMoveCo());
    }
    void FindMatches()
    {
        if (column > 0 && column < board.widht - 1)
        {
            GameObject leftPiece1 = board.allDots[column - 1, row];
            GameObject rightPiece1 = board.allDots[column + 1, row];
            if (leftPiece1 != null && rightPiece1 != null)
            {
                if (this.gameObject.CompareTag(leftPiece1.tag) && this.gameObject.CompareTag(rightPiece1.tag))
                {
                    leftPiece1.GetComponent<Piece>().isMatched = true;
                    rightPiece1.GetComponent<Piece>().isMatched = true;
                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upPiece1 = board.allDots[column, row + 1];
            GameObject downPiece1 = board.allDots[column, row - 1];
            if (upPiece1 != null && downPiece1 != null)
            {
                if (this.gameObject.CompareTag(upPiece1.tag) && this.gameObject.CompareTag(downPiece1.tag))
                {
                    upPiece1.GetComponent<Piece>().isMatched = true;
                    downPiece1.GetComponent<Piece>().isMatched = true;
                    isMatched = true;
                }
            }
        }
    }

    public void MakeRowBomb()
    {
        isRowBomb = true;
        GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }

    public void MakeColumnBomb()
    {
        isColumnBomb = true;
        GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
        arrow.transform.parent = this.transform;
    }
    public void MakeColorBomb()
    {
        isColorBomb = true;
        GameObject color = Instantiate(colorBomb, transform.position, Quaternion.identity);
        color.transform.parent = this.transform;
    }
    public void MakeAreaBomb()
    {
        isAreaBomb = true;
        GameObject area = Instantiate(areaBomb, transform.position, Quaternion.identity);
        area.transform.parent = this.transform;
    }
}
