// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using CommonTypes;


public class EventStateChange : GameEvent
{
    public EventStateChange( State newState, State prevState )
    {
        this.newState = newState;
        this.prevState = prevState;

    }

    public State newState = State.UNKNOWN;
    public State prevState = State.UNKNOWN;
}
