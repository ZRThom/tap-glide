using UnityEngine;
using System;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using System.Collections.Generic;

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
        if (keyboard == null) return;

        //Pour la calibration 
        /* if (Change_canvas.IsCalibrationActive && keyboard.spaceKey.wasPressedThisFrame)
         {
             Debug.Log("Calibration reset");
             Scorer();
         }*/

        void HandleKey(KeyControl key, Func<int, double> getNext, string direction)
        {
            if (key == null || !key.wasPressedThisFrame || getNext(100) == 0) return;
            double nextDouble = getNext(0);
            Debug.Log("Next " + direction + " tick: " + nextDouble);
            //If we are within 1 tick of the supposed tick, we count it as a hit.
            if (Math.Abs(tickPassed - nextDouble) <= 1)
            {
                List<GameObject> currentQueue = null;
                List<double> queue = null;
                switch (direction)
                {
                    case "left":
                        currentQueue = km.currentLeftQueue;
                        queue = km.leftQueue;
                        break;
                    case "up":
                        currentQueue = km.currentUpQueue;
                        queue = km.upQueue;
                        break;
                    case "down":
                        currentQueue = km.currentDownQueue;
                        queue = km.downQueue;
                        break;
                    case "right":
                        currentQueue = km.currentRightQueue;
                        queue = km.rightQueue;
                        break;
                }
                Scorer(nextDouble, currentQueue, queue);
                Debug.Log("Clicked at tick " + tickPassed);
            }
            else
            {
                Debug.Log("Clicked at tick " + tickPassed + " but expected " + nextDouble);
            }
        }

        var leftKey = keyboard[km.settings.keyLeft] as KeyControl;
        var upKey = keyboard[km.settings.keyUp] as KeyControl;
        var downKey = keyboard[km.settings.keyDown] as KeyControl;
        var rightKey = keyboard[km.settings.keyRight] as KeyControl;
        HandleKey(leftKey, km.getNextLeft, "left");
        HandleKey(upKey, km.getNextUp, "up");
        HandleKey(downKey, km.getNextDown, "down");
        HandleKey(rightKey, km.getNextRight, "right");

        void CleanUpMissedNotes()
        {
            // Check all 4 directions. 
            // If the tick in the logic queue is more than 1.5 ticks behind the current time, 
            // it means the note has flown past the hit zone and we missed it.

            CheckDirectionMiss(km.leftQueue, km.currentLeftQueue);
            CheckDirectionMiss(km.upQueue, km.currentUpQueue);
            CheckDirectionMiss(km.downQueue, km.currentDownQueue);
            CheckDirectionMiss(km.rightQueue, km.currentRightQueue);
        }

        void CheckDirectionMiss(List<double> logicQueue, List<GameObject> visualQueue)
        {
            // Use a while loop in case multiple notes were missed in a row
            while (logicQueue.Count > 0 && tickPassed > logicQueue[0] + 1.2)
            {
                Debug.Log("Missed note at tick " + logicQueue[0]);

                // 1. Remove from time list
                logicQueue.RemoveAt(0);

                // 2. Remove and destroy the actual circle
                if (visualQueue.Count > 0)
                {
                    GameObject note = visualQueue[0];
                    visualQueue.RemoveAt(0);
                    Destroy(note);
                }

                // 3. Optional: Trigger a "Miss" UI/Sound
                spawner?.ShowBad();
            }
        }

        CleanUpMissedNotes();
    }

    public void SetCalibration(float val, bool save = true)
    {
        calibrationMS = val;
        if (save) PlayerPrefs.SetFloat(CALIB_KEY, val);
    }

    public void Scorer(double supposedTick, List<GameObject> currentQueue = null, List<double> queue = null)
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

        // Judgement Logic
        if (absMs <= perfectMs)
        {
            spawner?.ShowPerfect();
            scoreManager?.AddPoints(5);
            pulseCircle?.PulsePerfect();
            Debug.Log($"Perfect! Error: {signedErrorMs:F2}ms");
        }
        else if (absMs <= niceMs)
        {
            spawner?.ShowNice();
            scoreManager?.AddPoints(3);
            pulseCircle?.PulseNice();
            Debug.Log($"Nice! Error: {signedErrorMs:F2}ms");
        }
        else if (absMs <= okMs)
        {
            spawner?.ShowOK();
            scoreManager?.AddPoints(1);
            pulseCircle?.PulseOk();
            Debug.Log($"OK! Error: {signedErrorMs:F2}ms");
        }
        else
        {
            spawner?.ShowBad();
            scoreManager?.AddPoints(-3);
            pulseCircle?.PulseBad();
            Debug.Log($"Bad/Late! Error: {signedErrorMs:F2}ms");
        }

        // 5. Cleanup (Essential to keep the game from stopping)
        if (currentQueue != null && currentQueue.Count > 0)
        {
            var noteObj = currentQueue[0];
            currentQueue.RemoveAt(0);
            Destroy(noteObj);
        }

        if (queue != null && queue.Count > 0)
        {
            queue.RemoveAt(0);
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

        if (tickPassed + 6 >= LeftToSpawn) { i++; km.Spawn(speed, "left"); LeftToSpawn = km.getNextLeft(i); }
        if (tickPassed + 6 >= UpToSpawn) { j++; km.Spawn(speed, "up"); UpToSpawn = km.getNextUp(j); }
        if (tickPassed + 6 >= DownToSpawn) { k++; km.Spawn(speed, "down"); DownToSpawn = km.getNextDown(k); }
        if (tickPassed + 6 >= RightToSpawn) { l++; km.Spawn(speed, "right"); RightToSpawn = km.getNextRight(l); }
    }
}