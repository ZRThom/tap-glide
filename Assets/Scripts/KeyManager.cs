using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls; 
using System.Collections.Generic;



public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }
    
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

    public List<double> leftQueue = new List<double>();
    public List<double> upQueue = new List<double>();
    public List<double> downQueue = new List<double>();
    public List<double> rightQueue = new List<double>();
    public List<GameObject> currentLeftQueue = new List<GameObject>();
    public List<GameObject> currentUpQueue = new List<GameObject>();
    public List<GameObject> currentDownQueue = new List<GameObject>();
    public List<GameObject> currentRightQueue = new List<GameObject>();

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
    }

    void Update()
    {
        var Input = Keyboard.current;
        if (Input == null || settings == null) return;

        var leftKey = Input[settings.keyLeft] as KeyControl;
        if (leftKey != null) {
            if (leftKey.wasPressedThisFrame) left.GetComponent<Renderer>().material.color = Color.red;
            if (leftKey.wasReleasedThisFrame) left.GetComponent<Renderer>().material.color = Color.white;
        }

        var upKey = Input[settings.keyUp] as KeyControl;
        if (upKey != null) {
            if (upKey.wasPressedThisFrame) up.GetComponent<Renderer>().material.color = Color.green;
            if (upKey.wasReleasedThisFrame) up.GetComponent<Renderer>().material.color = Color.white;
        }

        var downKey = Input[settings.keyDown] as KeyControl;
        if (downKey != null) {
            if (downKey.wasPressedThisFrame) down.GetComponent<Renderer>().material.color = Color.blue;
            if (downKey.wasReleasedThisFrame) down.GetComponent<Renderer>().material.color = Color.white;
        }

        var rightKey = Input[settings.keyRight] as KeyControl;
        if (rightKey != null) {
            if (rightKey.wasPressedThisFrame) right.GetComponent<Renderer>().material.color = Color.yellow;
            if (rightKey.wasReleasedThisFrame) right.GetComponent<Renderer>().material.color = Color.white;
        }
    }
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

    public void ClearQueues()
    {
        leftQueue.Clear();
        upQueue.Clear();
        downQueue.Clear();
        rightQueue.Clear();
        currentLeftQueue.Clear();
        currentUpQueue.Clear();
        currentDownQueue.Clear();
        currentRightQueue.Clear();
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
        addQueue(direction, circle);
    }

    private void addQueue(string direction, GameObject Circle)
    {
        switch (direction)
        {
            case "left":
                currentLeftQueue.Add(Circle);
                break;
            case "up":
                currentUpQueue.Add(Circle);
                break;
            case "down":
                currentDownQueue.Add(Circle);
                break;
            case "right":
                currentRightQueue.Add(Circle);
                break;
        }
    }
    
}