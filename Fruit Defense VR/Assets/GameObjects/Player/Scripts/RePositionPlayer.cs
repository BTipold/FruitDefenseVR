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
using CommonTypes;

public class RePositionPlayer : MonoBehaviour
{
    public GameObject mRaycastSource;
    public GameObject mOBTargetPrefab;
    public GameObject mIBTargetPrefab;
    public float mMaxDistance = 1000;

    private GameObject mOBTargetObj;
    private GameObject mIBTargetObj;
    private bool mIsInBounds = false;
    private bool mRayCastHitsSomething = false;

    private void Teleport()
    {
        Vector3 targetPos = mIBTargetObj.transform.position;
        Player.GetInstance().Teleport( targetPos );
    }

    private void Raycast()
    {
        RaycastHit hit;
        mRayCastHitsSomething = Physics.Raycast( mRaycastSource.transform.position,
            mRaycastSource.transform.forward, out hit, mMaxDistance,
            Utils.IgnoreBitmask("Ignore Raycast") );
        if ( mRayCastHitsSomething )
        {
            // Move the target icon to the raycast position.
            mOBTargetObj.transform.position = hit.point;
            mIBTargetObj.transform.position = hit.point;

            // Enable/disable teleporting based on if target is in bounds.
            string tag = hit.transform.tag;
            mIsInBounds = ( tag == "PlayerBounds" );
        }
    }

    private void SwitchTarget()
    {
        mIBTargetObj.SetActive( mIsInBounds & mRayCastHitsSomething );
        mOBTargetObj.SetActive( !mIsInBounds & mRayCastHitsSomething );
    }

    private void OnEnable()
    {
        mIBTargetObj = Instantiate( mIBTargetPrefab );
        mOBTargetObj = Instantiate( mOBTargetPrefab );
    }

    private void OnDisable()
    {
        Destroy( mIBTargetObj );
        Destroy( mOBTargetObj );
    }

    private void Update()
    {
        Raycast();
        SwitchTarget();

        //if ( Controllers.fire.Get( VRHand.LEFT ) || Controllers.fire.Get( VRHand.RIGHT ) )
        {
            if ( mIsInBounds )
            {
                Teleport();
                this.enabled = false;
            }
        }
    }
}
