using UnityEngine;
#if UNITY_EDITOR
using UnityEngine.SceneManagement;
#endif

/**
 * This is meant to be a base class for classes that want to have their updates controlled.
 * It is achieved by setting the 'enabled' flag to false, bypassing the actual Unity updates.
 * All other messaging and Editor features should remain active. To avoid confusion, the
 * *Update() methods are renamed to:
 * - ControlledUpdate()
 * - ControlledFixedUpdate()
 * - ControlledLateUpdate()
 */
public abstract class ControlledUpdateBehaviour : MonoBehaviour
{
    protected bool m_hasCompletedInit = false;
    private bool m_startTriggered = false;

    /// <summary>
    /// DO NOT OVERRIDE. Override and use ControlledAwake instead.
    /// Calls ControlledAwake if Game.Instance exists or registers to call it when it is loaded.
    /// </summary>
    protected virtual void Awake()
    {
#if UNITY_EDITOR
        if (Game.Instance == null)
        {
            SceneManager.sceneLoaded += TriggerControlledAwake;
        }
        else
#endif
        {
            ControlledAwake();
        }
    }

    protected virtual void Start()
    {
        enabled = false;
    }

#if UNITY_EDITOR
    protected void Update()
    {
        // During save/load sometimes Update is scheduled even though enabled is false.
        // This is annoying, but ok, so just ignore it.
        if (!enabled || !m_hasCompletedInit)
        {
            return;
        }

        LogUpdateError("Update");
    }

    protected void FixedUpdate()
    {
        // At startup sometimes FixedUpdate is scheduled even though enabled is false.
        // This is annoying, but ok, so just ignore it.
        if (!enabled || !m_hasCompletedInit)
        {
            return;
        }

        LogUpdateError("FixedUpdate");
    }

    protected void LateUpdate()
    {
        if (!m_hasCompletedInit)
        {
            if (!enabled)
            {
                m_hasCompletedInit = true;
            }

            return;
        }

        LogUpdateError("LateUpdate");
    }

    private void LogUpdateError(string functionName)
    {
        Debug.LogErrorFormat(
            "The script {0} {1}() method is called by error! Did you call base.Start() in Start()?",
            this,
            functionName);
    }

    private void TriggerControlledAwake(Scene scene, LoadSceneMode mode)
    {

        SceneManager.sceneLoaded -= TriggerControlledAwake;

        ControlledAwake();
    }
#endif

    protected virtual void ControlledAwake() { }

    protected virtual void ControlledStart() { }

    public void TriggerControlledUpdate()
    {
        if (!gameObject.activeInHierarchy || Game.Instance == null)
        {
            return;
        }

        if (!m_startTriggered)
        {
            m_startTriggered = true;
            ControlledStart();
        }


        ControlledUpdate();
    }

    protected virtual void ControlledUpdate() { }

    public void TriggerControlledFixedUpdate()
    {
        if (!gameObject.activeInHierarchy || Game.Instance == null)
        {
            return;
        }

        ControlledFixedUpdate();
    }

    protected virtual void ControlledFixedUpdate() { }

    public void TriggerControlledLateUpdate()
    {
        if (!gameObject.activeInHierarchy || Game.Instance == null)
        {
            return;
        }

        ControlledLateUpdate();
    }

    protected virtual void ControlledLateUpdate() { }
}
