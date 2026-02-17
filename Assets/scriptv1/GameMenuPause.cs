using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameMenuPause : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingsGame;
    public GameObject quitGame;

    void Start()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (settingsGame != null) settingsGame.SetActive(false);
        if (quitGame != null) quitGame.SetActive(false);
    }

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return;
        if (kb.escapeKey.wasPressedThisFrame)
        {
            if (settingsGame != null && settingsGame.activeSelf)
            {
                settingsGame.SetActive(false);
                return;
            }

            if (quitGame != null && quitGame.activeSelf)
            {
                quitGame.SetActive(false);
                return;
            }

            if (pauseMenu != null) pauseMenu.SetActive(!pauseMenu.activeSelf);
        }
    }

    public void PauseMenuOff()
    {
        if (pauseMenu != null) pauseMenu.SetActive(false);
        if (settingsGame != null) settingsGame.SetActive(false);
        if (quitGame != null) quitGame.SetActive(false);
    }

    public void GameSettings()
    {
        if (settingsGame != null) settingsGame.SetActive(true);
    }

    public void GameSettingsOff()
    {
        if (settingsGame != null) settingsGame.SetActive(false);
    }

    public void GameQuit()
    {
        if (quitGame != null) quitGame.SetActive(true);
    }

    public void GameQuitOff()
    {
        if (quitGame != null) quitGame.SetActive(false);
    }

    public void GameQuitCheck()
    {
        if (quitGame != null && quitGame.activeSelf)
        {
            SceneManager.LoadScene("test");
        }
    }
}
