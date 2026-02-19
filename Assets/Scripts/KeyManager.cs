using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls; 
using System.Collections.Generic;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }

    public Sprite spriteBase;
    public Sprite spriteLeft;
    public Sprite spriteDown;
    public Sprite spriteUp;
    public Sprite spriteRight;
    
    [SerializeField] public CustomSettings settings;

    [SerializeField]
    private GameObject left;

    [SerializeField]
    private GameObject up;

    [SerializeField]
    private GameObject down;

    [SerializeField]
    private GameObject right;

    [SerializeField]
    private GameObject Circle;

    private List<double> leftQueue = new List<double>();
    private List<double> upQueue = new List<double>();
    private List<double> downQueue = new List<double>();
    private List<double> rightQueue = new List<double>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (settings != null) {
            settings.LoadSettings();
            Debug.Log("Touches chargées depuis le JSON");
        }
        setBase(left);
        setBase(up);
        setBase(down);
        setBase(right);
    }

    private void setBase(GameObject obj)
    {
        if (obj == null) return;
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr != null && spriteBase != null) sr.sprite = spriteBase;
    }

    void Update()
    {
        var Input1 = Keyboard.current;
        if (Input1 == null || settings == null) return;

        HandleSprite(left, Input1[settings.keyLeft] as KeyControl, spriteLeft);
        HandleSprite(down, Input1[settings.keyDown] as KeyControl, spriteDown);
        HandleSprite(up, Input1[settings.keyUp] as KeyControl, spriteUp);
        HandleSprite(right, Input1[settings.keyRight] as KeyControl, spriteRight);
    }

    public void HandleSprite(GameObject obj, KeyControl key, Sprite pressedSprite)
    {
        if (obj == null || key == null) return;
        var sr = obj.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError($"{obj.name} don't have spriteRenderer");
            return;
        }
        if (key.wasPressedThisFrame) sr.sprite = pressedSprite;
        if (key.wasReleasedThisFrame) sr.sprite = spriteBase;
    }
    public void SpawnLeft(double speed) { Spawn(speed, "left"); }
    public void SpawnUp(double speed) { Spawn(speed, "up"); }
    public void SpawnDown(double speed) { Spawn(speed, "down"); }
    public void SpawnRight(double speed) { Spawn(speed, "right"); }

    public double getNextLeft(int index)
    {
        if (leftQueue.Count > index)
        {
            double next = leftQueue[index];
            return next;
        } else if (index == 100) {
            return leftQueue.Count;
        }
        return -1;
    }
    public double getNextUp(int index)
    {
        if (upQueue.Count > index)
        {
            double next = upQueue[index];
            return next;
        } else if (index == 100) {
            return upQueue.Count;
        }
        return -1;
    }
    public double getNextDown(int index)
    {
        if (downQueue.Count > index)
        {
            double next = downQueue[index];
            return next;
        } else if (index == 100) {
            return downQueue.Count;
        }
        return -1;
    }
    public double getNextRight(int index)
    {
        if (rightQueue.Count > index)
        {
            double next = rightQueue[index];
            return next;
        } else if (index == 100) {
            return rightQueue.Count;
        }
        return -1; //Template value
    }

    public void EnqueueNotes(double[] notes, System.Action<double> setter)
    {
        foreach (double note in notes)
        {
            setter(note);
        }
    }

    public void setLeftQueue(double Toqueue)
    {
        leftQueue.Add(Toqueue);
    }

    public void setUpQueue(double queue)
    {
        upQueue.Add(queue);
    }

    public void setDownQueue(double queue)
    {
        downQueue.Add(queue);
    }

    public void setRightQueue(double queue)
    {
        rightQueue.Add(queue);
    }

    public void Spawn(double speed, string direction)
    {
        float xPos = 0f;
        switch (direction)
        {
            case "left":
                xPos = -2.25f;
                break;
            case "up":
                xPos = -0.75f;
                break;
            case "down":
                xPos = 0.75f;
                break;
            case "right":
                xPos = 2.25f;
                break;
        }

        GameObject circle = Instantiate(Circle, new Vector2(xPos, 10f), Quaternion.identity);
        CircleMovement movement = circle.GetComponent<CircleMovement>();
        movement.spawnDspTime = AudioSettings.dspTime;
        movement.speed = (float)speed;
    }
    
}