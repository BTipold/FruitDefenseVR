// ---------------------------------------------------------------
// This software and all related information is the intellectual
// property of T2 Studios and may not be distributed, replicated
// or disclosed without the explicit prior written permission of
// T2 Studios. All Rights Reserved.
// ---------------------------------------------------------------


using CommonTypes;
using UnityEngine.UI;
using UnityEngine;


public abstract class BaseHeavy : BaseWeapon
{
    // ---------------------------------------------------------------
    // Member Variables
    // ---------------------------------------------------------------
    public float mRadius = 0F;
    public float mVelocity = 0F;
    public FireType mFireType = FireType.UNKNOWN;
    public Text mAmmoUi = null;
    public Transform mBarrelPos = null;
    public AudioClip mFireSfx = null;
    public AudioClip mMagEjectSfx = null;
    public AudioClip mMagInsertSfx = null;
    public AudioClip mExplosionSfx = null;
    public GameObject mPfMuzzleFlash = null;
    public GameObject mPfExplosion = null;
    public GameObject mPfGrenade = null;
    public Vibration.VibrationPreset mVibration;

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

    // ---------------------------------------------------------------
    // @Summary: 
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void InitGrenade()
    {
        GameObject grenade = Instantiate(mPfGrenade);
        grenade.transform.position = mBarrelPos.position;

        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce( mBarrelPos.forward * mVelocity );

        BaseGrenade script = grenade.GetComponent<BaseGrenade>();
        script.SetDamage( mDamage );
        script.SetRadius( mRadius );
        script.SetExplosionFx( mPfExplosion );
        script.SetExplosionSound( mExplosionSfx );
    }


    // ---------------------------------------------------------------
    // @Summary: shoot executes functions to make the gun shoot. 
    //   This function can be overriden by children if they need 
    //   different behaviour.
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void Shoot()
    {
        // Gun effects
        PlayFireSound();
        //Animate();
        //ControllerProxy.SendVibration( mHand, mHapticsClip );
        InitGrenade();
    }

    // ---------------------------------------------------------------
    // @Summary: Muzzleflash generates the weapon's flash effects. 
    //   This function is marked virtual and can be overriden by a 
    //   child class that needs to modify the behaviour. 
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void MuzzleFlash()
    {
        if ( mPfMuzzleFlash != null )
        {
            ParticleSystem[] particles = mPfMuzzleFlash.gameObject.GetComponentsInChildren<ParticleSystem>();

            foreach ( ParticleSystem particle in particles )
            {
                particle.Play();
            }
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
        mAnimator.Play( "Fire", -1, 0f );
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
        //Rigidbody mag_rb = m_magazine.AddComponent<Rigidbody>();
        //mag_rb.AddRelativeForce( new Vector3( 0f, -100f, 0f ) );
        //Destroy( m_magazine, .5f );
        PlayMagEjectSound();
        //PlayHaptic(mMagEjectSound);
        mReloading = true;
    }

    // ---------------------------------------------------------------
    // @Summary: This function is called to insert new mag. 
    //   This function is marked virtual and can be overriden by a 
    //   child class that needs to modify the behaviour. 
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void InsertMag()
    {
        //m_magazine = Instantiate<GameObject>( m_mag_prefab, m_mag_parent.transform );
        PlayMagInsertSound();
        //PlayHaptic(mMagInsertSound);
        mReloading = false;
    }

    // ---------------------------------------------------------------
    // @Summary: This function is called to play the fire sound. 
    //   This function is marked virtual and can be overriden by a 
    //   child class that needs to modify the behaviour. 
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void PlayFireSound()
    {
        mAudioSource.clip = mFireSfx;
        mAudioSource.pitch = Random.Range( 0.75f, 1.25f );
        mAudioSource.volume = Random.Range( 0.75f, 1.25f );
        mAudioSource.PlayOneShot( mFireSfx );
    }

    // ---------------------------------------------------------------
    // @Summary: This function is called to play the eject sound. 
    //   This function is marked virtual and can be overriden by a 
    //   child class that needs to modify the behaviour. 
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void PlayMagEjectSound()
    {
        mAudioSource.clip = mMagEjectSfx;
        mAudioSource.pitch = 1.0f;
        mAudioSource.volume = 1.0f;
        mAudioSource.PlayOneShot( mMagEjectSfx );
    }

    // ---------------------------------------------------------------
    // @Summary: This function is called to play the insert sound. 
    //   This function is marked virtual and can be overriden by a 
    //   child class that needs to modify the behaviour. 
    // @Return: void
    // ---------------------------------------------------------------
    protected virtual void PlayMagInsertSound()
    {
        mAudioSource.clip = mMagInsertSfx;
        mAudioSource.pitch = 1.0f;
        mAudioSource.volume = 1.0f;
        mAudioSource.PlayOneShot( mMagInsertSfx );
    }

    protected virtual bool DetectReloadGesture()
    {
        return true;//Controllers.DetectGestureRotational( mHand, 25.0f, Axis.Z );
    }

    protected virtual bool DetectMagInsert()
    {
        // Assume the waist position is roughly 1 unit below the head position.
        Transform headPosition = Player.GetInstance().GetPlayerPosition();
        Vector3 waistPosition = headPosition.position - headPosition.up;
        float gunToWaistDistance = Vector3.Distance(gameObject.transform.position, waistPosition);

        // If the gun is sufficiently close to players waste, pop a mag in.
        return gunToWaistDistance < 0.5f;
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
    protected virtual void Start()
    {
        
    }

    // ---------------------------------------------------------------
    // Override the Update() function. 
    // ---------------------------------------------------------------
    protected virtual void Update()
    {
        // Early escape option if weapon is disabled.
        if ( !mEnabled )
        {
            return;
        }

        // Check for reloads
        if ( !mReloading )
        {
            if ( mAmmo != mMagSize )
            {
                if ( DetectReloadGesture() )
                {
                    mAmmo = 0;
                    EjectMag();
                }
            }
        }

        // Check if brought to waist
        if ( mReloading )
        {
            if ( DetectMagInsert() ) // Cheese it for now, well detect this later 
            {
                InsertMag();
                mAmmo = mMagSize;
            }
        }

        // This block of code will handle all the shooting logic. 
        if ( false )
        {
            if ( CheckFireRate() )
            {
                if (mAmmo > 0 && !mBurstComplete )
                {
                    Shoot();

                    mLastFired = Time.time;
                    mAmmo--;
                    mShotsInBurst++;
                    mBurstComplete = ( mShotsInBurst >= mMaxBurstSize );
                }
            }
        }
        else
        {
            mShotsInBurst = 0;
            mBurstComplete = false;
        }

        // Update ammo count UI
        mAmmoUi.text = mAmmo.ToString();
    }
}