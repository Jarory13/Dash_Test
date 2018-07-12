using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleBackground : MonoBehaviour
{

    private Canvas canvas;
    RectTransform rectTransform;

    // Use this for initialization
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        rectTransform = gameObject.GetComponent<RectTransform>();

        if (canvas && rectTransform)
        {
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();

            if (canvasRect)
            {
                //Set the scale of the background to the size of the canvas then move the background to the same anchor point
                rectTransform.sizeDelta = new Vector2(canvasRect.rect.width, canvasRect.rect.height);
                rectTransform.position = canvasRect.position;

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
