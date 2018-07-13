using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spill : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            Player player = other.gameObject.GetComponent<Player>();

            if (player)
            {
                player.ReduceSpeed();
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
                player.ResetSpeed();
            }
        }
    }

    private void OnMouseDown()
    {
        gameObject.GetComponentInParent<TileUnit>().MoustDoneIntercept();
    }
}
