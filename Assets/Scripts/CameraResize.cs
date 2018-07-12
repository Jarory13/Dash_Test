using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResize : MonoBehaviour
{

    [SerializeField]
    private Canvas mainCanvas;

    // Use this for initialization
    void Start()
    {
        if (mainCanvas)
        {
            RectTransform canvasRect = mainCanvas.GetComponent<RectTransform>();

            gameObject.GetComponent<Camera>().orthographicSize = (canvasRect.rect.width / canvasRect.rect.height);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
