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
using TMPro;
using CommonTypes;

public class L51Script : BaseLight
{
    protected override void Start()
    {
        base.Start();

        if (mHand == VRHand.RIGHT)
        {
            mAmmoUi.transform.localPosition = new Vector3(-33.9f, 80f, -72.5f);
        }
        else if (mHand == VRHand.LEFT)
        {
            mAmmoUi.transform.localPosition = new Vector3(   50f, 80f, -72.5f);
        }
    }
}
