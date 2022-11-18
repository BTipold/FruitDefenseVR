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

public class VibrationManager : MonoBehaviour
{
    public static VibrationManager vib_singleton;

    private void Start()
    {
        if(vib_singleton && vib_singleton != this)
        {
            Destroy(this);
        }
        else
        {
            vib_singleton = this;
        } 
    }

    public void TriggerHaptic(AudioClip vibrationAudio, OVRInput.Controller controller)
    {
        OVRHapticsClip hapticsClip = new OVRHapticsClip(vibrationAudio);
         
        if (controller == OVRInput.Controller.LTouch)
        {
            //OVRHaptics.LeftChannel.Preempt(hapticsClip);
        }
        else if (controller == OVRInput.Controller.RTouch)
        {
            //OVRHaptics.RightChannel.Preempt(hapticsClip);
        }
    }
}
