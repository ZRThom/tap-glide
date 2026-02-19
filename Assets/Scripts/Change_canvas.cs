using UnityEngine;
using UnityEngine.SceneManagement;

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

    private static string canvasAActiver = "";
    public static bool IsCalibrationActive { get; private set; }

    void Start()
    {
        if (!string.IsNullOrEmpty(canvasAActiver))
        {
            AppliquerAffichage(canvasAActiver);
            canvasAActiver = "";
        }
    }

    public void ChargerSceneEtCanvas(string instruction)
    {
        PlayerPrefs.Save();

        if (instruction.Contains(":"))
        {
            string[] parts = instruction.Split(':');
            string nomScene = parts[0].Trim();
            canvasAActiver = parts[1].Trim();
            SceneManager.LoadScene(nomScene);
        }
        else
        {
            SceneManager.LoadScene(instruction.Trim());
        }
    }

    public void AfficherMenu() => AppliquerAffichage("Menu");
    public void AfficherCredits() => AppliquerAffichage("Credits");
    public void AfficherSettings() => AppliquerAffichage("Settings");
    public void AfficherGameSelect() => AppliquerAffichage("Canvas Game_select");
    public void AfficherLevel1() => AppliquerAffichage("Level 1");
    public void AfficherLevel2() => AppliquerAffichage("Level 2");
    public void AfficherLevel3() => AppliquerAffichage("Level 3");
    public void AfficherCalibrage() => AppliquerAffichage("Calibrage");

    private void AppliquerAffichage(string nom)
    {
        if (menuPrincipal) menuPrincipal.SetActive(false);
        if (creditsCanvas) creditsCanvas.SetActive(false);
        if (settingsCanvas) settingsCanvas.SetActive(false);
        if (gameSelectCanvas) gameSelectCanvas.SetActive(false);
        if (jeuCanvas) jeuCanvas.SetActive(false);
        if (level2Canvas) level2Canvas.SetActive(false);
        if (level3Canvas) level3Canvas.SetActive(false);
        if (LvlCalibrage) LvlCalibrage.SetActive(false);

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
                    leftNotes = new double[] { 7.0, 8.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 18.0, 19.0, 21.0, 22.0, 24.0, 25.0, 26.0, 27.0, 29.0, 30.0, 33.0, 34.0, 36.0, 37.0, 39.0 };
                    upNotes = new double[] { 8.0, 9.0, 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 17.0, 18.0, 19.0, 21.0, 22.0, 23.0, 25.0, 26.0, 27.0, 29.0, 30.0, 31.0, 32.0, 34.0 };
                    downNotes = new double[] { 10.0, 11.0, 12.0, 13.0, 14.0, 15.0, 16.0, 17.0, 18.0, 19.0, 20.0, 21.0, 22.0, 23.0, 24.0, 25.0, 26.0, 27.0, 28.0, 29.0, 30.0 };
                    rightNotes = new double[] { 11.0, 12.0, 13.0, 15.0, 16.0, 17.0, 18.0, 19.0, 20.0, 22.0, 23.0, 24.0, 25.0, 26.0, 27.0, 28.0, 29.0, 31.0, 32.0, 34.0 };


                    break;
                case 3:
                    km.ClearQueues();
                    // LEFT: The Anchor. Hits every single tick for 60 seconds.
                    leftNotes = new double[]
                    {
                    7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, 36, 37, 38, 39, 40, 41, 42, 43, 44, 45, 46, 47, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 91, 92, 93, 94, 95, 96, 97, 98, 99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111, 112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127, 128, 129, 130, 131, 132, 133, 134, 135, 136, 137, 138, 139, 140, 141, 142, 143, 144, 145, 146, 147, 148, 149, 150, 151, 152, 153, 154, 155, 156, 157, 158, 159, 160, 161, 162, 163, 164, 165, 166, 167, 168, 169, 170, 171, 172, 173, 174, 175, 176, 177, 178, 179, 180, 181, 182, 183, 184, 185, 186, 187, 188, 189, 190, 191, 192, 193, 194, 195, 196, 197, 198, 199, 200, 201, 202, 203, 204, 205, 206, 207, 208, 209, 210, 211, 212, 213, 214, 215, 216, 217, 218, 219, 220, 221, 222, 223, 224, 225, 226, 227, 228, 229, 230, 231, 232, 233, 234, 235, 236, 237, 238, 239, 240, 241, 242, 243, 244, 245, 246, 247, 248, 249, 250, 251, 252, 253, 254, 255, 256, 257, 258, 259, 260
                    };

                    // UP: Fast Jackhammer. Hits every tick except multiples of 5 (for "rhythm").
                    upNotes = new double[]
                    {
                    7, 8, 9, 11, 12, 13, 14, 16, 17, 18, 19, 21, 22, 23, 24, 26, 27, 28, 29, 31, 32, 33, 34, 36, 37, 38, 39, 41, 42, 43, 44, 46, 47, 48, 49, 51, 52, 53, 54, 56, 57, 58, 59, 61, 62, 63, 64, 66, 67, 68, 69, 71, 72, 73, 74, 76, 77, 78, 79, 81, 82, 83, 84, 86, 87, 88, 89, 91, 92, 93, 94, 96, 97, 98, 99, 101, 102, 103, 104, 106, 107, 108, 109, 111, 112, 113, 114, 116, 117, 118, 119, 121, 122, 123, 124, 126, 127, 128, 129, 131, 132, 133, 134, 136, 137, 138, 139, 141, 142, 143, 144, 146, 147, 148, 149, 151, 152, 153, 154, 156, 157, 158, 159, 161, 162, 163, 164, 166, 167, 168, 169, 171, 172, 173, 174, 176, 177, 178, 179, 181, 182, 183, 184, 186, 187, 188, 189, 191, 192, 193, 194, 196, 197, 198, 199, 201, 202, 203, 204, 206, 207, 208, 209, 211, 212, 213, 214, 216, 217, 218, 219, 221, 222, 223, 224, 226, 227, 228, 229, 231, 232, 233, 234, 236, 237, 238, 239, 241, 242, 243, 244, 246, 247, 248, 249, 251, 252, 253, 254, 256, 257, 258, 259
                    };

                    // DOWN: The Heavy Beat. Hits every even tick (2, 4, 6, 8...).
                    downNotes = new double[]
                    {
                    8, 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32, 34, 36, 38, 40, 42, 44, 46, 48, 50, 52, 54, 56, 58, 60, 62, 64, 66, 68, 70, 72, 74, 76, 78, 80, 82, 84, 86, 88, 90, 92, 94, 96, 98, 100, 102, 104, 106, 108, 110, 112, 114, 116, 118, 120, 122, 124, 126, 128, 130, 132, 134, 136, 138, 140, 142, 144, 146, 148, 150, 152, 154, 156, 158, 160, 162, 164, 166, 168, 170, 172, 174, 176, 178, 180, 182, 184, 186, 188, 190, 192, 194, 196, 198, 200, 202, 204, 206, 208, 210, 212, 214, 216, 218, 220, 222, 224, 226, 228, 230, 232, 234, 236, 238, 240, 242, 244, 246, 248, 250, 252, 254, 256, 258, 260
                    };

                    // RIGHT: The Chaos. Hits every odd tick (1, 3, 5, 7...) PLUS every 4th tick for Quads.
                    rightNotes = new double[]
                    {
                    7, 8, 9, 11, 12, 13, 15, 16, 17, 19, 20, 21, 23, 24, 25, 27, 28, 29, 31, 32, 33, 35, 36, 37, 39, 40, 41, 43, 44, 45, 47, 48, 49, 51, 52, 53, 55, 56, 57, 59, 60, 61, 63, 64, 65, 67, 68, 69, 71, 72, 73, 75, 76, 77, 79, 80, 81, 83, 84, 85, 87, 88, 89, 91, 92, 93, 95, 96, 97, 99, 100, 101, 103, 104, 105, 107, 108, 109, 111, 112, 113, 115, 116, 117, 119, 120, 121, 123, 124, 125, 127, 128, 129, 131, 132, 133, 135, 136, 137, 139, 140, 141, 143, 144, 145, 147, 148, 149, 151, 152, 153, 155, 156, 157, 159, 160, 161, 163, 164, 165, 167, 168, 169, 171, 172, 173, 175, 176, 177, 179, 180, 181, 183, 184, 185, 187, 188, 189, 191, 192, 193, 195, 196, 197, 199, 200, 201, 203, 204, 205, 207, 208, 209, 211, 212, 213, 215, 216, 217, 219, 220, 221, 223, 224, 225, 227, 228, 229, 231, 232, 233, 235, 236, 237, 239, 240, 241, 243, 244, 245, 247, 248, 249, 251, 252, 253, 255, 256, 257, 259, 260
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