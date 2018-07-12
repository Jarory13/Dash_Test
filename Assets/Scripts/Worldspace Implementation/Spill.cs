using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spill : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();

            if (player)
            {
                player.normalSpeed = false;
                player.lerp.speed = player.speed / 3;
                Debug.Log("Player entered spill");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();

            if (player)
            {
                player.normalSpeed = true;
                player.lerp.speed = player.speed;
                Debug.Log("Player exited spill");
            }
        }
    }

    private void OnMouseDown()
    {
        gameObject.GetComponentInParent<TileUnit>().MoustDoneIntercept();
    }
}
