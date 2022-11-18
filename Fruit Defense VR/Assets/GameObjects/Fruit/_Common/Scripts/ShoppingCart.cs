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

public class ShoppingCart : BaseFruit
{
    // -----------------------------------------------------------
    // Member Variables 
    // -----------------------------------------------------------
    public  GameObject    mChild1;
    public  GameObject    mChild2;
    public  GameObject    mChild3;
    public  GameObject    mChild4;
    private int           mNextPoint      = 0;
    private List<Vector3> mPoints         = null;
    private BaseFruit     mFruitScript    = null;
    private const float   CLOSE_THRESHOLD = 1f;

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
    private void MoveTowards()
    {
        // Get the direction vector
        Vector3 direction = mPoints[mNextPoint] - gameObject.transform.position;

        // The step size is equal to speed times frame time.
        float singleStep = mSpeed * Time.deltaTime;

        // Rotate the forward vector towards the target direction by one step
        Vector3 newDirection = Vector3.RotateTowards(gameObject.transform.forward, direction, singleStep, 0.0f);

        // Calculate a rotation a step closer to the target and applies rotation to this object
        transform.rotation = Quaternion.LookRotation( newDirection );

        gameObject.transform.Translate( 0, 0, 0.02f );
    }

    public override void Destroy()
    {
        Transform pos1 = gameObject.transform;    
        Transform pos2 = gameObject.transform;
        Transform pos3 = gameObject.transform;
        Transform pos4 = gameObject.transform;

        pos1.Translate(0.25f, -0.25f, 0.25f);
        pos2.Translate(-0.25f, -0.25f, -0.25f);
        pos3.Translate(-0.25f, 0.25f, 0.25f );
        pos4.Translate(0.25f, 0.25f, -0.25f);

        GameObject child1 = Instantiate( mChild1 );
        GameObject child2 = Instantiate( mChild2 );
        GameObject child3 = Instantiate( mChild3 );
        GameObject child4 = Instantiate( mChild4 );

        child1.transform.SetParent( gameObject.transform.parent );
        child2.transform.SetParent( gameObject.transform.parent );
        child3.transform.SetParent( gameObject.transform.parent );
        child4.transform.SetParent( gameObject.transform.parent );

        child1.GetComponent<FollowPath>().SetPosition( pos1.position, mNextPoint );
        child2.GetComponent<FollowPath>().SetPosition( pos2.position, mNextPoint );
        child3.GetComponent<FollowPath>().SetPosition( pos3.position, mNextPoint );
        child4.GetComponent<FollowPath>().SetPosition( pos4.position, mNextPoint );

        base.Destroy();
    }

    // -----------------------------------------------------------
    // Override the Start() method.
    // -----------------------------------------------------------
    void Start()
    {
        mPoints = FruitPath.GetInstance().GetPoints();
        mFruitScript = gameObject.GetComponent<BaseFruit>();
        transform.position = mPoints[mNextPoint];

        if ( !GetNextPoint() )
        {
            Debug.Log( "Only one point in the path..." );
        }
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
        MoveTowards();
    }
}
