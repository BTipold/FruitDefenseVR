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

public class L52Script : BaseLight
{
    public float mAbilityActiveTime = 0f;
    public ParticleSystem mAbilityFlash = null;
    public ParticleSystem mDefaultFlash = null;
    private bool mAbilityActive = false;
    private uint mDefaultFireRate = 0;

    protected override void Ability()
    {
        mDefaultFireRate = mFireRate;
        SetFireRate(600);
        mDefaultFlash = mPrMuzzleFlash;
        mPrMuzzleFlash = mAbilityFlash;
        StartCoroutine( AbilityTimer() );
        Debug.Log( "Using Ability!" );
    }

    private IEnumerator AbilityTimer()
    {
        yield return new WaitForSeconds( mAbilityActiveTime );
        SetFireRate( mDefaultFireRate );
        mPrMuzzleFlash = mDefaultFlash;
    }
}
