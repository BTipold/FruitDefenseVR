// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using CommonTypes;

public class L44Script : L34Script
{
    private enum ShellType
    {
        NORMAL,
        INCINDIARY,
        STUN
    }

    public Sprite mNormalShellIcon = null;
    public Sprite mIncindiaryShellIcon = null;
    public Sprite mStunShellIcon = null;
    public SpriteRenderer mShellIcon = null;

    private ShellType mShellType = ShellType.NORMAL;

    protected override bool Hit( RaycastHit hit )
    {
        bool hitFruit = false;

        // Get reference to a hittable object
        Hittable hittableObject = hit.collider.transform.GetComponent<Hittable>();
        if ( hittableObject != null )
        {
            hittableObject.Impact( hit, mHitMagnitude );
        }

        // Get reference to any object that derrives from BaseFruit. 
        BaseFruit fruit = hit.collider.transform.GetComponent<BaseFruit>();
        if ( fruit != null )
        {
            bool canHitGhost = mPlayer.HasEnhancement( mHand, LightEnhancements.DETECTION, 2 );

            uint damageDelt = fruit.Damage( mDamage, canHitGhost );
            uint cashValue = damageDelt * 10;
            if (mPlayer.HasEnhancement(mHand, LightEnhancements.CASH, 3))
            {
                mPlayer.AddCash(cashValue * 2);
            }
            else
            {
                mPlayer.AddCash(cashValue);
            }

            hitFruit = damageDelt > 0;
        }

        return hitFruit;
    }

    private void SwitchShells()
    {
        switch( mShellType )
        {
            case ShellType.NORMAL:
                mShellType = ShellType.INCINDIARY;
                mShellIcon.sprite = mIncindiaryShellIcon;
                break;

            case ShellType.INCINDIARY:
                mShellType = ShellType.STUN;
                mShellIcon.sprite = mStunShellIcon;
                break;

            case ShellType.STUN:
                mShellType = ShellType.NORMAL;
                mShellIcon.sprite = mNormalShellIcon;
                break;
        }

        return;
    }

    protected override void Update()
    {
        base.Update();

        // Check for ability button presses
        //if ( Controllers.utility.GetDown( mHand ) )
        /*{
            SwitchShells();
            Debug.Log( "Switching shell type" );
        }*/
    }
}
