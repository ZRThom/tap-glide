using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

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

    private KeyManager km;

    private bool CreateCircle = false;
    double LeftToSpawn = 0.0;
    double UpToSpawn = 0.0;
    double DownToSpawn = 0.0;
    double RightToSpawn = 0.0;

    private int i = 0;
    private int j = 0;
    private int k = 0;
    private int l = 0;

    private double tickPassed = 0.0;

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
        km = KeyManager.Instance;
        calibrationMS = PlayerPrefs.GetFloat(CALIB_KEY, calibrationMS);
        accent = signatureHi;
        sampleRate = AudioSettings.outputSampleRate;
        startDspTime = AudioSettings.dspTime + startOffsetSeconds;
        startSample = startDspTime * sampleRate;
        nextTick = startSample;
        running = true;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!running) return;

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
                nextTick += samplesPerTick;
                amp = 1.0F;
                if (++accent > signatureHi)
                {
                    accent = 1;
                    amp *= 2.0F;
                }
            }
            phase += amp * 0.3F;
            amp *= 0.993F;
            n++;
        }
    }

    void Update()
    {
        var inputDevice = Keyboard.current;
        if (inputDevice == null || km == null || km.settings == null) return;

        var leftKey = inputDevice[km.settings.keyLeft] as KeyControl;
        var upKey = inputDevice[km.settings.keyUp] as KeyControl;
        var downKey = inputDevice[km.settings.keyDown] as KeyControl;
        var rightKey = inputDevice[km.settings.keyRight] as KeyControl;

        bool AnyDirectionPressed()
        {
            return (leftKey != null && leftKey.wasPressedThisFrame) ||
                   (upKey != null && upKey.wasPressedThisFrame) ||
                   (downKey != null && downKey.wasPressedThisFrame) ||
                   (rightKey != null && rightKey.wasPressedThisFrame);
        }

        if (AnyDirectionPressed())
        {
            if (leftKey != null && leftKey.wasPressedThisFrame)
            {
                for (int i = 0; i < km.getNextLeft(100); i++)
                {
                    if (tickPassed == km.getNextLeft(i)) { Scorer(); break; }
                }
            }
            if (upKey != null && upKey.wasPressedThisFrame)
            {
                for (int i = 0; i < km.getNextUp(100); i++)
                {
                    if (tickPassed == km.getNextUp(i)) { Scorer(); break; }
                }
            }
            if (downKey != null && downKey.wasPressedThisFrame)
            {
                for (int i = 0; i < km.getNextDown(100); i++)
                {
                    if (tickPassed == km.getNextDown(i)) { Scorer(); break; }
                }
            }
            if (rightKey != null && rightKey.wasPressedThisFrame)
            {
                for (int i = 0; i < km.getNextRight(100); i++)
                {
                    if (tickPassed == km.getNextRight(i)) { Scorer(); break; }
                }
            }
        }
    }

    public void SetCalibration(float val, bool save = true)
    {
        calibrationMS = val;
        if (save) PlayerPrefs.SetFloat(CALIB_KEY, val);
    }

    public void Scorer()
    {
        double samplesPerTick = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
        double currentSampleAbs = (AudioSettings.dspTime - calibrationMS / 1000.0) * sampleRate;
        double sampleSinceStart = currentSampleAbs - startSample;
        if (sampleSinceStart < 0) return;

        if (halfTickOffset) sampleSinceStart += samplesPerTick * 0.5f;

        double withinTick = sampleSinceStart % samplesPerTick;
        if (withinTick < 0) withinTick += samplesPerTick;
        
        double signedErrorSamples = withinTick;
        if (withinTick > samplesPerTick * 0.5) signedErrorSamples = withinTick - samplesPerTick;
        
        float signedErrorMs = (float)(signedErrorSamples / sampleRate * 1000.0f);
        float absMs = Mathf.Abs(signedErrorMs);
        bool isEarly = signedErrorMs < 0f;

        if (absMs <= perfectMs)
        {
            spawner?.ShowPerfect();
            scoreManager?.AddPoints(5);
            pulseCircle?.PulsePerfect();
        }
        else if (absMs <= niceMs)
        {
            spawner?.ShowNice();
            scoreManager?.AddPoints(3);
            pulseCircle?.PulseNice();
        }
        else if (absMs <= okMs)
        {
            spawner?.ShowOK();
            scoreManager?.AddPoints(1);
            pulseCircle?.PulseOk();
        }
        else
        {
            spawner?.ShowBad();
            pulseCircle?.PulseBad();
        }
    }

    void FixedUpdate()
    {
        double samplesPerTick = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
        sampleRate = AudioSettings.outputSampleRate;
        if (!CreateCircle) return;
        CreateCircle = false;
        double tickDuration = samplesPerTick / sampleRate;
        double timeUntilHit = 6 * tickDuration;
        double distanceToTravel = 10f - (-3.5f);
        double speed = distanceToTravel / timeUntilHit;

        if (tickPassed == 1)
        {
            LeftToSpawn = km.getNextLeft(i);
            UpToSpawn = km.getNextUp(i);
            DownToSpawn = km.getNextDown(i);
            RightToSpawn = km.getNextRight(i);
        }

        if (km == null) return;
        
        if (tickPassed + 6 == LeftToSpawn) { i++; km.SpawnLeft(speed); LeftToSpawn = km.getNextLeft(i); }
        if (tickPassed + 6 == UpToSpawn) { j++; km.SpawnUp(speed); UpToSpawn = km.getNextUp(j); }
        if (tickPassed + 6 == DownToSpawn) { k++; km.SpawnDown(speed); DownToSpawn = km.getNextDown(k); }
        if (tickPassed + 6 == RightToSpawn) { l++; km.SpawnRight(speed); RightToSpawn = km.getNextRight(l); }
    }
}