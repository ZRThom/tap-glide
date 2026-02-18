using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class KeyManager : MonoBehaviour
{
    public static KeyManager Instance { get; private set; }
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

    //get the preset keys and colors;
    void Start()
    {

    }

    void Update()
    {
        var Input = Keyboard.current;
        if (Input == null) return;
        if (Input.dKey.wasPressedThisFrame)
        {
            left.GetComponent<Renderer>().material.color = Color.red;
        }
        if (Input.fKey.wasPressedThisFrame)
        {
            up.GetComponent<Renderer>().material.color = Color.green;
        }
        if (Input.jKey.wasPressedThisFrame)
        {
            down.GetComponent<Renderer>().material.color = Color.blue;
        }
        if (Input.kKey.wasPressedThisFrame)
        {
            right.GetComponent<Renderer>().material.color = Color.yellow;
        }

        if (Input.dKey.wasReleasedThisFrame)
        {
            left.GetComponent<Renderer>().material.color = Color.white;
        }
        if (Input.fKey.wasReleasedThisFrame)
        {
            up.GetComponent<Renderer>().material.color = Color.white;
        }
        if (Input.jKey.wasReleasedThisFrame)
        {
            down.GetComponent<Renderer>().material.color = Color.white;
        }
        if (Input.kKey.wasReleasedThisFrame)
        {
            right.GetComponent<Renderer>().material.color = Color.white;
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

    public void SpawnLeft(double speed)
    {
        GameObject circle = Instantiate(Circle, new Vector2(-2.25f, 10f), Quaternion.identity);
        CircleMovement movement = circle.GetComponent<CircleMovement>();
        movement.spawnDspTime = AudioSettings.dspTime;
        movement.speed = (float)speed;
    }

    public void SpawnUp(double speed)
    {
        GameObject circle = Instantiate(Circle, new Vector2(-0.75f, 10f), Quaternion.identity);
        CircleMovement movement = circle.GetComponent<CircleMovement>();
        movement.spawnDspTime = AudioSettings.dspTime;
        movement.speed = (float)speed;
    }

    public void SpawnDown(double speed)
    {
        GameObject circle = Instantiate(Circle, new Vector2(0.75f, 10f), Quaternion.identity);

        CircleMovement movement = circle.GetComponent<CircleMovement>();
        movement.spawnDspTime = AudioSettings.dspTime;
        movement.speed = (float)speed;
    }

    public void SpawnRight(double speed)
    {
        GameObject circle = Instantiate(Circle, new Vector2(2.25f, 10f), Quaternion.identity);
        CircleMovement movement = circle.GetComponent<CircleMovement>();
        movement.spawnDspTime = AudioSettings.dspTime;
        movement.speed = (float)speed;
    }
}
