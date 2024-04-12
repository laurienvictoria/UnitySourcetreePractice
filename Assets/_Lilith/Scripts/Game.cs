using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    private EventBarn m_eventBarn;

    private static Game s_instance;
    public static Game Instance => s_instance;

    public static EventBarn EventBarn => Instance?.m_eventBarn;

    public void Awake()
    {
        s_instance = this;
        m_eventBarn = GetComponent<EventBarn>();

        m_eventBarn.Initialize();
    }

    protected void Update()
    {
        m_eventBarn.TriggerControlledUpdate();
    }

    protected void FixedUpdate()
    {
        m_eventBarn.TriggerControlledFixedUpdate();
    }

    protected void LateUpdate()
    {
        m_eventBarn.TriggerControlledLateUpdate();
    }
}
