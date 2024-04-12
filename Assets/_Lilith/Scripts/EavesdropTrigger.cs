using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using Ink.Runtime;
using UnityEditor;

public class EavesdropTrigger : MonoBehaviour
{
    private ScriptMachine _scriptMachine;
    private static bool m_beginEavesdrop = false;

    [SerializeField]
    private string triggerID;

    private void OnTriggerEnter(Collider other)
    {
        if(triggerID == null)
        {
            return;
        }

        if(!m_beginEavesdrop)
        {
            EventBus.Trigger(Constants.Events.ON_EAVESDROP_EVENT_NAME, triggerID);
            m_beginEavesdrop = true;
        }
    }
}
