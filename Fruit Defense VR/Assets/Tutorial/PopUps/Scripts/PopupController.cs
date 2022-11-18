// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


public class PopupController : MonoBehaviour
{
    private List<Action> mOnDestroyFuncs = new List<Action>(); // Call these before destroying 
    private List<Func<bool>> mDestructionRegulations = new List<Func<bool>>(); 
    private Func<bool> mDestroyWhenFunc = null;
    private bool mAlreadyDestroyed = false;

    public Animator mExitAnimation = null;
    public bool mDestructionPending = false;

    // -----------------------------------------------------------
    // @Summary: sets the animation which will play on 
    //   destruction.
    // @Param: a - animator component.
    // @Return: void. 
    // -----------------------------------------------------------
    public void SetExitAnim( Animator a )
    {
        mExitAnimation = a;
    }

    // -----------------------------------------------------------
    // @Summary: sets the lambda which will be called when this 
    //   object is destroyed.
    // @Param: lambda - function that will be called. This lambda
    //   needs the caller to fill its "waitUntil" signal by 
    //   assigning it to a second lambda. When the object 
    //   destruction is requsted, it will wait until the signal
    //   is TRUE. 
    // @Return: void.
    // -----------------------------------------------------------
    public void DoOnDestroy(Action lambda, Func<bool> waitForConditional = null)
    {
        mOnDestroyFuncs.Add(lambda);
        if (waitForConditional != null)
        {
            mDestructionRegulations.Add(waitForConditional);
        }
    }

    // -----------------------------------------------------------
    // @Summary: sets the lambda which will be called when this 
    //   object is destroyed.
    // @Param: lambda - function that will be called.
    // @Return: void.
    // -----------------------------------------------------------
    public void DestroyWhen( Func<bool> lambda )
    {
        mDestroyWhenFunc = lambda;
    }

    // -----------------------------------------------------------
    // @Summary: destroys the object after t seconds.
    // @Param: t - number of seconds to wait before calling 
    //   destroy.
    // @Return: void.
    // -----------------------------------------------------------
    public void DestroyAfter( float t )
    {
        IEnumerator f()
        {
            yield return new WaitForSeconds( t );
            Destroy();
        }
        StartCoroutine( f() );
    }

    // -----------------------------------------------------------
    // @Summary: This function performs a series of steps:
    //   1. Call each onDestory callback.
    //   2. Wait for each dontDestroyUntilSignal to be TRUE.
    //   3. Destroy the game object.
    // @Return: void.
    // -----------------------------------------------------------
    public void Destroy()
    {
        foreach(Action doOnDestroy in mOnDestroyFuncs )
        {
            doOnDestroy(); 
        }
        mAlreadyDestroyed = true;
        IEnumerator destroyAfterConditions()
        {
            yield return new WaitUntil( () =>
            {
                // Wait until all the signals are true
                bool status = true;
                mDestructionRegulations.ForEach( (sig) => status &= sig() ); 
                return status;
            } );

            Debug.Log("Destroying popup.");
            Destroy( this.gameObject );
        }
        StartCoroutine(destroyAfterConditions() ); 
    }

    private void OnDisable()
    {
        if (!mAlreadyDestroyed)
        {
            mOnDestroyFuncs.ForEach((f) => f());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (mDestroyWhenFunc != null 
            && mDestroyWhenFunc()
            && !mDestructionPending)
        {
            mDestructionPending = true;
            this.Destroy();
        }
    }
}
