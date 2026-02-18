using UnityEngine;
using TMPro;
using System.IO;

[System.Serializable]
public class GameData
{
    public int level1_Last;
    public int level1_Best;
    public int level2_Last;
    public int level2_Best;
    public int level3_Last;
    public int level3_Best;
}

public class ScoreManager : MonoBehaviour
{
    public TMP_Text scoreText;
    public int score;
    public string levelName; 
    private string filePath;
    private GameData data = new GameData();

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "save.json");
        ChargerDonnees();
        DetecterNiveau();
        score = 0;
        Refresh();
    }

    void DetecterNiveau()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.activeInHierarchy)
            {
                string n = obj.name;
                if (n.Contains("Level 1")) { levelName = "Level 1"; break; }
                if (n.Contains("Level 2")) { levelName = "Level 2"; break; }
                if (n.Contains("Level 3")) { levelName = "Level 3"; break; }
            }
        }

        if (string.IsNullOrEmpty(levelName)) levelName = "Inconnu";
    }

    public void AddPoints(int points)
    {
        score += points;
        SauvegarderDonnees();
        Refresh();
    }

    void SauvegarderDonnees()
    {
        ChargerDonnees();

        if (levelName == "Level 1") { 
            data.level1_Last = score; 
            if(score > data.level1_Best) data.level1_Best = score; 
        }
        else if (levelName == "Level 2") { 
            data.level2_Last = score; 
            if(score > data.level2_Best) data.level2_Best = score; 
        }
        else if (levelName == "Level 3") { 
            data.level3_Last = score; 
            if(score > data.level3_Best) data.level3_Best = score; 
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    void ChargerDonnees()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<GameData>(json);
        }
    }

    void Refresh()
    {
        if (scoreText != null) scoreText.text = score.ToString();
    }
}