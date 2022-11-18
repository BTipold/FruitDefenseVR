// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using CommonTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FollowPath : MonoBehaviour
{
    // -----------------------------------------------------------
    // Member Variables 
    // -----------------------------------------------------------
    public  Rigidbody     mRigidBody      = null;
    private int           mNextPoint      = 0;
    private List<Vector3> mPoints         = null;
    private BaseFruit     mFruitScript    = null;
    private bool          mOverrideSpawn  = false;
    private uint          mFramesWithNoMovement = 0;
    private const float   CLOSE_THRESHOLD = 4.5f;
    private const float   RANDOMNESS_TIMER = 0.5f;


    // -----------------------------------------------------------
    // @Summary: increments the member variable "nextPoint" to
    //   tell fruit that it needs to move towards the next point. 
    // @Return: bool - returns true if there is a next point to
    //   get, returns false if there is no next point.
    // -----------------------------------------------------------
    private bool GetNextPoint()
    {
        bool isNextPoint = true;

        if ( mNextPoint < mPoints.Count - 1 )
        {
            mNextPoint++;
        }
        else
        {
            isNextPoint = false;
        }

        return isNextPoint;
    }

    // -----------------------------------------------------------
    // @Summary: 
    // @Return: 
    // -----------------------------------------------------------
    public void SetPosition( Vector3 pos, int index )
    {
        transform.position = pos;
        mOverrideSpawn = true;
        mNextPoint = index;

    }

    public float GetPositionAsProgress()
    {
        float totalLength = 0;
        float currentLength = 0;
        for (int i = 0; i < mPoints.Count - 1; i++)
        {
            totalLength += Vector3.Distance(mPoints[i], mPoints[i + 1]);
            if (i + 1 == mNextPoint) {
                currentLength += Vector3.Distance(mPoints[i], GetPosition());
            } else if (i < mNextPoint) {
                currentLength = totalLength;
            }
        }

        return currentLength / totalLength * 100;
    }

    public Vector3 GetPosition()
    {
        return gameObject.transform.position;
    }

    // -----------------------------------------------------------
    // @Summary: checks distance to path point and compares 
    //   squared magnitudes to determine if it's close enough. We 
    //   don't square root so we save cpu power. Also, we 
    //   only check if we're "close enough" not exact. 
    // @Return: bool - returns true if "close enough" to next 
    //   point. 
    // -----------------------------------------------------------
    private bool IsAtPoint()
    {
        Vector3 delta       = transform.position - mPoints[mNextPoint];
        float   sqr_dist    = delta.sqrMagnitude;
        return ( sqr_dist < CLOSE_THRESHOLD );
    }

    // -----------------------------------------------------------
    // @Summary: If fruit gets to the end, we call this function
    //   to despawn the fruit and damage the player.
    // @Return: void
    // -----------------------------------------------------------
    private void Despawn()
    {
        GameState.GetInstance().DecrementHealth( mFruitScript.mDamage );
        Destroy( gameObject );
    }

    // -----------------------------------------------------------
    // @Summary: adds force until the fruit reaches max speed.
    // @Return: void
    // -----------------------------------------------------------
    private void Accelerate()
    {
        float currentVelocity = mRigidBody.velocity.magnitude;
        float maxVelocity = (1.0f + mFruitScript.mSpeed)*2.5f;

        bool notMoving = currentVelocity < 0.1f;
        mFramesWithNoMovement = (notMoving ? mFramesWithNoMovement + 1 : 0 );

        if ( currentVelocity < maxVelocity )
        {
            // Get the direction vector
            Vector3 direction = mPoints[mNextPoint] - gameObject.transform.position;

            // We only want to the vector in the xz plane
            direction.y = 0;

            // Get the unit vector
            Vector3.Normalize( direction );

            direction *= 2f;
            mRigidBody.AddForce( direction );
        }
    }

    // -----------------------------------------------------------
    // @Summary: adds random force in xy direction so that fruit
    //   fruit don't line up as easily.
    // @Return: void
    // -----------------------------------------------------------
    private void Randomness( )
    {
        // Get the direction vector
        Vector3 direction = mPoints[mNextPoint] - gameObject.transform.position;

        // We only want to the vector in the xz plane
        direction.y = 0;

        // Get the unit vector
        Vector3.Normalize( direction );

        // Get random left/right vector
        float randomAngle = Random.Range( -90, 90 );
        Vector3 force = Quaternion.Euler( 0, randomAngle, 0 ) * direction;

        // Apply random force
        mRigidBody.AddForce( force );
    }

    // -----------------------------------------------------------
    // @Summary: this function is called when the fruit is stuck 
    //   and not moving. It's intent is to get fruits to unstuck.
    // @Return: void
    // -----------------------------------------------------------
    private void MixUp( )
    {
        transform.Translate( -0.25f, 0.25f, 0.25f );
    }

    // -----------------------------------------------------------
    // @Summary: Applys randomness on a timer so that fruit can 
    //   can move side to side a little bit.
    // @Return: returns IEnumerator yield.
    // -----------------------------------------------------------
    private IEnumerator RandomnessGenerator()
    {
        while(true)
        {
            yield return new WaitForSeconds( RANDOMNESS_TIMER );
            Randomness();
        }
    }

    // -----------------------------------------------------------
    // Override the Start() method.
    // -----------------------------------------------------------
    void Start()
    {
        mPoints = FruitPath.GetInstance().GetPoints();
        mFruitScript = gameObject.GetComponent<BaseFruit>();

        if ( !mOverrideSpawn )
        {
            transform.position = mPoints[mNextPoint];
        }

        if ( !GetNextPoint() )
        {
            Debug.Log( "Only one point in the path..." );
        }

        // Start coroutine which adds randomness on a timer.
        StartCoroutine( RandomnessGenerator() );
    }

    // -----------------------------------------------------------
    // Override the FixedUpdate() method.
    // -----------------------------------------------------------
    void FixedUpdate()
    {
        if ( IsAtPoint() )
        {
            // If at last point
            if ( !GetNextPoint() )
            {
                Despawn();
            }
        }

        Accelerate();
        
        if (mFramesWithNoMovement > 50)
        {
            MixUp();
        }
    }
}

