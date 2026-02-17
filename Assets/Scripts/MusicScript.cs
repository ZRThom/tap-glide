using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicScript : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        double startTime = AudioSettings.dspTime + 0.2; // 1 second delay

        audioSource.PlayScheduled(startTime);
    }
}