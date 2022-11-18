using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterBombs : BaseHeavy
{
    protected override void InitGrenade()
    {
        GameObject grenade = Instantiate(mPfGrenade);
        grenade.transform.position = mBarrelPos.position;
        grenade.transform.rotation = mBarrelPos.rotation;

        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce( mBarrelPos.forward * mVelocity );
        BaseGrenade script = grenade.GetComponent<BaseGrenade>();
        script.SetDamage( mDamage );
        script.SetRadius( mRadius );
        script.SetExplosionFx( mPfExplosion );
        script.SetExplosionSound( mExplosionSfx );
    }
}
