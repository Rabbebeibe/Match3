using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamerScaler : MonoBehaviour
{
    private Board board;
    public float cameraOffset;
    public float aspectRatio = 0.625f;
    public float padding = 2;
    void Start()
    {
        board = FindAnyObjectByType<Board>();
        if(board != null)
        {
            RePosCam(board.widht - 1, board.height - 1);
        }
    }
    void RePosCam(float x, float y)
    {
        Vector3 tempPos = new Vector3(x/2 , y/2 , cameraOffset);
        transform.position = tempPos;
        if(board.widht >= board.height)
        {
            Camera.main.orthographicSize = (board.widht / 2 + padding) / aspectRatio;
        }
        else
        {
            Camera.main.orthographicSize = board.height / 2 + padding;
        }
    }
}
