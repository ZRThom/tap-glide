using UnityEngine;
using System.Collections.Generic;
using System.IO;

[CreateAssetMenu(fileName = "CustomSettings", menuName = "ScriptableObjects/CustomSettings")]
public class CustomSettings : ScriptableObject
{
    public List<string> devs; 
    public string keyLeft = "d";
    public string keyUp = "f";
    public string keyDown = "j";
    public string keyRight = "k";

    private string SavePath => Application.persistentDataPath + "/controls.json";

    public static CustomSettings Load()
    {
        return Resources.Load<CustomSettings>("CustomSettings");
    }

#if UNITY_EDITOR
    public static UnityEditor.SerializedObject SerializedSettings()
    {
        return new UnityEditor.SerializedObject(Load());
    }
#endif

    public void SaveSettings() {
        SaveData data = new SaveData {
            left = keyLeft, up = keyUp, down = keyDown, right = keyRight
        };
        File.WriteAllText(SavePath, JsonUtility.ToJson(data));
    }

    public void LoadSettings() {
        if (File.Exists(SavePath)) {
            string json = File.ReadAllText(SavePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            keyLeft = data.left; keyUp = data.up; keyDown = data.down; keyRight = data.right;
        }
    }

    [System.Serializable]
    private class SaveData { public string left, up, down, right; }
}