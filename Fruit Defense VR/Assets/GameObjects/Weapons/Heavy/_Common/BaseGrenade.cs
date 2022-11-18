using System;
using UnityEngine;

[Serializable]
public class BaseGrenade : MonoBehaviour
{
    // -----------------------------------------------------------
    // Member Variables
    // -----------------------------------------------------------
    protected uint mDamage = 0;
    protected float mRadius = 0;
    protected GameObject mExplosionVfx = null;
    protected AudioClip mExplosionSfx = null;

    // -----------------------------------------------------------
    // @Summary: will instantiate audio and visual effects and 
    //   will deal damage to enemies in surrounding areas. 
    // @Return: void
    // -----------------------------------------------------------
    protected virtual void ExplosionFx()
    {
        GameObject explosion = Instantiate( mExplosionVfx );
        explosion.transform.position = gameObject.transform.position;
        explosion.AddComponent<AudioSource>();

        AudioSource sound = explosion.GetComponent<AudioSource>();
        sound.pitch = UnityEngine.Random.Range( 0.75f, 1.25f );
        sound.volume = UnityEngine.Random.Range( 0.5f, 0.75f );
        sound.PlayOneShot( mExplosionSfx );

        // Destroy the grenade object
        Destroy( gameObject );

        // Destroy the explosion after 2 seconds.
        Destroy( explosion, 2f );
    }

    // -----------------------------------------------------------
    // @Summary: will instantiate audio and visual effects and 
    //   will deal damage to enemies in surrounding areas. 
    // @Return: void
    // -----------------------------------------------------------
    protected virtual void DealDamage()
    {
        Collider[] fruitsHit = Physics.OverlapSphere( gameObject.transform.position, mRadius, (1 << LayerMask.NameToLayer( "Fruit" )) );
        foreach (Collider fruitCollider in fruitsHit)
        {
            BaseFruit fruit = fruitCollider.gameObject.GetComponent<BaseFruit>();
            if ( fruit != null)
            {
                fruit.Damage( mDamage );
            }
        }
    }

    // -----------------------------------------------------------
    // @Summary: allows the grenade launcher to set grenade dmg. 
    // @Param: damage - the damage to inflict. 
    // @Return: void
    // -----------------------------------------------------------
    public void SetDamage( uint damage )
    {
        mDamage = damage;
    }

    // -----------------------------------------------------------
    // @Summary: allows the grenade launcher to set explosion 
    //   radius.
    // @Param: radius - the new damage radius. 
    // @Return: void
    // -----------------------------------------------------------
    public void SetRadius( float radius )
    {
        mRadius = radius;
    }

    // -----------------------------------------------------------
    // @Summary: allows the grenade launcher to set explosion 
    //   effect prefab.
    // @Param: explosionFx - the particle effect prefab.
    // @Return: void
    // -----------------------------------------------------------
    public void SetExplosionFx( GameObject explosionFx )
    {
        mExplosionVfx = explosionFx;
    }

    // -----------------------------------------------------------
    // @Summary: allows the grenade launcher to set explosion 
    //   sound fx.
    // @Param: sfx - the sound clip to play on explode. 
    // @Return: void
    // -----------------------------------------------------------
    public void SetExplosionSound( AudioClip sfx)
    {
        mExplosionSfx = sfx;
    }


}
