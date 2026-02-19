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
                loadQueue(1);
                if (jeuCanvas) jeuCanvas.SetActive(true);
                break;

            case "Level 2":
                loadQueue(2);
                if (level2Canvas) level2Canvas.SetActive(true);
                break;

            case "Level 3":
                loadQueue(3);
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

    private void loadQueue(int level)
    {
        KeyManager km = KeyManager.Instance;
        if (km != null)
        {
            double[] leftNotes = { };
            double[] upNotes = { };
            double[] downNotes = { };
            double[] rightNotes = { };
            switch (level)
            {
                case 1:
                    km.ClearQueues();
                    leftNotes = new double[] { 7.0, 10.0, 12.0, 14.0, 16.0, 19.0, 22.0, 25.0, 27.0, 30.0, 33.0, 36.0, 39.0 };
                    upNotes = new double[] { 8.0, 9.0, 11.0, 13.0, 15.0, 18.0, 21.0, 23.0, 26.0, 29.0, 31.0, 34.0 };
                    downNotes = new double[] { 9.0, 11.0, 13.0, 14.0, 17.0, 18.0, 20.0, 22.0, 24.0, 27.0, 28.0, 30.0 };
                    rightNotes = new double[] { 11.0, 13.0, 16.0, 17.0, 20.0, 23.0, 24.0, 27.0, 29.0, 32.0, 36.0, 38.0 };
                    break;
                case 2:
                    km.ClearQueues();
                    leftNotes = new double[] {
                        7.0, 8.0, 10.0, 11.0, 13.0, 15.0, 16.0, 17.0, 19.0, 21.0, 23.0, 24.0, 25.0, 26.0, 28.0, 30.0,
                        31.0, 32.0, 33.0, 35.0, 36.0, 38.0, 39.0, 40.0, 42.0, 44.0, 45.0, 46.0, 47.0, 48.0, 49.0, 50.0,
                        52.0, 54.0, 56.0, 58.0, 60.0, 61.0, 63.0, 65.0, 67.0, 68.0, 70.0, 71.0, 73.0, 75.0, 77.0, 79.0,
                        81.0, 82.0, 83.0, 84.0, 86.0, 88.0, 89.0, 91.0, 93.0, 95.0, 96.0, 98.0, 100.0, 102.0, 103.0, 104.0,
                        105.0, 107.0, 108.0, 110.0, 111.0, 113.0, 114.0, 116.0, 118.0, 119.0, 121.0, 122.0, 123.0, 125.0, 127.0, 129.0
                    };

                    upNotes = new double[] {
                        8.0, 9.0, 11.0, 12.0, 14.0, 16.0, 18.0, 19.0, 21.0, 23.0, 25.0, 26.0, 27.0, 29.0, 30.0, 31.0,
                        33.0, 34.0, 35.0, 36.0, 38.0, 40.0, 41.0, 43.0, 45.0, 47.0, 48.0, 50.0, 51.0, 52.0, 54.0, 56.0,
                        57.0, 58.0, 60.0, 61.0, 63.0, 65.0, 67.0, 69.0, 71.0, 72.0, 73.0, 75.0, 76.0, 78.0, 79.0, 81.0,
                        82.0, 84.0, 85.0, 87.0, 88.0, 89.0, 91.0, 93.0, 95.0, 96.0, 97.0, 98.0, 99.0, 100.0, 101.0, 103.0,
                        104.0, 105.0, 107.0, 108.0, 110.0, 111.0, 112.0, 114.0, 116.0, 118.0, 119.0, 121.0, 123.0, 125.0, 127.0, 128.0
                    };

                    downNotes = new double[] {
                        10.0, 12.0, 14.0, 15.0, 17.0, 19.0, 20.0, 22.0, 23.0, 25.0, 27.0, 28.0, 30.0, 31.0, 33.0, 35.0,
                        36.0, 38.0, 40.0, 41.0, 43.0, 45.0, 47.0, 49.0, 50.0, 52.0, 54.0, 56.0, 57.0, 58.0, 59.0, 60.0,
                        62.0, 64.0, 65.0, 67.0, 68.0, 70.0, 71.0, 73.0, 75.0, 76.0, 77.0, 79.0, 81.0, 82.0, 84.0, 86.0,
                        87.0, 88.0, 90.0, 92.0, 93.0, 95.0, 97.0, 99.0, 100.0, 101.0, 103.0, 104.0, 105.0, 106.0, 108.0, 109.0,
                        110.0, 112.0, 114.0, 116.0, 117.0, 119.0, 120.0, 121.0, 123.0, 125.0, 126.0, 127.0, 129.0, 131.0, 133.0, 134.0
                    };

                    rightNotes = new double[] {
                        11.0, 12.0, 13.0, 15.0, 17.0, 18.0, 20.0, 21.0, 23.0, 25.0, 26.0, 27.0, 29.0, 30.0, 32.0, 33.0,
                        35.0, 36.0, 38.0, 40.0, 41.0, 43.0, 44.0, 46.0, 47.0, 49.0, 50.0, 52.0, 53.0, 54.0, 55.0, 57.0,
                        59.0, 60.0, 62.0, 64.0, 66.0, 68.0, 69.0, 71.0, 72.0, 73.0, 74.0, 76.0, 77.0, 78.0, 80.0, 81.0,
                        83.0, 84.0, 86.0, 87.0, 88.0, 89.0, 90.0, 92.0, 93.0, 94.0, 95.0, 96.0, 98.0, 100.0, 101.0, 103.0,
                        104.0, 105.0, 106.0, 107.0, 109.0, 110.0, 111.0, 112.0, 114.0, 116.0, 117.0, 118.0, 119.0, 121.0, 123.0, 124.0
                    };

                    break;
                case 3:
                    km.ClearQueues();
                    // LEFT: The Anchor. Hits every single tick for 60 seconds.
                    leftNotes = new double[]
                    {
                    7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35,
                    36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64,
                    65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93
                    };

                    // UP: Fast Jackhammer. Hits every tick except multiples of 5 (for "rhythm").
                    upNotes = new double[]
                    {
                    7, 8, 9, 11, 12, 13, 14, 16, 17, 18, 19, 21, 22, 23, 24, 26, 27, 28, 29, 31, 32, 33, 34, 36, 37, 38, 39, 41, 42, 43, 44,
                    46, 47, 48, 49, 51, 52, 53, 54, 56, 57, 58, 59, 61, 62, 63, 64, 66, 67, 68, 69, 71, 72, 73, 74, 76, 77, 78, 79, 81, 82,
                    83, 84, 86, 87, 88, 89, 91, 92, 93, 94, 96, 97, 98, 99, 101, 102, 103, 104, 106, 107, 108, 109, 111, 112, 113, 114, 116
                    };

                    // DOWN: The Heavy Beat. Hits every even tick (2, 4, 6, 8...).
                    downNotes = new double[]
                    {
                    8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 46, 48, 50, 52, 54, 56, 58, 60, 62, 64, 66,
                    68, 70, 72, 74, 76, 78, 80, 82, 84, 86, 88, 90, 92, 94, 96, 98, 100, 102, 104, 106, 108, 110, 112, 114, 116, 118, 120,
                    122, 124, 126, 128
                    };

                    // RIGHT: The Chaos. Hits every odd tick (1, 3, 5, 7...) PLUS every 4th tick for Quads.
                    rightNotes = new double[]
                    {
                    7, 8, 9, 11, 12, 13, 15, 16, 17, 19, 20, 21, 23, 24, 25, 27, 28, 29, 31, 32, 33, 35, 36, 37, 39, 40, 41, 43, 44, 45, 47,
                    48, 49, 51, 52, 53, 55, 56, 57, 59, 60, 61, 63, 64, 65, 67, 68, 69, 71, 72, 73, 75, 76, 77, 79, 80, 81, 83, 84, 85, 87,
                    88, 89, 91, 92, 93, 95, 96, 97, 99, 100, 101, 103, 104, 105, 107, 108, 109, 111, 112, 113, 115, 116, 117, 119, 120, 121
                    };
                    break;
                default:
                    Debug.LogWarning("Level " + level + " n'est pas reconnu pour le chargement des queues.");
                    return;
            }
            Debug.Log("Chargement des queues dans KeyManager...");

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