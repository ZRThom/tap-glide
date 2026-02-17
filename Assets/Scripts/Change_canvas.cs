using UnityEngine;

public class Change_canvas : MonoBehaviour
{
    public GameObject menuPrincipal;
    public GameObject creditsCanvas;
    public GameObject settingsCanvas;
    public GameObject jeuCanvas;

    public void AfficherCredits()
    {
        menuPrincipal.SetActive(false);
        creditsCanvas.SetActive(true);
    }

    public void AfficherMenu()
    {
        creditsCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
        jeuCanvas.SetActive(false);
        
        menuPrincipal.SetActive(true);
    }

    public void AfficherJeu()
    {
        menuPrincipal.SetActive(false);
        jeuCanvas.SetActive(true); 
    }

    public void AfficherSettings()
    {
        menuPrincipal.SetActive(false);
        settingsCanvas.SetActive(true); 
    }
}