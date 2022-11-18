// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios.
// ---------------------------------------------------------------


using UnityEngine;
using CommonTypes;


public class Settings : MonoBehaviour
{
    private static Settings mInstance = null;
    public static Settings Instance() { return mInstance; }

    private PersistentDataType<bool> mSaveableHintsBool = null;
    private PersistentDataType<float> mSaveableSoundVolume = null;
    private PersistentDataType<float> mSaveableMusicVolume = null;
    private PersistentDataType<ButtonType> mSaveableFireBtn = null;
    private PersistentDataType<ButtonType> mSaveableHandBtn = null;
    private PersistentDataType<ButtonType> mSaveableTopBtn = null;
    private PersistentDataType<ButtonType> mSaveableBotBtn = null;
    private PersistentDataType<bool> mSaveableMirrorOffset = null;
    private PersistentDataType<SerializableVector3> mSaveableRightControllerOffset = null;
    private PersistentDataType<SerializableVector3> mSaveableLeftControllerOffset = null;
    private PersistentDataType<SerializableVector3> mSaveableRightControllerRotation = null;
    private PersistentDataType<SerializableVector3> mSaveableLeftControllerRotation = null;
    private PersistentDataType<float> mSaveableTriggerThreshold = null;

    private SerializableVector3 mLControllerOffset = new SerializableVector3(0,0,0);
    private SerializableVector3 mRControllerOffset = new SerializableVector3(0,0,0);
    private SerializableVector3 mLControllerRotation = new SerializableVector3(0,0,0);
    private SerializableVector3 mRControllerRotation = new SerializableVector3(0,0,0);

    private ControllerProxy mControllerProxy = null;

    private const string SETTINGS_DATABASE_ID = "settings"; 

    private void Awake()
    {
        mInstance = this;
        mSaveableHintsBool = new PersistentDataType<bool>(true, "hintsenabled");
        mSaveableSoundVolume = new PersistentDataType<float>(1.0f, "soundvolume");
        mSaveableMusicVolume = new PersistentDataType<float>(1.0f, "musicvolume");
        mSaveableFireBtn = new PersistentDataType<ButtonType>(ButtonType.TRIGGER, "firebtn");
        mSaveableHandBtn = new PersistentDataType<ButtonType>(ButtonType.HAND, "handbtn");
        mSaveableTopBtn = new PersistentDataType<ButtonType>(ButtonType.TOP, "topbtn");
        mSaveableBotBtn = new PersistentDataType<ButtonType>(ButtonType.BOT, "botbtn");
        mSaveableMirrorOffset = new PersistentDataType<bool>(true, "mirrorcontrolleroffset");
        mSaveableRightControllerOffset = new PersistentDataType<SerializableVector3>(mRControllerOffset, "rightcontrolleroffset");
        mSaveableLeftControllerOffset = new PersistentDataType<SerializableVector3>(mLControllerOffset, "leftcontrolleroffset");
        mSaveableRightControllerRotation = new PersistentDataType<SerializableVector3>(mRControllerRotation, "rightcontrollerrotation");
        mSaveableLeftControllerRotation = new PersistentDataType<SerializableVector3>(mLControllerRotation, "leftcontrollerrotation");
        mSaveableTriggerThreshold = new PersistentDataType<float>(0.4f, "triggerthreshold");
    }

    private void Start()
    {
        mControllerProxy = ControllerProxy.Instance();
    }

    // General settings
    public void SetHintsEnabled(bool enabled)
    {
        mSaveableHintsBool.Assign(enabled);
        mSaveableHintsBool.Save(SETTINGS_DATABASE_ID);
    }

    public bool GetHintsEnabled()
    {
        mSaveableHintsBool.Load(SETTINGS_DATABASE_ID);
        return mSaveableHintsBool;
    }

    public void SetSoundVolume(float volume)
    {
        mSaveableSoundVolume.Assign(volume);
        mSaveableSoundVolume.Save(SETTINGS_DATABASE_ID);
    }

    public float GetSoundVolume()
    {
        mSaveableSoundVolume.Load(SETTINGS_DATABASE_ID);
        return mSaveableSoundVolume;
    }

    public void SetMusicVolume(float volume)
    {
        mSaveableMusicVolume.Assign(volume);
        mSaveableMusicVolume.Save(SETTINGS_DATABASE_ID);
    }

    public float GetMusicVolume()
    {
        mSaveableMusicVolume.Load(SETTINGS_DATABASE_ID);
        return mSaveableMusicVolume;
    }

    // Controller settings
    public void SetFireBtn(ButtonType btn)
    {
        mSaveableFireBtn.Assign(btn);
        mSaveableFireBtn.Save(SETTINGS_DATABASE_ID);
        mControllerProxy.SetFireControl(btn);
    }

    public ButtonType GetFireBtn()
    {
        mSaveableFireBtn.Load(SETTINGS_DATABASE_ID);
        return mSaveableFireBtn;
    }

    public void SetHandBtn(ButtonType btn)
    {
        mSaveableHandBtn.Assign(btn);
        mSaveableHandBtn.Save(SETTINGS_DATABASE_ID);
        mControllerProxy.SetHandControl(btn);
    }

    public ButtonType GetHandBtn()
    {
        mSaveableHandBtn.Load(SETTINGS_DATABASE_ID);
        return mSaveableHandBtn;
    }

    public void SetTopBtn(ButtonType btn)
    {
        mSaveableTopBtn.Assign(btn);
        mSaveableTopBtn.Save(SETTINGS_DATABASE_ID);
        mControllerProxy.SetTopControl(btn);
    }

    public ButtonType GetTopBtn()
    {
        mSaveableTopBtn.Load(SETTINGS_DATABASE_ID);
        return mSaveableTopBtn;
    }

    public void SetBotBtn(ButtonType btn)
    {
        mSaveableBotBtn.Assign(btn);
        mSaveableBotBtn.Save(SETTINGS_DATABASE_ID);
        mControllerProxy.SetBotControl(btn);
    }

    public ButtonType GetBotBtn()
    {
        mSaveableBotBtn.Load(SETTINGS_DATABASE_ID);
        return mSaveableBotBtn;
    }

    public void SetMirrorOffset(bool mirror)
    {
        mSaveableMirrorOffset.Assign(mirror);
        mSaveableMirrorOffset.Save(SETTINGS_DATABASE_ID);
    }

    public bool GetMirrorOffset()
    {
        mSaveableMirrorOffset.Load(SETTINGS_DATABASE_ID);
        return mSaveableMirrorOffset;
    }

    public void SetControllerOffset(VRHand hand, Vector3 offset)
    {
        if (hand == VRHand.LEFT)
        {
            mSaveableLeftControllerOffset.Assign(offset);
            mSaveableLeftControllerOffset.Save(SETTINGS_DATABASE_ID);
        }

        else if (hand == VRHand.RIGHT)
        {
            mSaveableRightControllerOffset.Assign(offset);
            mSaveableRightControllerOffset.Save(SETTINGS_DATABASE_ID);
        }
    }

    public SerializableVector3 GetControllerOffset(VRHand hand)
    {
        mSaveableLeftControllerOffset.Load(SETTINGS_DATABASE_ID);
        mSaveableRightControllerOffset.Load(SETTINGS_DATABASE_ID);

        if (hand == VRHand.LEFT) return mSaveableLeftControllerOffset;
        else if (hand == VRHand.RIGHT) return mSaveableRightControllerOffset;
        else return new Vector3(0, 0, 0);
    }

    public void SetControllerRotation(VRHand hand, Vector3 offset)
    {
        if (hand == VRHand.LEFT)
        {
            mSaveableLeftControllerRotation.Assign(offset);
            mSaveableLeftControllerRotation.Save(SETTINGS_DATABASE_ID);
        }

        else if (hand == VRHand.RIGHT)
        {
            mSaveableRightControllerRotation.Assign(offset);
            mSaveableRightControllerRotation.Save(SETTINGS_DATABASE_ID);
        }
    }

    public SerializableVector3 GetControllerRotation(VRHand hand)
    {
        mSaveableLeftControllerRotation.Load(SETTINGS_DATABASE_ID);
        mSaveableRightControllerRotation.Load(SETTINGS_DATABASE_ID);

        if (hand == VRHand.LEFT) return mSaveableLeftControllerRotation;
        else if (hand == VRHand.RIGHT) return mSaveableRightControllerRotation;
        else return new Vector3(0, 0, 0);
    }

    public void SetTriggerPoint(float triggerPoint)
    {
        mSaveableTriggerThreshold.Assign(triggerPoint);
        mSaveableTriggerThreshold.Save(SETTINGS_DATABASE_ID);
    }

    public float GetTriggerPoint()
    {
        mSaveableTriggerThreshold.Load(SETTINGS_DATABASE_ID);
        return mSaveableTriggerThreshold;
    }
}                              
