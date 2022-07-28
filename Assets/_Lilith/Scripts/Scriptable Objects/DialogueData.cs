using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public string m_line;
    public float m_duration;
    public AudioClip m_clip;

    public DialogueData() { }
    public DialogueData(string _line, float _duration, AudioClip clip)
    {
        m_line = _line;
        m_duration = _duration;
        m_clip = clip;
    }
    public void Init(string _line, float _duration, AudioClip clip)
    {
        m_line = _line;
        m_duration = _duration;
        m_clip = clip;
    }
}
