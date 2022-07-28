using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class Subtitle : ControlledUpdateBehaviour
{
#pragma warning disable 0649
    [SerializeField] private float m_scale;
#pragma warning restore 0649

    private TMP_Text m_textDisplay;

    public float Scale => m_scale;
    public bool SubtitlesEnabled = true; //TODO: Get/set so this can be set from elsewhere

    protected override void ControlledAwake()
    {
        m_textDisplay = GetComponentInChildren<TMP_Text>();
        Clear();

        Game.EventBarn.m_onNewStoryText.Register(OnNewStoryText);
        Game.EventBarn.m_onDialogueEnded.Register(Clear);
    }

    private void OnDestroy()
    {
        Game.EventBarn.m_onNewStoryText.Unregister(OnNewStoryText);
        Game.EventBarn.m_onDialogueEnded.Unregister(Clear);
    }

    protected override void ControlledUpdate()
    {
        transform.localScale = Constants.Geometry.ONE_VECTOR * m_scale;
    }

    private void OnNewStoryText(DialogueData dialogue)
    {
        if (!SubtitlesEnabled)
        {
            return;
        }

        m_textDisplay.text = dialogue.m_line;
    }

    public void Clear()
    {
        m_textDisplay.text = string.Empty;
    }
}
