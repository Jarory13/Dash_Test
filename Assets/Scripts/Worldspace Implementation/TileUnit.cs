using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileUnit : MonoBehaviour
{

    //Is something here? Often this means we can't pass through these nodes
    public bool occupied;
    public bool conatinsSplill;

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
        //GridManager.instance.playerReference.WeShouldMove();
    }
}
