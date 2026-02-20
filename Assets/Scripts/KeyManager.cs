using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls; 
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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

    // Track spawned circles for hit detection
    private List<GameObject> spawnedCircles = new List<GameObject>();
    
    private const float HIT_ZONE_Y = -3.5f; // Position de la zone de hit visual
    private const float HIT_TOLERANCE = 1.2f; // Y tolerance for hitting (covers Good range)

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

    public void Spawn(double speed, string direction, double expectedTick = -1)
    {
        Color color = Color.white;
        float xPos = 0f;
        switch (direction)
        {
            case "left":
                xPos = -2.25f;
                color = Color.yellow;
                break;
            case "up":
                xPos = -0.75f;
                color = Color.green;
                break;
            case "down":
                xPos = 0.75f;
                color = Color.blue;
                break;
            case "right":
                xPos = 2.25f;
                color = Color.red;
                break;
        }

        GameObject circle = Instantiate(Circle, new Vector2(xPos, 10f), Quaternion.identity);
        CircleMovement movement = circle.GetComponent<CircleMovement>();
        circle.GetComponent<SpriteRenderer>().color = color; // Set color for visual distinction
        movement.spawnDspTime = AudioSettings.dspTime;
        movement.speed = (float)speed;
        movement.direction = direction;
        movement.expectedTick = expectedTick;
        
        spawnedCircles.Add(circle);
    }

    // Try to hit a circle in the specified direction and return its expected tick
    public double TryHitCircle(string direction)
    {
        // Find the closest circle in the right direction and Y position
        float targetX = 0f;
        switch (direction)
        {
            case "left":
                targetX = -2.25f;
                break;
            case "up":
                targetX = -0.75f;
                break;
            case "down":
                targetX = 0.75f;
                break;
            case "right":
                targetX = 2.25f;
                break;
            default:
                return -1;
        }

        GameObject targetCircle = null;
        float closestDistance = float.MaxValue;
        double hitExpectedTick = -1;

        // Clean up destroyed circles and find the best target
        spawnedCircles.RemoveAll(c => c == null);

        foreach (GameObject circle in spawnedCircles)
        {
            CircleMovement cm = circle.GetComponent<CircleMovement>();
            if (cm == null) continue;

            // Check if it's in the correct direction
            if (cm.direction != direction) continue;

            // Check if it's in the hit zone
            float circleY = circle.transform.position.y;
            float distanceFromHitZone = Mathf.Abs(circleY - HIT_ZONE_Y);

            if (distanceFromHitZone <= HIT_TOLERANCE && distanceFromHitZone < closestDistance)
            {
                closestDistance = distanceFromHitZone;
                targetCircle = circle;
                hitExpectedTick = cm.expectedTick;
            }
        }

        if (targetCircle != null)
        {
            targetCircle.GetComponent<CircleMovement>().DestroyCircle();
            return hitExpectedTick;
        }

        return -1;
    }

    public void ClearQueues()
    {
        leftQueue.Clear();
        upQueue.Clear();
        downQueue.Clear();
        rightQueue.Clear();
    }

    public void RefreshAfterRebind()
    {
        if (settings != null) settings.LoadSettings();
        setBase(left);
        setBase(up);
        setBase(down);
        setBase(right);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisnable() => SceneManager.sceneLoaded -= OnSceneLoaded;
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        left = GameObject.FindGameObjectWithTag("KeyLeft");
        up = GameObject.FindGameObjectWithTag("KeyUp");
        down = GameObject.FindGameObjectWithTag("KeyDown");
        right = GameObject.FindGameObjectWithTag("KeyRight");

        RefreshAfterRebind();
    }
    
}