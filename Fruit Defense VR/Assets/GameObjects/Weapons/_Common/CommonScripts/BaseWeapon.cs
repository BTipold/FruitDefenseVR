// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using CommonTypes;
using UnityEngine;
using System;


public abstract class BaseWeapon : MonoBehaviour
{
    // -----------------------------------------------------------
    // Member Variables
    // -----------------------------------------------------------
    public WeaponClass mWeaponClass = WeaponClass.UNKNOWN;
    public bool mComingSoon = false;
    public uint mCost;
    public uint mFireRate;
    public uint mDamage;
    public uint mMagSize;
    public uint mAbilities;
    public float mCooldownTime;
    public Sprite mImage;
    public string mId;
    public string mName;
    public string mDescription;
    public GameObject mPrNextUpgradeTop;
    public GameObject mPrNextUpgradeBottom;
    public GameObject mPrPrevUpgrade;
    public Transform mLaserEmitter;
    public bool mCanHitGhost;
    protected Player mPlayer = null;
    protected VRHand mHand;
    protected bool mEnabled;
    protected float mAbilityCooldown = 0;
    protected LineRenderer mLaser = null;

    // -----------------------------------------------------------
    // @Summary: setter for weapon hand.
    // @Param: hand - which hand the weapon is in. 
    // @Return: void
    // -----------------------------------------------------------
    public void SetHand( VRHand hand )
    {
        mHand = hand;
    }

    // -----------------------------------------------------------
    // @Summary: destroys this current object and returns the 
    //   previous weapon, so that the players weapon is 
    //   downgraded.
    // @Return: returns the downgraded weapon or null if there is
    //   none. 
    // -----------------------------------------------------------
    public GameObject Sell()
    {
        GameObject previousUpgrade = mPrPrevUpgrade;
        Destroy( gameObject );
        return previousUpgrade;
    }

    // -----------------------------------------------------------
    // @Summary: returns the game object of top path weapon.
    // @Return: returns the upgraded weapon or null if there is
    //   none. 
    // -----------------------------------------------------------
    public GameObject GetUpgradeTop()
    {
        return mPrNextUpgradeTop;
    }

    // -----------------------------------------------------------
    // @Summary: returns the game object of bottom path weapon.
    // @Return: returns the upgraded weapon or null if there is
    //   none. 
    // -----------------------------------------------------------
    public GameObject GetUpgradeBottom()
    {
        return mPrNextUpgradeBottom;
    }

    // -----------------------------------------------------------
    // @Summary: enables the weapon mesh, canvas, enables weapon 
    //   script functions. 
    // @Return: void
    // -----------------------------------------------------------
    public virtual void Enable()
    {
        if (mEnabled) return;
        Debug.Log( "Enabling weapon" );
        mEnabled = true;
        if ( mLaser )
        {
            if ( GameState.GetDifficulty() == Difficulty.EASY 
                || GameState.GetInstance().GetState() == State.ROUND_END
                || GameState.GetInstance().GetState() == State.PAUSE)
            {
                mLaser.enabled = true;
            }
            else
            {
                mLaser.enabled = false;
            }
        }
    }

    // -----------------------------------------------------------
    // @Summary: dsiables the weapon mesh, disables weapon 
    //   script functions. 
    // @Return: void
    // -----------------------------------------------------------
    public virtual void Disable()
    {
        mEnabled = false;
        if (mLaser)
        {
            mLaser.enabled = true;
        }
    }

    // -----------------------------------------------------------
    // @Summary: called when user presses the ability button.
    // @Return: void
    // -----------------------------------------------------------
    private void UseAbility()
    {
        if ( mAbilities > 0 )
        {
            if ( mAbilityCooldown == 0 )
            {
                Ability();
                mAbilityCooldown = mCooldownTime;
            }
        }
    }

    // -----------------------------------------------------------
    // @Summary: function which activates the ability. Overriden
    //   by specific weapons to implement abilities.
    // @Return: void
    // ----------------------------------------------------------- 
    protected virtual void Ability()
    {
        // Override this function
    }

    // -----------------------------------------------------------
    // @Summary: state change event handler. This handler 
    //   switches the weapons for controllers when the round ends
    //   or menu is opened. 
    // @Param: e - event data. Tells you new and last state.
    // @Return: void
    // -----------------------------------------------------------
    protected virtual void HandleStateChange( EventStateChange e )
    {
        Debug.Log( "BaseWeapon" );

        switch ( e.newState )
        {
            case State.ROUND_IN_PROGRESS:
                Enable();
                break;

            case State.PAUSE:
                Disable();
                break;

            case State.GAME_WON:
            case State.GAME_LOST:
                Disable();
                break; 

            case State.ROUND_END:
                Disable();
                break;

            default:
                break;
        }
    }

    // -----------------------------------------------------------
    // Override the OnEnable() method. 
    // -----------------------------------------------------------
    private void OnEnable()
    {
        Events.Instance.AddListener<EventStateChange>( HandleStateChange );
    }

    // -----------------------------------------------------------
    // Override the OnDisable() method. 
    // -----------------------------------------------------------
    private void OnDisable()
    {
        Events.Instance.RemoveListener<EventStateChange>( HandleStateChange );
    }

    public void Start()
    {
        mPlayer = Player.GetInstance();

        if ( mLaserEmitter != null )
        {
            // Only activate if gun doesn't already have a laser
            if ( gameObject.GetComponentsInChildren<LineRenderer>().Length <= 1 )
            {
                mLaser = gameObject.GetComponent<LineRenderer>();
                mLaser.enabled = true;
            }
        }
    }

    protected virtual void Update()
    {
        // Check for ability button presses
        if ( false )
        {
            UseAbility();
        }

        // Tickdown the ability cooldown
        if ( mAbilityCooldown > 0 )
        {
            mAbilityCooldown = Math.Max( 0f, mAbilityCooldown - Time.deltaTime );
        }

        // Set laser if on easy mode
        if ( mLaser && mLaser.enabled )
        {
            mLaser.SetPosition( 0, mLaserEmitter.position );

            RaycastHit hit;
            if ( Physics.Raycast( mLaserEmitter.position, mLaserEmitter.forward, out hit, 4f, Utils.IgnoreBitmask("Ignore Raycast")) )
            {
                mLaser.SetPosition( 1, hit.point );
            }
            else
            {
                mLaser.SetPosition( 1, mLaserEmitter.position + mLaserEmitter.forward * 4f );
            }
        }
    }
}