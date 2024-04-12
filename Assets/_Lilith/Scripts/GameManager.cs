using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public enum PickUpObjects
    {
        Default,
        Star,
        Flower
    }

    public bool isPickupStar = false;
    public bool isPickupFlower = false;


    private void Awake()
    {
        instance = this;
    }

    public void ObjectPickedUp(PickUpObjects pickup)
    {
        switch (pickup)
        {
            case PickUpObjects.Star:
                isPickupStar = true;
                break;
            case PickUpObjects.Flower:
                isPickupFlower = true;
                break;
            case PickUpObjects.Default:
                break;
            default:
                break;
        }
    }
}
