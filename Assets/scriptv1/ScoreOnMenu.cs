using UnityEngine;
using TMPro;
using System.IO;

public class ScoreOnMenu : MonoBehaviour
{
    public string levelName; 
    public TMP_Text lastScoreText;
    public TMP_Text bestScoreText;
    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "save.json");
    }

    void Update()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameData data = JsonUtility.FromJson<GameData>(json);

            string check = levelName.ToLower().Trim();

            if (check.Contains("level 1") || check == "level1") 
            {
                Afficher(data.level1_Last, data.level1_Best);
            }
            else if (check.Contains("level 2") || check == "level2") 
            {
                Afficher(data.level2_Last, data.level2_Best);
            }
            else if (check.Contains("level 3") || check == "level3") 
            {
                Afficher(data.level3_Last, data.level3_Best);
            }
        }
    }

    void Afficher(int last, int best)
    {
        if (lastScoreText != null) lastScoreText.text = "Dernier : " + last;
        if (bestScoreText != null) bestScoreText.text = "Record : " + best;
    }
}