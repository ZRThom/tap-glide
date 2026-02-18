using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class RebindButton : MonoBehaviour
{
    public CustomSettings settings;
    public string direction; 
    public TextMeshProUGUI buttonText;

    private bool isListening = false;

    void Start() { UpdateUI(); }

    public void StartRebinding() {
        isListening = true;
        buttonText.text = "...";
    }

    void Update() {
        if (!isListening) return;

        if (Keyboard.current.anyKey.wasPressedThisFrame) {
            foreach (var key in Keyboard.current.allKeys) {
                if (key.wasPressedThisFrame) {
                    ApplyKey(key.name.ToLower());
                    break;
                }
            }
        }
    }

    void ApplyKey(string keyName) {
    if (direction == "left") settings.keyLeft = keyName;
    else if (direction == "up") settings.keyUp = keyName;
    else if (direction == "down") settings.keyDown = keyName;
    else if (direction == "right") settings.keyRight = keyName;

    // IMPORTANT : On sauvegarde physiquement dans le JSON
    settings.SaveSettings();

    isListening = false;
    UpdateUI();
    
    Debug.Log("Touche " + direction + " changée pour : " + keyName);
}

    void UpdateUI() {
        string k = (direction == "left") ? settings.keyLeft : (direction == "up") ? settings.keyUp : (direction == "down") ? settings.keyDown : settings.keyRight;
        buttonText.text = k.ToUpper();
    }
}