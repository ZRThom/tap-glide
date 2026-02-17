using UnityEngine;
using TMPro;

public class CreditsScroll : MonoBehaviour
{
    public float scrollSpeed = 25f;
    public float startYPosition = -1200f; 
    
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }


    void OnEnable()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        
      
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, startYPosition);
    }

    void Update()
    {
  
        rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
    }
}