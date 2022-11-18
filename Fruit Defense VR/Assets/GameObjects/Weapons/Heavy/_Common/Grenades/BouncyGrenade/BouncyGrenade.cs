// ---------------------------------------------------------------
// This software and all related information is the intellectual
// property of T2 Studios and may not be distributed, replicated
// or disclosed without the explicit prior written permission of
// T2 Studios. All Rights Reserved.
// ---------------------------------------------------------------


using System.Collections;
using UnityEngine;


public class BouncyGrenade : BaseGrenade
{
    // -----------------------------------------------------------
    // Member Variables
    // -----------------------------------------------------------
    public float mFuseTimer = 0f;

    // -----------------------------------------------------------
    // @Summary: Co-routine which causes an explosion after the
    //   grenade timer expires. 
    // @Return: IEnumerator
    // -----------------------------------------------------------
    IEnumerator DelayedExplosion()
    {
        // Delay until the grenade is ready to explode.
        yield return new WaitForSeconds( mFuseTimer );

        // Create explosion
        ExplosionFx();
        DealDamage();
    }

    // -----------------------------------------------------------
    // Override the Start() method. 
    // -----------------------------------------------------------
    void Start()
    {
        // Start the fuse when grenade is spawned
        StartCoroutine( DelayedExplosion() );
    }
}
