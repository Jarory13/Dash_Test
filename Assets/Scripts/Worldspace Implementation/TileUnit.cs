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

    //Because placing spills on top of a tile intercepts it's mouse down input, we need this function to be able to call the MouseDownMethod externally
    public void MoustDoneIntercept()
    {
        OnMouseDown();
    }

    private void OnMouseDown()
    {
        Debug.Log($"clicked {gameObject.name}");
        if (!occupied)
        {
            Player playerRef = GridManager.instance.playerReference;

            if (!playerRef || playerRef.isMoving || playerRef.isCleaning)
            {
                return;
            }

            //StartCoroutine(playerRef.MoveWait(myIndex, transform));
            playerRef.MoveToTarget(myIndex, transform);
        }
    }
}
