using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AStarReposition : MonoBehaviour
{

    AstarPath path;

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
