using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniLauncher : BaseHeavy
{
    protected override void InitGrenade()
    {
        GameObject grenade = Instantiate(mPfGrenade);
        grenade.transform.position = mBarrelPos.position;

        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce( mBarrelPos.forward * mVelocity );
        rb.AddRelativeTorque( Random.Range( -50.0f, 50.0f ),
                              Random.Range( -50.0f, 50.0f ),
                              Random.Range( -50.0f, 50.0f ) );

        BaseGrenade script = grenade.GetComponent<BaseGrenade>();
        script.SetDamage( mDamage );
        script.SetRadius( mRadius );
        script.SetExplosionFx( mPfExplosion );
        script.SetExplosionSound( mExplosionSfx );
    }
}
