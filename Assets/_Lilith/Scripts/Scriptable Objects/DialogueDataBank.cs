using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DialogueDataBank", menuName = "Lilith/DataBanks/DialogueDataBank", order = 1)]
public class DialogueDataBank : ScriptableObject {
    [System.Serializable] 
    public class DialogueDictionary : SerializableDictionary<string, DialogueData> 
    {

    }

    [SerializeField] public DialogueDictionary lineDictionary;
}
