using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private Board board;
    public TextMeshProUGUI scoreText;
    public int score;
    public Image scoreBar;

    private void Start()
    {
        board = FindObjectOfType<Board>();
    }
    private void Update()
    {
        scoreText.text = score.ToString();
    }
    public void IncreaseScore( int amountToIncrease)
    {
        score += amountToIncrease;
        if(board != null && scoreBar != null)
        {
            int lenght = board.scoreGoals.Length;
            scoreBar.fillAmount = (float)score / (float)board.scoreGoals[lenght -1];
        }
    }
}
