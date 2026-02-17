using UnityEngine;

public class exit_button : MonoBehaviour
{
    public void QuitterJeu()
    {
        Debug.Log("Le Jeu va se fermer...(EN BUILD FINAL SEULEMENT, PAS DANS L'EDITEUR)");

        Application.Quit();
    }
}


