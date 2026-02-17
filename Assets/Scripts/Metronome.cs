using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class Metronome : MonoBehaviour
{

    public double bpm = 130.0F;
    public float gain = 0.5F;
    public int signatureHi = 4;
    public int signatureLo = 4;
    private double nextTick = 0.0;
    private float amp = 0.0F;
    private float phase = 0.0F;
    private double sampleRate = 0.0;
    private int accent;
    private bool running = false;
    
    private double startDspTime;
    private double startSample;
    public float startOffsetSeconds = 0.1f;


    [Header("FeedBack and score")]
    public FeedBackSpawner spawner;
    public ScoreManager scoreManager;

    [Header("Timing fix")]
    public float perfectMs = 45f;
    public float niceMs = 90f;
    public float okMs = 135f;
    public float calibrationMS = 0f;

    public bool halfTickOffset = true;
    void Start()
    {
        accent = signatureHi;
        sampleRate = AudioSettings.outputSampleRate;
        // delai pr eviter le decalage music de chiasse
        startDspTime = AudioSettings.dspTime + startOffsetSeconds;
        startSample = startDspTime * sampleRate;
        nextTick = startSample;
        running = true;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!running)
            return;

        double samplesPerTick = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
        double sample = AudioSettings.dspTime * sampleRate;
        int dataLen = data.Length / channels;
        int n = 0;
        while (n < dataLen)
        {
            float x = gain * amp * Mathf.Sin(phase);
            for (int i = 0; i < channels; i++)
            {
                data[n * channels + i] += x;
            }
            while (sample + n >= nextTick)
            {
                nextTick += samplesPerTick;
                amp = 1.0F;
                if (++accent > signatureHi)
                {
                    accent = 1;
                    amp *= 2.0F;
                }
                // Debug.Log("Tick: " + accent + "/" + signatureHi);
            }
            phase += amp * 0.3F;
            amp *= 0.993F;
            n++;
        }

    }

    void Update()
    {
        var Input1 = Keyboard.current;
        if (Input1 == null) return;
        if (Input1.spaceKey.wasPressedThisFrame)
        {
            //Get the number of samples per tick
            double samplesPerTick = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
            //each frame, get the current sample (sample of the current frame)
            double currentSampleAbs = (AudioSettings.dspTime - calibrationMS / 1000.0) * sampleRate;
            // 3h j'en ai marre
            // calc pos dans le beat
            //sample relatif (start)
            double sampleSinceStart = currentSampleAbs - startSample;
            // si startDspTime+0.1 pressed passé
            if (sampleSinceStart < 0) return;

            //fix decalage de moitié
            if (halfTickOffset) sampleSinceStart += samplesPerTick * 0.5f;

            // modulo de check (anti decalage en principe)
            double withinTick = sampleSinceStart % samplesPerTick;
            if (withinTick < 0) withinTick += samplesPerTick; 

            // fix inversement (horrible)
            double signedErrorSamples = withinTick;
            if (withinTick > samplesPerTick * 0.5) signedErrorSamples = withinTick - samplesPerTick; // inversement (+ = -)
            float signedErrorMs = (float)(signedErrorSamples / sampleRate * 1000.0f);
            float absMs = Mathf.Abs(signedErrorMs);
            // early / late (en + pr "nice", les too early et trucs sont les "ok")
            bool isEarly = signedErrorMs < 0f;
            // debug reglage
            Debug.Log($"errMs={signedErrorMs:+0.0;-0.0;0.0} abs={absMs:0.0}");

            if (absMs <= perfectMs)
            {
                spawner?.ShowPerfect();
                scoreManager?.AddPoints(5);
                Debug.Log("Perfect");
            }
            else if (absMs <= niceMs)
            {
                spawner?.ShowNice();
                scoreManager?.AddPoints(3);
                Debug.Log(isEarly? "Early" : "Late");
            }
            else if (absMs <= okMs)
            {
                spawner?.ShowOK();
                scoreManager?.AddPoints(1);
                Debug.Log(isEarly? "TooEarly" : "TooLate");
            }
            else
            {
                spawner?.ShowBad();
                Debug.Log("Bad");
            }
        }
    }
}
