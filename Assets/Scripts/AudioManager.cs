﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager s_instance;
    public AudioSource m_pop;

    void Awake()
        {
            if (s_instance == null) { s_instance = this; }
            else { print("Warning! Multiple AudioManager instances created!"); return; }
        }

    public void PlayPop()
    {
        if(m_pop != null) m_pop.Play();
    }
}