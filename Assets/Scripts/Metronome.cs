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
    private double tickPassed = 0.0;
    private KeyManager km;

    //Mandatory boolean, since we cant call functions from music thread
    private bool CreateCircle = false;
    // Instantiate there, to not call them every tick
    double LeftToSpawn = 0.0;
    double UpToSpawn = 0.0;
    double DownToSpawn = 0.0;
    double RightToSpawn = 0.0;

    private int i = 0;
    private int j = 0;
    private int k = 0;
    private int l = 0;

    [Header("FeedBack and score")]
    public FeedBackSpawner spawner;
    public ScoreManager scoreManager;
    public PulseCircle pulseCircle;

    [Header("Timing fix")]
    public float perfectMs = 45f;
    public float niceMs = 90f;
    public float okMs = 135f;
    public float calibrationMS = 0f;

    public bool halfTickOffset = true;
    private const string CALIB_KEY = "CALIBRATION_MS";
    void Start()
    {
        //get the queue of the keys to spawn from the KeyManager
        km = KeyManager.Instance;
        calibrationMS = PlayerPrefs.GetFloat(CALIB_KEY, calibrationMS);
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
                tickPassed++;
                CreateCircle = true;
                Debug.Log($"Tick Nb: {tickPassed}");
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
        bool AnyDirectionPressed()
        {
            return Input1.dKey.wasPressedThisFrame ||
                   Input1.fKey.wasPressedThisFrame ||
                   Input1.jKey.wasPressedThisFrame ||
                   Input1.kKey.wasPressedThisFrame;
        }

        if (AnyDirectionPressed())
        {
            if (Input1.dKey.wasPressedThisFrame)
            {
                for (int i = 0; i < km.getNextLeft(100); i++)
                {
                    if (tickPassed == km.getNextLeft(i))
                    {
                        Scorer();
                        break;
                    }
                }
            }
            if (Input1.fKey.wasPressedThisFrame)
            {
                for (int i = 0; i < km.getNextUp(100); i++)
                {
                    if (tickPassed == km.getNextUp(i))
                    {
                        Scorer();
                        break;
                    }
                }
            }
            if (Input1.jKey.wasPressedThisFrame)
            {
                for (int i = 0; i < km.getNextDown(100); i++)
                {
                    if (tickPassed == km.getNextDown(i))
                    {
                        Scorer();
                        break;
                    }
                }
            }
            if (Input1.kKey.wasPressedThisFrame)
            {
                for (int i = 0; i < km.getNextRight(100); i++)
                {
                    if (tickPassed == km.getNextRight(i))
                    {
                        Scorer();
                        break;
                    }
                }
            }
        }
    }
//TODO : DETRUIRE LA REDONDANCE DE CODE AVEC LES 4 DIRECTIONS, C'EST HORRIBLE, SURTOUT KEYMANAGER
//TODO : ENLEVER L'INSTANCE QUAND PRESSE, SAUF SI RATE.
    public void Scorer()
    {

        //Get the number of samples per tick
        double samplesPerTick = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
        //each frame, get the current sample (sample of the current frame)
        double currentSampleAbs = (AudioSettings.dspTime - calibrationMS / 1000.0) * sampleRate;
        // 2h j'en ai marre
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
            // debug reglage test 
            Debug.Log($"errMs={signedErrorMs:+0.0;-0.0;0.0} abs={absMs:0.0}");

            if (absMs <= perfectMs)
            {
                if (pulseCircle != null) pulseCircle.PulsePerfect();
                spawner?.ShowPerfect();
                scoreManager?.AddPoints(5);
                Debug.Log("Perfect");
            }
            else if (absMs <= niceMs)
            {
                if (pulseCircle != null) pulseCircle.PulseNice();
                spawner?.ShowNice();
                scoreManager?.AddPoints(3);
                Debug.Log(isEarly? "Early" : "Late");
            }
            else if (absMs <= okMs)
            {
                if (pulseCircle != null) pulseCircle.PulseOk();
                spawner?.ShowOK();
                scoreManager?.AddPoints(1);
                Debug.Log(isEarly? "TooEarly" : "TooLate");
            }
            else
            {
                if (pulseCircle != null) pulseCircle.PulseBad();
                spawner?.ShowBad();
                Debug.Log("Bad");
            }
        }
    }

    public void SetCalibration(float ms, bool save = true)
    {
        calibrationMS = ms;
        if (save)
        {
            PlayerPrefs.SetFloat(CALIB_KEY, calibrationMS);
            PlayerPrefs.Save();
        }
    }
}
