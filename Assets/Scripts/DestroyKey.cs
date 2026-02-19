using UnityEngine;

public class DestroyKey : MonoBehaviour
{
    KeyManager km;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        km = KeyManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Compter une faute ICI;
        Destroy(other.gameObject);
    }
}
