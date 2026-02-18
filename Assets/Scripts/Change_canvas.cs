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

    private static string canvasAActiver = "";

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

    public void AfficherMenu()       => AppliquerAffichage("Menu");
    public void AfficherCredits()    => AppliquerAffichage("Credits");
    public void AfficherSettings()   => AppliquerAffichage("Settings");
    public void AfficherGameSelect() => AppliquerAffichage("Canvas Game_select");
    public void AfficherLevel1()     => AppliquerAffichage("Level 1");
    public void AfficherLevel2()     => AppliquerAffichage("Level 2");
    public void AfficherLevel3()     => AppliquerAffichage("Level 3");

    private void AppliquerAffichage(string nom)
    {
        if(menuPrincipal)    menuPrincipal.SetActive(false);
        if(creditsCanvas)    creditsCanvas.SetActive(false);
        if(settingsCanvas)   settingsCanvas.SetActive(false);
        if(gameSelectCanvas) gameSelectCanvas.SetActive(false);
        if(jeuCanvas)        jeuCanvas.SetActive(false); 
        if(level2Canvas)     level2Canvas.SetActive(false);
        if(level3Canvas)     level3Canvas.SetActive(false);

        switch (nom)
        {
            case "Canvas Game_select":
            case "game_select":
                if(gameSelectCanvas) gameSelectCanvas.SetActive(true);
                break;

            case "Menu": 
                if(menuPrincipal) menuPrincipal.SetActive(true); 
                break;

            case "Credits": 
                if(creditsCanvas) creditsCanvas.SetActive(true); 
                break;

            case "Settings": 
                if(settingsCanvas) settingsCanvas.SetActive(true); 
                break;

            case "Level 1": 
            case "Jeu":
                if(jeuCanvas) jeuCanvas.SetActive(true); 
                break;

            case "Level 2": 
                if(level2Canvas) level2Canvas.SetActive(true); 
                break;

            case "Level 3": 
                if(level3Canvas) level3Canvas.SetActive(true); 
                break;
                
            default:
                Debug.LogWarning("Le nom '" + nom + "' n'est pas reconnu dans le switch.");
                break;
        }
    }
}