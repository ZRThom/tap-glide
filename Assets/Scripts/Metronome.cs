using UnityEngine;
using System; 
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
        var keyboard = Keyboard.current;
        if (keyboard == null || km == null) return;

        //Pour la calibration 
        if(Change_canvas.IsCalibrationActive && keyboard.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("Calibration reset");
            Scorer();
        }

        void HandleKey(KeyControl key, string direction)
        {
            if(!key.wasPressedThisFrame) return;
            double expectedTick = km.TryHitCircle(direction);
            if (expectedTick >= 0) // If a circle was hit
            {
                Scorer(expectedTick);
                Debug.Log("Hit on " + direction + " - Expected tick: " + expectedTick + " - Current tick: " + tickPassed);
            }
        }

        var leftKey = keyboard[km.settings.keyLeft] as KeyControl;
        var upKey = keyboard[km.settings.keyUp] as KeyControl;
        var downKey = keyboard[km.settings.keyDown] as KeyControl;
        var rightKey = keyboard[km.settings.keyRight] as KeyControl;

        HandleKey(leftKey, "left");
        HandleKey(upKey, "up");
        HandleKey(downKey, "down");
        HandleKey(rightKey, "right");
    }

    public void SetCalibration(float val, bool save = true)
    {
        calibrationMS = val;
        if (save) PlayerPrefs.SetFloat(CALIB_KEY, val);
    }

    // Calibration mode - no expected tick
    public void Scorer()
    {
        Scorer(tickPassed);
    }

    // Normal scoring with expected tick
    public void Scorer(double expectedTick)
    {
        double samplesPerTick = sampleRate * 60.0F / bpm * 4.0F / signatureLo;
        double currentSampleAbs = (AudioSettings.dspTime - calibrationMS / 1000.0) * sampleRate;
        double sampleSinceStart = currentSampleAbs - startSample;
        if (sampleSinceStart < 0) return;

        // Apply halfTickOffset if needed
        if (halfTickOffset) sampleSinceStart += samplesPerTick * 0.5f;

        // Calculate current tick based on time elapsed
        double currentTick = sampleSinceStart / samplesPerTick;
        
        // Error in ticks
        double errorTicks = currentTick - expectedTick;
        
        // Normalize to [-0.5, 0.5] tick range (wrapping around)
        while (errorTicks > 0.5) errorTicks -= 1.0;
        while (errorTicks < -0.5) errorTicks += 1.0;
        
        // Convert to samples and then ms
        double errorSamples = errorTicks * samplesPerTick;
        float errorMs = (float)(errorSamples / sampleRate * 1000.0f);
        float absMs = Mathf.Abs(errorMs);
        bool isEarly = errorMs < 0f;

        Debug.Log($"Timing: errorMs={errorMs:F2}ms, absMs={absMs:F2}ms, currentTick={currentTick:F2}, expectedTick={expectedTick}, isEarly={isEarly}");

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
            scoreManager?.AddPoints(-3);
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
        
        if (tickPassed + 6 == LeftToSpawn) { i++; km.Spawn(speed, "left", LeftToSpawn); LeftToSpawn = km.getNextLeft(i); }
        if (tickPassed + 6 == UpToSpawn) { j++; km.Spawn(speed, "up", UpToSpawn); UpToSpawn = km.getNextUp(j); }
        if (tickPassed + 6 == DownToSpawn) { k++; km.Spawn(speed, "down", DownToSpawn); DownToSpawn = km.getNextDown(k); }
        if (tickPassed + 6 == RightToSpawn) { l++; km.Spawn(speed, "right", RightToSpawn); RightToSpawn = km.getNextRight(l); }
    }
}