using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New House", menuName = "Lilith/DataBanks/House Data")]
public class HouseData : ScriptableObject
{
    [SerializeField] public string houseName;
}
