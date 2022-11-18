// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using System;
using System.Collections.Generic;
using UnityEngine;


public class BaseFruit : Hittable
{
    // -----------------------------------------------------------
    // Member Variables
    // -----------------------------------------------------------
    public GameObject mDeathEffect = null;
    public AudioClip mDeathSound = null;
    public uint mDamage = 0;
    public uint mSpeed = 0;
    public int  mHealth = 0;
    public uint mLevel = 0;
    public bool mGhost = false;
    public uint mDamageLevel1 = 0;
    public uint mDamageLevel2 = 0;
    public Mesh[] mDamageMeshs1 = new Mesh[3];
    public Mesh[] mDamageMeshs2 = new Mesh[3];
    public MeshFilter mLod0Mesh = null;
    public MeshFilter mLod1Mesh = null;
    public MeshFilter mLod2Mesh = null;
    public Material mDamagedMat = null;

    private List<Action<GameObject>> mDoOnDestroy = new List<Action<GameObject>>();

    // -----------------------------------------------------------
    // @Summary: will delete the game object, activate death 
    //   animation and activate the death sound.
    // @Return: void
    // -----------------------------------------------------------
    public virtual void Destroy()
    {
        mDoOnDestroy.ForEach((f) => f(this.gameObject));
        if ( mDeathEffect && mDeathSound )
        {
            GameObject deathEffect = Instantiate(mDeathEffect);
            deathEffect.transform.position = gameObject.transform.position;
            deathEffect.AddComponent<AudioSource>();
            
            AudioSource sound = deathEffect.GetComponent<AudioSource>();
            sound.pitch = UnityEngine.Random.Range(0.75f, 1.25f);
            sound.volume = UnityEngine.Random.Range(0.5f, 0.75f);
            sound.PlayOneShot(mDeathSound);

            Destroy(deathEffect, 1f);
        }
        Destroy(gameObject);
    }

    // -----------------------------------------------------------
    // @Summary: adds functions that will be run before 
    //   destroying.
    // @Param: action - function to be performed
    // @Return: void
    // -----------------------------------------------------------
    public void DoOnDestroy(Action<GameObject> action)
    {
        mDoOnDestroy.Add(action);
    }

    // -----------------------------------------------------------
    // @Summary: function is called when the fruit is hit by a 
    //   ray. Will deal damage and despawn the fruit if its dead.
    // @Return: uint - returns how much damage was actually done
    //   to the fruit. 
    // -----------------------------------------------------------
    public uint Damage( uint damage, bool canHitGhost = false )
    {
        uint damageDelt = 0U;

        if ( mGhost == false || canHitGhost == true )
        {
            damageDelt = damage;
            int newHealth = (int)mHealth - (int)damage;

            if ( newHealth <= 0 )
            {
                // If we've done enough damage to destroy the fruit, the 
                // damage delt needs to subtract the (or add the negative)
                // overflow health. After subtracting overlow health, destroy
                // the fruit.
                damageDelt += (uint)newHealth;
                Destroy();
            }
            else
            {
                ActivateVisualDamage( newHealth );
                mHealth = newHealth;
            }
        }

        return damageDelt;
    }

    // -----------------------------------------------------------
    // @Summary: function will display the correct visual model
    //   based on how much health the fruit has remaining.
    // @Param: health - new health of the fruit.
    // @Return: void. 
    // -----------------------------------------------------------
    private void ActivateVisualDamage( int health )
    {
        if ( health <= mDamageLevel2)
        {
            mLod0Mesh.mesh = mDamageMeshs2[0];
            mLod1Mesh.mesh = mDamageMeshs2[1];
            mLod2Mesh.mesh = mDamageMeshs2[2];
        }
        else if ( health <= mDamageLevel1 )
        {
            mLod0Mesh.mesh = mDamageMeshs1[0];
            mLod1Mesh.mesh = mDamageMeshs1[1];
            mLod2Mesh.mesh = mDamageMeshs1[2];
        }

        return;
    }

    // -----------------------------------------------------------
    // Override the Awake() function. 
    // -----------------------------------------------------------
    protected void Awake()
    {
    }
}