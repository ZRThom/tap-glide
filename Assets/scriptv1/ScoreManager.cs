using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public int score;   
    void Start()
    {
        Refresh();
    }

    public void AddPoints(int points)
    {
        score += points;
        Refresh();
    }

    void Refresh()
    {
        if (scoreText != null) scoreText.text = score.ToString();
    }

}
