using UnityEngine;
using UnityEngine.InputSystem;

public class KeyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject left;

    [SerializeField]
    private GameObject up;

    [SerializeField]
    private GameObject down;

    [SerializeField]
    private GameObject right;

    //get the preset keys and colors;
    void Start()
    {
        
    }

    void Update()
    {
        var Input = Keyboard.current;
        if(Input == null) return;
        if(Input.dKey.wasPressedThisFrame)
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


}
