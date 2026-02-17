using TMPro;
using UnityEngine;

public class RightScrollerTMP : MonoBehaviour
{
    public float scrollSpeed = 100f;    

    public float startXPosition = 0f;  
    public float limitX = 500f;        
    
    private RectTransform rectTransform;
    private int direction = 1;      

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
        

        rectTransform.anchoredPosition = new Vector2(startXPosition, rectTransform.anchoredPosition.y);
        direction = 1; 
    }

    void Update()
    {
        float movement = scrollSpeed * Time.deltaTime * direction;
        rectTransform.anchoredPosition += new Vector2(movement, 0);

        if (rectTransform.anchoredPosition.x >= limitX)
        {
            direction = -1; 
        }
        else if (rectTransform.anchoredPosition.x <= startXPosition)
        {
            direction = 1;  
        }
    }
}