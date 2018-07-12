using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPosition : MonoBehaviour
{

    [SerializeField]
    private float GridXOffset;

    [SerializeField]
    private float GridYOffset;


    [SerializeField]
    private float GridUnitScaler;


    private Canvas canvas;

    // Use this for initialization
    void Start()
    {
        GameObject canvasObject = GameObject.FindGameObjectWithTag("Canvas");

        if (canvasObject)
        {
            canvas = canvasObject.GetComponent<Canvas>();

            if (canvas)
            {

                RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();

                float CanvasWidth = canvasRect.rect.width;
                float CanvasHeight = canvasRect.rect.height;

                transform.SetParent(canvasObject.transform, false);


                RectTransform gridRect = gameObject.GetComponent<RectTransform>();

                //Make sure the grid is at proper scale, sized right for the canvas and then positioned in a good location
                gridRect.localScale = Vector3.one;
                gridRect.sizeDelta = new Vector2(CanvasWidth, CanvasHeight) / GridUnitScaler;

                gridRect.position = new Vector3(canvasRect.rect.xMin - GridXOffset, canvasRect.rect.yMin - GridYOffset, 0.0f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
