// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------

using UnityEngine;
using CommonTypes;

public class L34Script : BaseLight
{
    public uint mPelletCount = 5U;
    public Transform mUiLeftPos = null;
    public Transform mUiRightPos = null;
    public Canvas mUi = null;

    protected override void ShootingMechanics()
    {
        for ( int i = 0; i < mPelletCount; i++ )
        {
            // Set Pellet spread direction vector
            Vector3 directionVector = mRayEmitter.forward;
            directionVector.x += Random.Range( -0.1f, 0.1f );
            directionVector.y += Random.Range( -0.1f, 0.1f );

            // Raycast physics
            RaycastHit[] hits = Physics.RaycastAll( mRayEmitter.position,
            directionVector, mRange, Utils.IgnoreBitmask("Ignore Raycast") );

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
    }

    protected void Start()
    {
        base.Start();

        switch (mHand)
        {
            case VRHand.LEFT:
                mUi.transform.position = mUiLeftPos.position;
                break;

            case VRHand.RIGHT:
                mUi.transform.position = mUiRightPos.position;
                break;
        }

        return;
    }
}
