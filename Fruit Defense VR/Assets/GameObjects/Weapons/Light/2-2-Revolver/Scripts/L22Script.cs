// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L22Script : BaseLight
{
    public  GameObject mCylinder;
    public  GameObject mBulletPrefab;
    public  Transform[] mBulletParents = new Transform[6];
    private GameObject[] mBulletObj = new GameObject[6];
    private Quaternion mNewRotation;
    private Quaternion mOriginalRotation;

    protected override void Shoot()
    {
        base.Shoot();
        uint bulletNumber = 6 - mAmmo;
        Destroy( mBulletObj[bulletNumber] );

        // Rotate the barrel 60 degrees. 
        mNewRotation *= Quaternion.Euler( -60, 0, 0 );
    }

    protected override void Start()
    {
        if ( mLaserEmitter != null )
        {
            // Only activate if gun doesn't already have a laser
            if ( gameObject.GetComponentsInChildren<LineRenderer>().Length <= 1 )
            {
                mLaser = gameObject.GetComponent<LineRenderer>();
                mLaser.enabled = true;
            }
        }

        mPlayer = Player.GetInstance();
        mOriginalRotation = mCylinder.transform.localRotation;
        mNewRotation = mOriginalRotation;

        for (int i = 0; i < 6; i++ )
        {
            mBulletObj[i] = Instantiate( mBulletPrefab, mBulletParents[i] );
        }
    }

    protected override void InsertMag()
    {
        PlayMagInsertSound();
        mReloading = false;

        mAmmo = mMagSize;
        mFullyEmpty = false;

        mNewRotation = mOriginalRotation;

        for ( int i = 0; i < 6; i++ )
        {
            if ( mBulletObj[i] != null )
            {
                Destroy( mBulletObj[i] );
            }
            mBulletObj[i] = Instantiate( mBulletPrefab, mBulletParents[i] );
        }
        mAnimator.Play( "MagIn", -1, 0f );
        
    }

    protected override void EjectMag()
    {
        PlayMagEjectSound();
        mReloading = true;
        mAnimator.Play( "MagOut", -1, 0f );
        mAmmo = 0;
    }

    protected override void Update()
    {
        base.Update();

        // Get the current rotation
        Quaternion currentRotation = mCylinder.transform.localRotation;

        // The step size is equal to speed times frame time.
        float step = 200 * Time.deltaTime;

        // Rotate our transform a step closer to the target's.
        mCylinder.transform.localRotation = Quaternion.RotateTowards( currentRotation, mNewRotation, step );
    }

    protected override void Animate()
    {
        mAnimator.Play( "Fire", -1, 0f );
    }
}
