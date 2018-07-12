using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileUnit : MonoBehaviour
{

    //Is something here? Often this means we can't pass through these nodes
    public bool occupied = false;
    public bool conatinsSplill;
    public NodeIndex myIndex = new NodeIndex();

    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnMouseDown()
    {
        Debug.Log($"clicked {gameObject.name}");
        if (!occupied)
        {
            StartCoroutine(GridManager.instance.playerReference.MoveWait(myIndex, transform));
        }
    }
}
