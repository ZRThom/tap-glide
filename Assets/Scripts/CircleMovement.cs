using UnityEngine;

public class CircleMovement : MonoBehaviour
{
    public double spawnDspTime;
    public float speed = 5f;
    public string direction; // left, up, down, right
    public double expectedTick; // Le tick auquel ce cercle devrait être frappé

    private Vector3 spawnPosition;
    private const float DESPAWN_Y = -5f; // Position de destruction automatique

    void Start()
    {
        spawnPosition = transform.position;
    }

    void Update()
    {
        double timeSinceSpawn = AudioSettings.dspTime - spawnDspTime;
        float newY = spawnPosition.y - (float)(timeSinceSpawn * speed);
        transform.position = new Vector3(spawnPosition.x, newY, spawnPosition.z);

        // Auto-destroy si trop loin en bas
        if (newY < DESPAWN_Y)
        {
            Destroy(gameObject);
        }
    }

    public void DestroyCircle()
    {
        Destroy(gameObject);
    }
}
