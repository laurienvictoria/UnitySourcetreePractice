using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderCheck : MonoBehaviour
{
    [HideInInspector] public bool isInTrigger = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isInTrigger = true;
            SendMessageUpwards("PlayerEnteredTrigger", SendMessageOptions.DontRequireReceiver);
        }
        //send event
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isInTrigger = false;
            SendMessageUpwards("PlayerExitedTrigger", SendMessageOptions.DontRequireReceiver);
        }
    }
}
