using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using Ink.Runtime;
using UnityEditor;

public class KnockTrigger : MonoBehaviour
{
    private ScriptMachine _scriptMachine;
    private static bool m_beginKnock = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!m_beginKnock)
        {
            Game.EventBarn.m_onKnockAvailable.Dispatch(true);
            m_beginKnock = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Game.EventBarn.m_onKnockAvailable.Dispatch(false);
    }
}
