using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager s_instance;
    public AudioSource m_pop;
    public AudioSource m_hurt;

    void Awake()
        {
            if (s_instance == null) { s_instance = this; }
            else { print("Warning! Multiple AudioManager instances created!"); return; }
        }

    public void PlayPop()
    {
        if(m_pop != null) m_pop.Play();
    }

    public void PlayHurt()
    {
        if(m_hurt != null) m_hurt.Play();
    }
}
