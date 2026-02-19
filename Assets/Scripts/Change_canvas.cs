using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Change_canvas : MonoBehaviour
{
    [Header("Panneaux de Menu (Scène MainMenu)")]
    public GameObject menuPrincipal;
    public GameObject creditsCanvas;
    public GameObject settingsCanvas;
    public GameObject gameSelectCanvas;

    [Header("Panneaux de Niveaux (Scène de Jeu)")]
    public GameObject jeuCanvas;
    public GameObject level2Canvas;
    public GameObject level3Canvas;
    public GameObject LvlCalibrage;
    
    public GameObject CanvasKey;
    [Header("Fade")]
    [SerializeField] private CanvasGroup fader;
    [SerializeField] private float fadeDuration = 1f;
    private bool isTransitioning = false;
    private static bool pendingFadeIn = false;

    private static string canvasAActiver = "";
    public static bool IsCalibrationActive { get; private set;  }

    void Start()
    {
        if (!string.IsNullOrEmpty(canvasAActiver))
        {
            AppliquerAffichage(canvasAActiver);
            canvasAActiver = "";
        }

        if (fader != null)
        {
            if (pendingFadeIn)
            {
                fader.alpha = 1f;
                fader.blocksRaycasts = true;
                pendingFadeIn = false;
                StartCoroutine(Fade(1f, 0f, fadeDuration));
            }
            else
            {
                fader.alpha = 0f;
                fader.blocksRaycasts = false;
            }
        }
    }

    public void ChargerSceneEtCanvas(string instruction)
    {
        PlayerPrefs.Save(); 

        if (instruction.Contains(":"))
        {
            string[] parts = instruction.Split(':');
            string nomScene = parts[0].Trim();
            string canvas = parts[1].Trim();
            StartCoroutine(FadeLoadScene(nomScene, canvas));
        }
        else
        {
            StartCoroutine(FadeLoadScene(instruction.Trim(), ""));
        }
    }

    public void AfficherMenu()       => AppliquerAffichage("Menu");
    public void AfficherCredits()    => AppliquerAffichage("Credits");
    public void AfficherSettings()   => AppliquerAffichage("Settings");
    public void AfficherGameSelect() => AppliquerAffichage("Canvas Game_select");
    public void AfficherLevel1()     => StartCoroutine(FadeSwitch("Level 1"));
    public void AfficherLevel2()     => StartCoroutine(FadeSwitch("Level 2"));
    public void AfficherLevel3()     => StartCoroutine(FadeSwitch("Level 3"));
    public void AfficherCalibrage()  => StartCoroutine(FadeSwitch("Calibrage"));

    private IEnumerator FadeSwitch(string canvasName)
    {
        if (isTransitioning) yield break;
        isTransitioning = true;
        yield return Fade(0f, 1f, fadeDuration);
        AppliquerAffichage(canvasName);
        yield return null;
        yield return Fade(1f, 0f, fadeDuration);
        isTransitioning = false;
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        if (fader == null) yield break;
        fader.blocksRaycasts = true;
        fader.alpha = from;

        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            fader.alpha = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        fader.alpha = to;
        fader.blocksRaycasts = (to > 0.001f);
    }

    private IEnumerator FadeLoadScene(string sceneName, string canvasName)
    {
        if (isTransitioning) yield break;
        isTransitioning = true;
        if (fader != null) yield return Fade(0f, 1f, fadeDuration);
        canvasAActiver = canvasName;
        pendingFadeIn = true;
        SceneManager.LoadScene(sceneName);
    }

    private void AppliquerAffichage(string nom)
    {
        if(menuPrincipal)    menuPrincipal.SetActive(false);
        if(creditsCanvas)    creditsCanvas.SetActive(false);
        if(settingsCanvas)   settingsCanvas.SetActive(false);
        if(gameSelectCanvas) gameSelectCanvas.SetActive(false);
        if(jeuCanvas)        jeuCanvas.SetActive(false); 
        if(level2Canvas)     level2Canvas.SetActive(false);
        if(level3Canvas)     level3Canvas.SetActive(false);
        if(LvlCalibrage)     LvlCalibrage.SetActive(false);
        if (CanvasKey) CanvasKey.SetActive(true);

        IsCalibrationActive = false;

        switch (nom)
        {
            case "Canvas Game_select":
            case "game_select":
                if (gameSelectCanvas) gameSelectCanvas.SetActive(true);
                break;

            case "Menu":
                if (menuPrincipal) menuPrincipal.SetActive(true);
                break;

            case "Credits":
                if (creditsCanvas) creditsCanvas.SetActive(true);
                break;

            case "Settings":
                if (settingsCanvas) settingsCanvas.SetActive(true);
                break;

            case "Level 1":
            case "Jeu":
                loadQueue();
                if (jeuCanvas) jeuCanvas.SetActive(true);
                break;

            case "Level 2":
                loadQueue();
                if (level2Canvas) level2Canvas.SetActive(true);
                break;

            case "Level 3":
                loadQueue();
                if (level3Canvas) level3Canvas.SetActive(true);
                break;
            
            case "Calibrage":
            case "LvlCalibrage":
            case "Calibration":
                if (LvlCalibrage) LvlCalibrage.SetActive(true);
                IsCalibrationActive = true;
                break;
                
            default:
                Debug.LogWarning("Le nom '" + nom + "' n'est pas reconnu dans le switch.");
                break;
        }
    }

    private void loadQueue()
    {
        KeyManager km = KeyManager.Instance;
        if (km != null)
        {
            Debug.Log("Chargement des queues dans KeyManager...");
            double[] leftNotes = { 7.0, 10.0, 12.0, 14.0, 16.0, 19.0, 22.0, 25.0, 27.0, 30.0, 33.0, 36.0, 39.0 };
            double[] upNotes = { 8.0, 9.0, 11.0, 13.0, 15.0, 18.0, 21.0, 23.0, 26.0, 29.0, 31.0, 34.0 };
            double[] downNotes = { 9.5, 10.5, 12.5, 14.5, 17.0, 18.0, 20.0, 22.0, 24.0, 26.5, 28.0, 30.0 };
            double[] rightNotes = { 11.0, 13.0, 16.0, 17.0, 20.0, 23.0, 24.0, 27.0, 29.0, 32.0, 36.0, 38.0 };

            km.EnqueueNotes(leftNotes, km.setLeftQueue);
            km.EnqueueNotes(upNotes, km.setUpQueue);
            km.EnqueueNotes(downNotes, km.setDownQueue);
            km.EnqueueNotes(rightNotes, km.setRightQueue);
        }
        else
        {
            Debug.LogError("KeyManager instance is null. Cannot load queues.");
        }
    }
}