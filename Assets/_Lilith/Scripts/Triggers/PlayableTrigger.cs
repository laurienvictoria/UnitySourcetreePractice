using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayableTrigger : MonoBehaviour
{
    [Header("Sequence Setup")]
    public PlayableDirector playableDirector;
    public bool playOnce = true;
    public bool lookAtCheck = false;

    private bool isPlayed= false;
    private bool inTrigger = false;


    public void PlayerEnteredTrigger()
    {
        if (!lookAtCheck)
            PlaySequence();
        inTrigger = true;
    }

    public void PlayerExitedTrigger()
    {
        inTrigger = false;
    }

    /*private void Update()
    {
        if (lookAtCheck && inTrigger)
        {
            CheckLookAt();
        }
    }

    public GameObject player;
     Camera cam;
     float dist = 5;
     //public LayerMask layerMask;
     int layerMask = 1 << 6;
     Vector3 pos = new Vector3(0.5f,0.5f,0);
     public GameObject col;
     private void CheckLookAt()
     {
         if (Camera.main != null)
         {
             Ray ray = Camera.main.ScreenPointToRay(pos);

             if (Physics.Raycast(ray, out RaycastHit hit2))
             {
                 print(hit2.collider.GetComponent<GameObject>());
                // Transform objectHit = hit.transform;

             }
         }


         //check player raycast.
         if(Physics.Raycast(player.transform.position, Vector3.forward, out RaycastHit hit, Mathf.Infinity, layerMask))
         {
             Debug.DrawRay(player.transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
             print("hit!");
             PlaySequence();
         }
         else
         {
             print("did not hit");
         }*/


    private void PlaySequence()
    {
        if(!playOnce)
        {
            if (playableDirector != null)
                playableDirector.Play();
        }

        else
        {
            if (!isPlayed)
            {
                if (playableDirector != null)
                    playableDirector.Play();
                isPlayed = true;
            }
        }
    }
}
