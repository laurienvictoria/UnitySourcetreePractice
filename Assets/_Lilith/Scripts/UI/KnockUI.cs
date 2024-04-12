using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.UI;

public class KnockUI : ControlledUpdateBehaviour
{

    private GameObject m_knockButton;
    protected override void ControlledAwake()
    {
        m_knockButton = GetComponentInChildren<Button>(true).gameObject;

        Game.EventBarn.m_onKnockAvailable.Register(ToggleKnockUI);
    }

    private void OnDestroy()
    {
        Game.EventBarn.m_onKnockAvailable.Unregister(ToggleKnockUI);
    }

    private void ToggleKnockUI(bool isEnabled)
    {
        m_knockButton.SetActive(isEnabled);
    }
    public void TriggerKnock()
    {
        EventBus.Trigger(Constants.Events.ON_KNOCK_EVENT_NAME, 1);
        Game.EventBarn.m_onKnockAvailable.Dispatch(false);
    }
}
