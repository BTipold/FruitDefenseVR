// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public struct Sound
{
    public  string      name;
    public  AudioClip   clip;
    public  float       volume;
    public  float       pitch;
}

public sealed class ManageAudio : MonoBehaviour
{
    public  Sound[] m_sounds_input;
    private Dictionary<string, AudioSource> m_sounds_map = new Dictionary<string, AudioSource>();

    private static ManageAudio m_instance = null;
    public static ManageAudio inst() 
    {
        return m_instance;
    }

    public bool play( string sound_name )
    {
        bool status = true;

        if ( m_sounds_map.ContainsKey( sound_name ) )
        {
            m_sounds_map[sound_name].Play();
        }
        else
        {
            status = false;
        }

        return status;
    }

    private void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
    }

    void Start()
    {
        foreach ( Sound s in m_sounds_input )
        {
            AudioSource sound = this.gameObject.AddComponent<AudioSource>();
            sound.clip      = s.clip;
            sound.pitch     = s.pitch;
            sound.volume    = s.volume;
            m_sounds_map.Add( s.name, sound );
        }
    }

    void Update()
    {
        
    }
}
