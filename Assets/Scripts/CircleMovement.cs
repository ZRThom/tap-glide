using UnityEngine;

public class CircleMovement : MonoBehaviour
{
    public double spawnDspTime;
    public float speed = 5f;

    private Vector3 spawnPosition;
    void Start()
    {
        spawnPosition = transform.position;
    }

    void Update()
    {
        double timeSinceSpawn = AudioSettings.dspTime - spawnDspTime;
        transform.position = spawnPosition + Vector3.down * (float)(timeSinceSpawn * speed);
    }
}
