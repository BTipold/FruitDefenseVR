// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using System.Collections.Generic;
using UnityEngine;


public sealed class FruitPath : MonoBehaviour
{
    // -----------------------------------------------------------
    // Member Variables
    // -----------------------------------------------------------
    private static FruitPath    m_instance  = null;
    private List<Vector3>       m_points    = new List<Vector3>();

    // -----------------------------------------------------------
    // @Summary: finds all gameobjects in children tagged as 
    //   "Point" and adds their positions to a List. 
    // @Return: bool - returns true if path built successfully.
    // -----------------------------------------------------------
    private bool InitPath()
    {
        bool status = true;

        Debug.Log( "initializing path..." );

        // Get the transforms of all points in the path
        Transform[] transform_points = GetComponentsInChildren<Transform>();

        if ( transform_points != null )
        {
            foreach ( Transform child in transform_points )
            {
                if ( child.gameObject.tag == "Point" )
                {

                    // Save the positions into a list
                    m_points.Add( child.position );
                }
            }
            Debug.Log( "Num of points: " + m_points.Count );
        }
        else
        {
            status = false;
        }

        return status;
    }

    // -----------------------------------------------------------
    // @Summary: getter for instance of this singleton.
    // @Return: FruitPath - returns reference to the only inst of
    //   this class. 
    // -----------------------------------------------------------
    public static FruitPath GetInstance()
    {
        return m_instance;
    }

    // -----------------------------------------------------------
    // @Summary: getter for list of points.
    // @Return: List<Vector3> - returns list of points.
    // -----------------------------------------------------------
    public List<Vector3> GetPoints()
    {
        return m_points;
    }

    // -----------------------------------------------------------
    // @Summary: returns the first point in the path (aka the 
    //   spawn point).
    // @Return: Vector3 - position of spawn point.
    // -----------------------------------------------------------
    public Vector3 GetSpawnPoint()
    {
        return m_points[0];
    }

    // -----------------------------------------------------------
    // Override the Awake() method.
    // -----------------------------------------------------------
    void Awake()
    {
        bool status = true;

        if ( m_instance == null )
        {
            m_instance = this;
        }

        status &= InitPath();

        if ( !status )
        {
            Debug.Log( "Could not initialize path" );
        }
    }
}
