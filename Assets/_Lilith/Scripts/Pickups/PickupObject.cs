using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

public class PickupObject : MonoBehaviour
{
    public GameManager.PickUpObjects pickupObject;
    GameManager gameManager;
    private PlayableDirector playableDirector;
    ColliderCheck colCheck;

    DefaultInputMapping defaultMapping;

    private void Awake()
    {
        defaultMapping = new DefaultInputMapping();
    }
    private void OnEnable()
    {
        defaultMapping.Enable();
    }
    private void Start()
    {
        gameManager = GameManager.instance;
        playableDirector = GetComponent<PlayableDirector>();
        colCheck = GetComponentInChildren<ColliderCheck>();
    }

    private void OnClicked()
    {
        playableDirector.Play();
        gameManager.ObjectPickedUp(pickupObject);

    }

    private void InputReceived()
    {
        if (colCheck.isInTrigger)
        {
            OnClicked();
        }
    }

    private void Update()
    {
        if (defaultMapping.Player.Interact.triggered)
        {
            InputReceived();
        }
    }

}
