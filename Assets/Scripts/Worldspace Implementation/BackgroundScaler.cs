using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScaler : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        ResizeSpriteToScreen();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ResizeSpriteToScreen()
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (spriteRenderer)
        {

            transform.localScale = Vector3.one;

            float width = spriteRenderer.sprite.bounds.size.x;
            float height = spriteRenderer.sprite.bounds.size.y;

            var worldScreenHeight = Camera.main.orthographicSize * 2.0;
            var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            transform.localScale = new Vector3((float)worldScreenWidth / width, (float)worldScreenHeight / height, 1.0f);
        }
    }
}
