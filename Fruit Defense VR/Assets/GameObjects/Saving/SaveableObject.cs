// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using System;


public abstract class SaveableObject
{
    protected string mKey = null;
    private Action mOnLoadFailed;
    private Action mOnLoad;

    // ---------------------------------------------------------------
    // @Summary: Constructor
    // @Param: key - key used to save and retrieve the object
    // @Return: void
    // ---------------------------------------------------------------
    protected SaveableObject(string key) { mKey = key; }

    // ---------------------------------------------------------------
    // @Summary: returns the true if underlying data is null
    // @Return: bool - returns null
    // ---------------------------------------------------------------
    public abstract bool IsDataNull();

    // ---------------------------------------------------------------
    // @Summary: returns the key as a string
    // @Return: string - returns key
    // ---------------------------------------------------------------
    public string GetKey() { return mKey; }

    // ---------------------------------------------------------------
    // @Summary: to be overriden to provide conversion from datatype 
    //   to byte stream
    // @Return: byte[] - returns stream of bytes 
    // ---------------------------------------------------------------
    public abstract byte[] SerializeData();

    // ---------------------------------------------------------------
    // @Summary: to be overriden to provide conversion from byte 
    //   stream to original data type
    // @Param: data - array of bytes from save file
    // @Return: void
    // ---------------------------------------------------------------
    public abstract void UnSerializeData(byte[] data);

    // ---------------------------------------------------------------
    // @Summary: Register load failed callback function
    // @Param: Action - void (void) function ptr
    // @Return: void
    // ---------------------------------------------------------------
    public void RegisterLoadFailedHandler(Action action)
    {
        mOnLoadFailed = action;
    }

    // ---------------------------------------------------------------
    // @Summary: Register on load callback function
    // @Param: Action - void (void) function ptr
    // @Return: void
    // ---------------------------------------------------------------
    public void RegisterOnLoadCallback(Action action)
    {
        mOnLoad = action;
    }

    // ---------------------------------------------------------------
    // @Summary: Called by savebackend when object fails to load
    // @Return: void
    // ---------------------------------------------------------------
    public void OnLoadFailed()
    {
        if (mOnLoadFailed != null)
        {
            mOnLoadFailed();
        }
    }

    // ---------------------------------------------------------------
    // @Summary: Called by savebackend when object is loaded
    // @Return: void
    // ---------------------------------------------------------------
    public void OnLoad()
    {
        if (mOnLoad != null)
        {
            mOnLoad();
        }
    }
}
