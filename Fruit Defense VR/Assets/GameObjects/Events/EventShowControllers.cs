// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


public class EventShowControllers : GameEvent
{
    // ---------------------------------------------------------------
    // Member Variables
    // ---------------------------------------------------------------
    public bool showControllers = false;

    // ---------------------------------------------------------------
    // @Summary: constructor
    // @Param: showControllers - if true, controllers should be 
    //   enabled. If false, controllers should be disabled.
    // @Return: void
    // ---------------------------------------------------------------
    public EventShowControllers( bool showControllers )
    {
        this.showControllers = showControllers;
    }
}
