using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;

public class EventBarn : ControlledUpdateBehaviour
{
    public readonly GameEvent<DialogueData> m_onNewStoryText = new();
    public readonly GameEvent<bool> m_onKnockAvailable = new();
    public readonly GameEvent m_onDialogueEnded = new();

    public void Initialize()
    {
        
    }
}
