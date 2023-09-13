using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int widht;
    public int height;
    public GameObject tilePrefab;
    private BackGround[,] allTiles;

    void Start()
    {
        allTiles = new BackGround[widht, height];
        Setup();
    }
    private void Setup()
    {
        for (int i = 0; i < widht; i++) //x
        {
            for (int j = 0; j < height; j++) //y
            {
                Vector2 tempPosition = new Vector2(i, j);
                GameObject background = Instantiate(tilePrefab, tempPosition, Quaternion.identity);
            }
        }
    }


}
