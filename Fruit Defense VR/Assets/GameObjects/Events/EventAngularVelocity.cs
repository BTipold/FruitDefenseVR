// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using UnityEngine;
using CommonTypes;


public class EventAngularVelocity : GameEvent
{
    public EventAngularVelocity( VRHand hand, Axis axis, Vector3 values)
    {
        this.hand = hand;
        this.axis = axis;
        this.values = values;
    }

    public VRHand hand = VRHand.UNKNOWN;
    public Vector3 values = new Vector3();
    public Axis axis = Axis.UNKNOWN;
}
