// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using CommonTypes;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public abstract class BaseLight : BaseWeapon
{
    // ---------------------------------------------------------------
    // Member Variables
    // ---------------------------------------------------------------
    public uint mPierce = 0U;
    public float mRange = 0F;
    public float mHitMagnitude = 1F;
    public float mGunshotVolume = 1F;
    public FireType mFireType = FireType.UNKNOWN;
    public TextMeshProUGUI mAmmoUi = null;
    public Transform mRayEmitter = null;
    public AudioClip mFireSound = null;
    public AudioClip mDryfireSound = null;
    public AudioClip mMagEjectSound = null;
    public AudioClip mMagInsertSound = null;
    public ParticleSystem mPrMuzzleFlash = null;
    public Transform mMagazineParent = null;
    public GameObject mMagazinePrefab = null;
    public Vibration.VibrationPreset mVibration;

    protected GameObject mMagazineObject = null;
    protected Vibration mHapticsClip = null;
    protected Animator mAnimator = null;
    protected AudioSource mAudioSource = new AudioSource();
    protected float mFiringDelay = 0F;
    protected float mLastFired = 0F;
    protected uint mAmmo = 0U;
    protected uint mShotsInBurst = 0U;
    protected uint mMaxBurstSize = 0U;
    protected bool mReloading = false;
    protected bool mFireBtnDown = false;
    protected bool mBurstComplete = false;
    protected bool mFullyEmpty = false;
    protected bool mListeningForReloadGesture = false;

    // ---------------------------------------------------------------
    // @Summary: returns current ammo count
    // @Return: uint - returns ammo count
    // ---------------------------------------------------------------
    public uint GetAmmo() { return mAmmo; }

    // ---------------------------------------------------------------
    // @Summary: checks if player is reloading
    // @Return: bool - true if player is reloading
    // ---------------------------------------------------------------
    public bool IsReloading() { return mReloading; }

    // ---------------------------------------------------------------
    // @Summary: Function to check if the gun has a laser sight. 
    // @Return: bool - true if gun has a laser already. 
    // ---------------------------------------------------------------
    public bool HasWeaponLaser()
    {
        List<LineRenderer> l = new List<LineRenderer> ();
        return Extension.GetActiveComponentsInChildrenRecursively(gameObject.transform, l).Count > 0;
    }

    // ---------------------------------------------------------------
    // @Summary: shoot executes functions to make the gun shoot. 
    //   This function can be overriden by children if they need 
    //   different behaviour.
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void Shoot()
    {
        MuzzleFlash();
        PlayFireSound();
        Animate();
        //ControllerProxy.SendVibration( mHand, mHapticsClip );
        ShootingMechanics();
    }

    // ---------------------------------------------------------------
    // @Summary: shooting mechanics defines how the raycast works.
    //   Override this function if different shooting mechanics are
    //   required.
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void ShootingMechanics()
    {
        // Raycast physics
        RaycastHit[] hits = Physics.RaycastAll( mRayEmitter.position,
            mRayEmitter.forward, mRange, Utils.IgnoreBitmask("Ignore Raycast") );

        uint hitCount = 0;
        foreach ( RaycastHit hit in hits )
        {
            // If this weapon has the level 1 hit enhancement, increase the pierce by 1.
            uint pierce = mPierce;
            if ( mPlayer.HasEnhancement( mHand, LightEnhancements.HIT, 1 ) )
            {
                pierce += 1;
            }

            // If we've hit enough stuff, break out of loop, otherwise, call hit function.
            if ( hitCount >= mPierce )
            {
                break;
            }
            else
            {
                // Note: this may be overriden by children of BaseLight.
                if ( Hit( hit ) )
                {
                    hitCount++;
                }
            }
        }
    }

    // ---------------------------------------------------------------
    // @Summary: Hit is called when an object is hit by this guns 
    //   raycast. It passes a reference to the "hit object". This 
    //   function is marked virtual and can also be overriden by 
    //   child classes. 
    // @Param: hit - reference to the actual object that was hit. 
    // @Return: bool - returns true if a fruit is hit
    // ---------------------------------------------------------------
    protected virtual bool Hit( RaycastHit hit )
    {
        bool hitFruit = false;

        // Get reference to any object that derrives from BaseFruit. 
        BaseFruit fruit = hit.collider.transform.GetComponent<BaseFruit>();
        if ( fruit != null )
        {
            bool canHitGhost = mPlayer.HasEnhancement( mHand, LightEnhancements.DETECTION, 2 ) || mCanHitGhost;

            uint damageDelt = fruit.Damage( mDamage, canHitGhost );
            uint cashValue = damageDelt * 5;
            if ( mPlayer.HasEnhancement( mHand, LightEnhancements.CASH, 3 ) )
            {
                mPlayer.AddCash(cashValue * 2 );
            }
            else
            {
                mPlayer.AddCash(cashValue);
            }

            hitFruit = damageDelt > 0;
        }

        // Get reference to a hittable object
        Hittable hittableObject = hit.collider.transform.GetComponent<Hittable>();
        if ( hittableObject != null )
        {
            // if not hitting fruit, or you are hitting a fruit and its not ghost
            if ( fruit == null || (fruit && !fruit.mGhost) )
            {
                hittableObject.Impact( hit, mHitMagnitude );
            }
        }

        return hitFruit;
    }

    // ---------------------------------------------------------------
    // @Summary: Muzzleflash generates the weapon's flash effects. 
    //   This function is marked virtual and can be overriden by a 
    //   child class that needs to modify the behaviour. 
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void MuzzleFlash()
    {
        if ( mPrMuzzleFlash )
        {
            mPrMuzzleFlash.Play();
        }
    }

    // ---------------------------------------------------------------
    // @Summary: Animate plays the animations for firing the gun. 
    //   This function is marked virtual and can be overriden by a 
    //   child class that needs to modify the behaviour. 
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void Animate()
    {
        if ( mAmmo == 1 )
        {
            mAnimator.Play( "LastShot", -1, 0f );
        }
        else
        {
            mAnimator.Play( "Fire", -1, 0f );
        }
    }

    // ---------------------------------------------------------------
    // @Summary: Check fire rate is a function to see if enough time 
    //   has passed for the gun to fire again. Uses mFireRate for 
    //   this calculation mFiringDelay = (60 / mFireRate).
    // @Return: bool - true if the gun is ready to shoot again. 
    // ---------------------------------------------------------------
    private bool CheckFireRate()
    {
        return (Time.time - mLastFired) > mFiringDelay;
    }

    // ---------------------------------------------------------------
    // @Summary: Function to set the max burst size based on the fire
    //   mode. For example, if you have a 3 roung burst gun, the max 
    //   burst size is 3 and you have to let go of the trigger after 
    //   that. 
    // @Return: uint - the max burst size
    // ---------------------------------------------------------------
    private uint GetMaxBurstSize()
    {
        uint maxBurst = 0U;

        switch ( mFireType )
        {
            case FireType.SEMI:
                maxBurst = 1U;
                break;

            case FireType.BURST:
                maxBurst = 3U;
                break;

            case FireType.AUTO:
                maxBurst = mMagSize;
                break;

            case FireType.UNKNOWN:
            default:
                break;
        }

        return maxBurst;
    }

    // ---------------------------------------------------------------
    // @Summary: This function is called to eject the mag cartrage. 
    //   This function is marked virtual and can be overriden by a 
    //   child class that needs to modify the behaviour. 
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void EjectMag()
    {
        Debug.Log("Ejecting mag");
        // Set reloading = TRUE to disable shooting
        mReloading = true;
        PlayMagEjectSound();
        Events.Instance.RemoveListener<EventAngularVelocity>(HandleReloadGesture);
        mListeningForReloadGesture = false;

        // Set ammo to 0 while mag is out
        mFullyEmpty = ( mAmmo == 0 );
        mAmmo = 0;

        // Shoot the empty mag out
        mMagazineObject.transform.parent = null;
        Rigidbody mag_rb = mMagazineObject.GetComponent<Rigidbody>();
        if (mag_rb == null)
        {
            mag_rb = mMagazineObject.AddComponent<Rigidbody>();
        }
        mag_rb.AddRelativeForce( new Vector3( 20, -100, 0f ) );
        mag_rb.AddRelativeTorque( new Vector3( -200, 0f, 0f ) );
        Destroy( mMagazineObject, .5f );
    }

    // ---------------------------------------------------------------
    // @Summary: This function is called to insert new mag. 
    //   This function is marked virtual and can be overriden by a 
    //   child class that needs to modify the behaviour. 
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void InsertMag()
    {
        PlayMagInsertSound();
        mAmmo = mMagSize;
        mMagazineObject = Instantiate(mMagazinePrefab, mMagazineParent);

        if (mFullyEmpty)
        {
            mAnimator.Play("MagIn", -1, 0f);
            mFullyEmpty = false;
        }

        // Done reloading, start listening for button presses again.
        mReloading = false;
        ControllerProxy.Instance().RegisterCallbackTriggerDown(mHand, HandleFireBtnDown);
        ControllerProxy.Instance().RegisterCallbackTriggerUp(mHand, HandleFireBtnUp);
    }

    // ---------------------------------------------------------------
    // @Summary: This function is called to insert new mag. 
    //   This function is marked virtual and can be overriden by a 
    //   child class that needs to modify the behaviour. 
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void MockInsertMag()
    {
        mAmmo = mMagSize;
        mMagazineObject = Instantiate(mMagazinePrefab, mMagazineParent);

        if (mFullyEmpty)
        {
            mAnimator.Play("MagIn", -1, 0f);
            mFullyEmpty = false;
        }

        // Done reloading, start listening for button presses again.
        mReloading = false;
    }


    protected void SetFireRate(uint newFireRate)
    {
        mFiringDelay = 60.0f / newFireRate;
    }

    // ---------------------------------------------------------------
    // @Summary: This function is called to play the fire sound. 
    //   This function is marked virtual and can be overriden by a 
    //   child class that needs to modify the behaviour. 
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void PlayFireSound()
    {
        mAudioSource.clip = mFireSound;
        mAudioSource.pitch = UnityEngine.Random.Range( 0.75f, 1.25f );
        mAudioSource.volume = UnityEngine.Random.Range( 0.75f, 1.25f ) * mGunshotVolume;
        mAudioSource.PlayOneShot( mFireSound );
    }

    // ---------------------------------------------------------------
    // @Summary: This function is called to play the fire sound. 
    //   This function is marked virtual and can be overriden by a 
    //   child class that needs to modify the behaviour. 
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void PlayDryfireSound()
    {
        mAudioSource.clip = mDryfireSound;
        mAudioSource.pitch = UnityEngine.Random.Range( 0.75f, 1.25f );
        mAudioSource.volume = UnityEngine.Random.Range( 0.75f, 1.25f ) * mGunshotVolume;
        mAudioSource.PlayOneShot( mDryfireSound );
    }

    // ---------------------------------------------------------------
    // @Summary: This function is called to play the eject sound. 
    //   This function is marked virtual and can be overriden by a 
    //   child class that needs to modify the behaviour. 
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void PlayMagEjectSound()
    {
        mAudioSource.clip = mMagEjectSound;
        mAudioSource.pitch = 1.0f;
        mAudioSource.volume = 1.0f;
        mAudioSource.PlayOneShot( mMagEjectSound );
    }

    // ---------------------------------------------------------------
    // @Summary: This function is called to play the insert sound. 
    //   This function is marked virtual and can be overriden by a 
    //   child class that needs to modify the behaviour. 
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void PlayMagInsertSound()
    {
        mAudioSource.clip = mMagInsertSound;
        mAudioSource.pitch = 1.0f;
        mAudioSource.volume = 1.0f;
        mAudioSource.PlayOneShot( mMagInsertSound );
    }

    // ---------------------------------------------------------------
    // @Summary: Listener for reload gesture events.
    // @Param: e - event contents
    // @Return: void.
    // ---------------------------------------------------------------
    protected virtual void HandleReloadGesture( EventAngularVelocity e )
    {
        if (e.axis == Axis.Z && e.hand == mHand)
        {
            mListeningForReloadGesture = false;
            Events.Instance.RemoveListener<EventAngularVelocity>( HandleReloadGesture );
            ControllerProxy.Instance().RemoveCallbackTriggerDown(mHand, HandleFireBtnDown);
            mShotsInBurst = 0;
            EjectMag();
        }
    }

    // ---------------------------------------------------------------
    // @Summary: This function is polled when the mag is dropped and 
    //   we need to check if the gun is brought to hip. 
    // @Return: bool - returns true if gun is brought to hip.
    // ---------------------------------------------------------------
    protected virtual bool DetectMagInsert()
    {
        // Assume the waist position is roughly 1 unit below the head position.
        Transform headPosition = Player.GetInstance().GetPlayerPosition();
        Vector3 waistPosition = headPosition.position;
        waistPosition.y = headPosition.position.y - 0.8f;

        // Get distance
        float gunToWaistDistance = Vector3.Distance(gameObject.transform.position, waistPosition);

        // If the gun is sufficiently close to players waste, pop a mag in.
        return gunToWaistDistance < 0.35f;
    }

    protected override void HandleStateChange( EventStateChange e )
    {
        Debug.Log( "BaseLight" );
        base.HandleStateChange( e );

        switch ( e.newState )
        {
            case State.ROUND_END:
                if ( mReloading )
                {
                    MockInsertMag();
                }
                else
                {
                    if ( mAmmo == 0 )
                    {
                        mAnimator.Play( "MagIn", -1, 0f );
                        mFullyEmpty = false;
                    }
                    mAmmo = mMagSize;
                }
                ControllerProxy.Instance().RemoveCallbackTriggerDown(mHand, HandleFireBtnDown);
                break;

            case State.ROUND_IN_PROGRESS:
                ControllerProxy.Instance().RegisterCallbackTriggerDown(mHand, HandleFireBtnDown);
                break;

            default:
                break;
        }
    }


    protected void HandleFireBtnDown( InputAction.CallbackContext context )
    {
        Debug.Log("FireBtnDown");
        if ( !mReloading && !mListeningForReloadGesture && mEnabled )
        {
            mListeningForReloadGesture = true;
            Debug.Log( "Adding reload listener" );
            Events.Instance.AddListener<EventAngularVelocity>( HandleReloadGesture );
        }
        if ( mFireType != FireType.BURST
            || (mFireType == FireType.BURST && mShotsInBurst == 0))
        {
            mFireBtnDown = true;
            mShotsInBurst = Math.Min( mAmmo, GetMaxBurstSize() );
        }

        // If empty, play dry fire sound
        if (mAmmo == 0 )
        {
            PlayDryfireSound();
        }
    }

    protected void HandleFireBtnUp( InputAction.CallbackContext context )
    {
        Debug.Log("FireBtnUp");

        if ( mFireType != FireType.BURST )
        {
            mFireBtnDown = false;
        }
    }

    // ---------------------------------------------------------------
    // Override the Awake() function. 
    // ---------------------------------------------------------------
    private void Awake()
    {
        mHapticsClip = new Vibration(mVibration);
        mAudioSource = gameObject.AddComponent<AudioSource>();
        mAnimator = GetComponent<Animator>();
        mFiringDelay = 60.0f / mFireRate;
        mMaxBurstSize = GetMaxBurstSize();
        mAmmo = mMagSize;
    }

    // ---------------------------------------------------------------
    // Override the Start() function. 
    // ---------------------------------------------------------------
    protected new virtual void Start()
    {
        base.Start();
        mMagazineObject = Instantiate( mMagazinePrefab, mMagazineParent );
        ControllerProxy.Instance().RegisterCallbackTriggerUp(mHand, HandleFireBtnUp);
    }

    // ---------------------------------------------------------------
    // Override the Update() function. 
    // ---------------------------------------------------------------
    protected new virtual void Update()
    {
        base.Update();

        // Check if brought to waist
        if ( mReloading )
        {
            if ( DetectMagInsert() ) 
            {
                InsertMag();
            }
        }

        // Firing sequence
        if (mFireBtnDown)
        {
            if(CheckFireRate())
            {
                if ( mEnabled)
                {
                    if ( mAmmo > 0 )
                    {
                        Shoot();
                        mAmmo--;
                        mLastFired = Time.time;
                        mShotsInBurst--;

                        if ( mShotsInBurst == 0)
                        {
                            // Force fire button up after semi-auto or burst firing
                            mFireBtnDown = false;
                        }
                    }
                }
            }
        }

        // Update ammo count UI
        mAmmoUi.text = mAmmo.ToString();
    }

    public override void Enable()
    {
        if (mEnabled) return;
        base.Enable();
        ControllerProxy.Instance().RegisterCallbackTriggerDown(mHand, HandleFireBtnDown);
        if (mAmmo < mMagSize)
        {
            Events.Instance.AddListener<EventAngularVelocity>(HandleReloadGesture);
            mListeningForReloadGesture = true;
        }
    }

    public override void Disable()
    {
        base.Disable();
        ControllerProxy.Instance().RemoveCallbackTriggerDown(mHand, HandleFireBtnDown);
        Events.Instance.RemoveListener<EventAngularVelocity>(HandleReloadGesture);
        mListeningForReloadGesture = false;
    }

    private void OnEnable()
    {
        Events.Instance.AddListener<EventStateChange>( HandleStateChange );
    }

    private void OnDisable()
    {
        Events.Instance.RemoveListener<EventStateChange>( HandleStateChange );
        ControllerProxy.Instance().RemoveCallbackTriggerDown(mHand, HandleFireBtnDown);
        ControllerProxy.Instance().RemoveCallbackTriggerUp(mHand, HandleFireBtnUp);
    }
}