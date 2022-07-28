using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ZoneVisualsTrigger : MonoBehaviour
{
    public VisualEffect[] visualEffects;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            for (int i = 0; i < visualEffects.Length; i++)
            {
                visualEffects[i].SendEvent("OnStart");
            }
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            for (int i = 0; i < visualEffects.Length; i++)
            {
                visualEffects[i].SendEvent("OnStop");
            }
        }
    }
}
