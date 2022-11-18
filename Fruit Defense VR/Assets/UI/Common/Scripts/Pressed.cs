// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonTypes;

public class Pressed : MonoBehaviour
{
    public AudioClip Clip;
    public AudioSource source;

    void OnEnable()
    {
        if ( Clip != null )
        {
            source.PlayOneShot(Clip);
        }
        if (source != null)
        {
            //ControllerProxy.SendVibration( VRHand.RIGHT, new Vibration( Clip ) );
        }
    }

}
