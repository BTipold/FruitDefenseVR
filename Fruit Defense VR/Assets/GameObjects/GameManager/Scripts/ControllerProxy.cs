// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using CommonTypes;
using System;

 
public class ControllerProxy : MonoBehaviour
{
    private static ControllerProxy mInstance = null;

    public static ControllerProxy Instance()
    {
        return mInstance;
    }

    public float mRotationalThreshold = 15f;
    public InputActionReference mTriggerRightReference = null;
    public InputActionReference mTriggerLeftReference = null;
    public InputActionReference mHandRightReference = null;
    public InputActionReference mHandLeftReference = null;
    public InputActionReference mAReference = null;
    public InputActionReference mBReference = null;
    public InputActionReference mXReference = null;
    public InputActionReference mYReference = null;
    public InputActionReference mMenuReference = null;
    public InputActionReference mRightControllerTrackingState = null;
    public InputActionReference mLeftControllerTrackingState = null;
    public InputActionReference mRightRotationalVelocity = null;
    public InputActionReference mLeftRotationalVelocity = null;
    public InputActionReference mRightCtrlRotation = null;
    public InputActionReference mLeftCtrlRotation = null;

    private HandedType<InputActionReference> mFire = new HandedType<InputActionReference>();
    private HandedType<InputActionReference> mHand = new HandedType<InputActionReference>();
    private HandedType<InputActionReference> mTop = new HandedType<InputActionReference>();
    private HandedType<InputActionReference> mBot = new HandedType<InputActionReference>();
    private HandedType<InputActionReference> mMenu = new HandedType<InputActionReference>();

    public void RegisterCallbackTriggerDown( VRHand hand, Action<InputAction.CallbackContext> cb ) { mFire.Get(hand).action.started += cb; }
    public void RegisterCallbackTriggerUp( VRHand hand, Action<InputAction.CallbackContext> cb ) { mFire.Get(hand).action.canceled += cb; }
    public void RegisterCallbackHandDown( VRHand hand, Action<InputAction.CallbackContext> cb ) { mHand.Get(hand).action.started += cb; }
    public void RegisterCallbackHandUp( VRHand hand, Action<InputAction.CallbackContext> cb ) { mHand.Get(hand).action.canceled += cb; }
    public void RegisterCallbackTopDown( VRHand hand, Action<InputAction.CallbackContext> cb ) { mTop.Get(hand).action.started += cb; }
    public void RegisterCallbackTopUp( VRHand hand, Action<InputAction.CallbackContext> cb ) { mTop.Get(hand).action.canceled += cb; }
    public void RegisterCallbackBotDown( VRHand hand, Action<InputAction.CallbackContext> cb ) { mBot.Get(hand).action.started += cb; }
    public void RegisterCallbackBotUp( VRHand hand, Action<InputAction.CallbackContext> cb ) { mBot.Get(hand).action.canceled += cb; }
    public void RegisterCallbackMenuDown( VRHand hand, Action<InputAction.CallbackContext> cb ) { mMenu.Get(hand).action.started += cb; }
    public void RegisterCallbackMenuUp( VRHand hand, Action<InputAction.CallbackContext> cb ) { mMenu.Get(hand).action.canceled += cb; }
    public void RemoveCallbackTriggerDown( VRHand hand, Action<InputAction.CallbackContext> cb ) { mFire.Get(hand).action.started -= cb; }
    public void RemoveCallbackTriggerUp( VRHand hand, Action<InputAction.CallbackContext> cb ) { mFire.Get(hand).action.canceled -= cb; }
    public void RemoveCallbackHandDown( VRHand hand, Action<InputAction.CallbackContext> cb ) { mHand.Get(hand).action.started -= cb; }
    public void RemoveCallbackHandUp( VRHand hand, Action<InputAction.CallbackContext> cb ) { mHand.Get(hand).action.canceled -= cb; }
    public void RemoveCallbackTopDown( VRHand hand, Action<InputAction.CallbackContext> cb ) { mTop.Get(hand).action.started -= cb; }
    public void RemoveCallbackTopUp( VRHand hand, Action<InputAction.CallbackContext> cb ) { mTop.Get(hand).action.canceled -= cb; }
    public void RemoveCallbackBotDown( VRHand hand, Action<InputAction.CallbackContext> cb ) { mBot.Get(hand).action.started -= cb; }
    public void RemoveCallbackBotUp( VRHand hand, Action<InputAction.CallbackContext> cb ) { mBot.Get(hand).action.canceled -= cb; }
    public void RemoveCallbackMenuDown( VRHand hand, Action<InputAction.CallbackContext> cb ) { mMenu.Get(hand).action.started -= cb; }
    public void RemoveCallbackMenuUp( VRHand hand, Action<InputAction.CallbackContext> cb ) { mMenu.Get(hand).action.canceled -= cb; }


    public void SetFireControl( ButtonType btn )
    {
        switch ( btn )
        {
            case ButtonType.TRIGGER:
                mFire.Set(VRHand.LEFT, mTriggerLeftReference);
                mFire.Set(VRHand.RIGHT, mTriggerRightReference);
                break;

            case ButtonType.HAND:
                mFire.Set(VRHand.LEFT, mHandLeftReference);
                mFire.Set(VRHand.RIGHT, mHandRightReference);
                break;

            case ButtonType.TOP:
                mFire.Set(VRHand.LEFT, mYReference);
                mFire.Set(VRHand.RIGHT, mBReference);
                break;

            case ButtonType.BOT:
                mFire.Set(VRHand.LEFT, mXReference);
                mFire.Set(VRHand.RIGHT, mAReference);
                break;

            case ButtonType.MENU:
                mFire.Set(VRHand.LEFT, mMenuReference);
                mFire.Set(VRHand.RIGHT, null);
                break;

            default:
                break;
        }
    }

    public void SetHandControl( ButtonType btn )
    {
        switch ( btn )
        {
            case ButtonType.TRIGGER:
                mHand.Set(VRHand.LEFT, mTriggerLeftReference);
                mHand.Set(VRHand.RIGHT, mTriggerRightReference);
                break;

            case ButtonType.HAND:
                mHand.Set(VRHand.LEFT, mHandLeftReference);
                mHand.Set(VRHand.RIGHT, mHandRightReference);
                break;

            case ButtonType.TOP:
                mHand.Set(VRHand.LEFT, mYReference);
                mHand.Set(VRHand.RIGHT, mBReference);
                break;

            case ButtonType.BOT:
                mHand.Set(VRHand.LEFT, mXReference);
                mHand.Set(VRHand.RIGHT, mAReference);
                break;

            case ButtonType.MENU:
                mHand.Set(VRHand.LEFT, mMenuReference);
                mHand.Set(VRHand.RIGHT, null);
                break;

            default:
                break;
        }
    }
    public void SetTopControl( ButtonType btn )
    {
        switch ( btn )
        {
            case ButtonType.TRIGGER:
                mTop.Set(VRHand.LEFT, mTriggerLeftReference);
                mTop.Set(VRHand.RIGHT, mTriggerRightReference);
                break;

            case ButtonType.HAND:
                mTop.Set(VRHand.LEFT, mHandLeftReference);
                mTop.Set(VRHand.RIGHT, mHandRightReference);
                break;

            case ButtonType.TOP:
                mTop.Set(VRHand.LEFT, mYReference);
                mTop.Set(VRHand.RIGHT, mBReference);
                break;

            case ButtonType.BOT:
                mTop.Set(VRHand.LEFT, mXReference);
                mTop.Set(VRHand.RIGHT, mAReference);
                break;

            case ButtonType.MENU:
                mTop.Set(VRHand.LEFT, mMenuReference);
                mTop.Set(VRHand.RIGHT, null);
                break;

            default:
                break;
        }
    }

    public void SetBotControl( ButtonType btn )
    {
        switch ( btn )
        {
            case ButtonType.TRIGGER:
                mBot.Set(VRHand.LEFT, mTriggerLeftReference);
                mBot.Set(VRHand.RIGHT, mTriggerRightReference);
                break;

            case ButtonType.HAND:
                mBot.Set(VRHand.LEFT, mHandLeftReference);
                mBot.Set(VRHand.RIGHT, mHandRightReference);
                break;

            case ButtonType.TOP:
                mBot.Set(VRHand.LEFT, mYReference);
                mBot.Set(VRHand.RIGHT, mBReference);
                break;

            case ButtonType.BOT:
                mBot.Set(VRHand.LEFT, mXReference);
                mBot.Set(VRHand.RIGHT, mAReference);
                break;

            case ButtonType.MENU:
                mBot.Set(VRHand.LEFT, mMenuReference);
                mBot.Set(VRHand.RIGHT, null);
                break;

            default:
                break;
        }
    }
    public void SetMenuControl( ButtonType btn )
    {
        switch ( btn )
        {
            case ButtonType.TRIGGER:
                mMenu.Set(VRHand.LEFT, mTriggerLeftReference);
                mMenu.Set(VRHand.RIGHT, mTriggerRightReference);
                break;

            case ButtonType.HAND:
                mMenu.Set(VRHand.LEFT, mHandLeftReference);
                mMenu.Set(VRHand.RIGHT, mHandRightReference);
                break;

            case ButtonType.TOP:
                mMenu.Set(VRHand.LEFT, mYReference);
                mMenu.Set(VRHand.RIGHT, mBReference);
                break;

            case ButtonType.BOT:
                mMenu.Set(VRHand.LEFT, mXReference);
                mMenu.Set(VRHand.RIGHT, mAReference);
                break;

            case ButtonType.MENU:
                mMenu.Set(VRHand.LEFT, mMenuReference);
                mMenu.Set(VRHand.RIGHT, null);
                break;

            default:
                break;
        }
    }



    private void Awake()
    {
        mInstance = this;

        // Setting fallbacks/defaults. To be overriden by settings.
        mFire.Set(VRHand.LEFT, mTriggerLeftReference);
        mFire.Set(VRHand.RIGHT, mTriggerRightReference);
        mHand.Set(VRHand.LEFT, mHandLeftReference);
        mHand.Set(VRHand.RIGHT, mHandRightReference);
        mTop.Set(VRHand.LEFT, mYReference);
        mTop.Set(VRHand.RIGHT, mBReference);
        mBot.Set(VRHand.LEFT, mXReference);
        mBot.Set(VRHand.RIGHT, mAReference);
        mMenu.Set(VRHand.LEFT, mMenuReference);
        mMenu.Set(VRHand.RIGHT, null);

        mLeftControllerTrackingState.action.performed += LeftControllerStateChanged;
        mLeftControllerTrackingState.action.canceled += LeftControllerStateChanged;
        mRightControllerTrackingState.action.performed += RightControllerStateChanged;
        mRightControllerTrackingState.action.canceled += RightControllerStateChanged;
    }

    private void LeftControllerStateChanged( InputAction.CallbackContext context )
    {
        ControllerState controllerState = (ControllerState)context.ReadValue<int>();
        Debug.Log("left controller state: " + controllerState.ToString());
        switch ( controllerState )
        {
            case ControllerState.NOT_CONNECTED:
                EventControllerOffline eOffline = new EventControllerOffline(VRHand.LEFT);
                Events.Instance.Raise(eOffline);
                break;
            case ControllerState.CONNECTED:
                EventControllerOnline eOnline = new EventControllerOnline(VRHand.LEFT);
                Events.Instance.Raise(eOnline);
                break;
            case ControllerState.CONNECTING:
            default:
                break;
        }
    }

    private void RightControllerStateChanged( InputAction.CallbackContext context )
    {
        ControllerState controllerState = (ControllerState)context.ReadValue<int>();
        Debug.Log("right controller state: " + controllerState.ToString());
        switch (controllerState)
        {
            case ControllerState.NOT_CONNECTED:
                EventControllerOffline eOffline = new EventControllerOffline( VRHand.RIGHT);
                Events.Instance.Raise(eOffline);
                break;
            case ControllerState.CONNECTED:
                EventControllerOnline eOnline = new EventControllerOnline( VRHand.RIGHT);
                Events.Instance.Raise(eOnline);
                break;
            case ControllerState.CONNECTING:
            default:
                break;
        }
    }

    public void Update()
    {
        Vector3 angVel = mLeftRotationalVelocity.action.ReadValue<Vector3>();
        Quaternion deviceRot = mLeftCtrlRotation.action.ReadValue<Quaternion>();
        Vector3 correctedVel = Quaternion.Inverse(deviceRot) * angVel;
        var angularVel = Application.isEditor ? angVel : correctedVel;

        if ( Math.Abs(angularVel.x) > mRotationalThreshold ) {
            EventAngularVelocity e = new EventAngularVelocity(VRHand.LEFT, Axis.X, angularVel);
            Events.Instance.Raise(e);
        } else if (Math.Abs(angularVel.y) > mRotationalThreshold ) {
            EventAngularVelocity e = new EventAngularVelocity(VRHand.LEFT, Axis.Y, angularVel);
            Events.Instance.Raise(e);
        } else if (Math.Abs(angularVel.z) > mRotationalThreshold ) {
            EventAngularVelocity e = new EventAngularVelocity(VRHand.LEFT, Axis.Z, angularVel);
            Events.Instance.Raise(e);
        }

        angVel = mRightRotationalVelocity.action.ReadValue<Vector3>();
        deviceRot = mRightCtrlRotation.action.ReadValue<Quaternion>();
        correctedVel = Quaternion.Inverse(deviceRot) * angVel;
        angularVel = Application.isEditor ? angVel : correctedVel;
        if ( Math.Abs(angularVel.x) > mRotationalThreshold ) {
            EventAngularVelocity e = new EventAngularVelocity(VRHand.RIGHT, Axis.X, angularVel);
            Events.Instance.Raise(e);
        } else if (Math.Abs(angularVel.y) > mRotationalThreshold ) {
            EventAngularVelocity e = new EventAngularVelocity(VRHand.RIGHT, Axis.Y, angularVel);
            Events.Instance.Raise(e);
        } else if (Math.Abs(angularVel.z) > mRotationalThreshold ) {
            EventAngularVelocity e = new EventAngularVelocity(VRHand.RIGHT, Axis.Z, angularVel);
            Events.Instance.Raise(e);
        }
    }
}
