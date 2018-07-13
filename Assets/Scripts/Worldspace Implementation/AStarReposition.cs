using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AStarReposition : MonoBehaviour
{

    AstarPath path;

    //We need to center our A* path finder and do a second scan so that it gets all the tables after they've been placed
    // Use this for initialization
    void Start()
    {
        path = GetComponent<AstarPath>();

        if (path)
        {
            path.transform.position = Camera.main.transform.position;
            path.graphs[0].Scan();
            
        }
    }
}
