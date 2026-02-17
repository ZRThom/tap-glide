using UnityEngine;

public class FeedBackOnTrigger2D : MonoBehaviour
{
    public FeedBackSpawner spawner;
    public ScoreManager scoreManager;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Perfect")) 
        {
            spawner.ShowPerfect();
            scoreManager.AddPoints(5);
        }
        else if (other.CompareTag("Nice")) 
        {
            spawner.ShowNice();
            scoreManager.AddPoints(3);
        }
        else if (other.CompareTag("OK")) 
        {
            spawner.ShowOK();
            scoreManager.AddPoints(1);
        }
        else if (other.CompareTag("Bad"))
        {
            spawner.ShowBad();
        }
    }
}
