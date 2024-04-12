using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : ControlledUpdateBehaviour
{
#pragma warning disable 0649
    [SerializeField] private float m_volume = 1.0f;    //TODO: Will be removed and replaced with Fmod stuff
#pragma warning restore 0649

    private AudioSource m_audioSource;

    protected override void ControlledAwake()
    {
        m_audioSource = GetComponentInChildren<AudioSource>();
        StopAudioSource();

        Game.EventBarn.m_onNewStoryText.Register(PlayDialogue);
        Game.EventBarn.m_onDialogueEnded.Register(StopAudioSource);
    }

    private void OnDestroy()
    {
        Game.EventBarn.m_onNewStoryText.Unregister(PlayDialogue);
        Game.EventBarn.m_onDialogueEnded.Unregister(StopAudioSource);
    }

    private void PlayDialogue(DialogueData dialogue)
    {
        if(dialogue == null || dialogue.m_clip == null)
        {
            return;
        }

        //Will probably be replaced with Fmod stuff
        m_audioSource.clip = dialogue.m_clip;
        m_audioSource.volume = m_volume;
        m_audioSource.loop = false;
        m_audioSource.Play();
    }

    public void StopAudioSource()
    {
        m_audioSource.Stop();
    }
}
