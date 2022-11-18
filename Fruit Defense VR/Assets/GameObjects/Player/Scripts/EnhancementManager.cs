// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using CommonTypes;
using UnityEngine;
using System;

[Serializable]
public class EnhancementManager
{
    private uint[] left  = new uint[4] { 0, 0, 0, 0 };
    private uint[] right = new uint[4] { 0, 0, 0, 0 };

    // -----------------------------------------------------------
    // @Summary: Increments integer in array position specified
    //   by category. The integer represents the level of 
    //   enhancement that is currently owned.
    // @Param: hand - which hand to buy an enhancement for.
    // @Param: category - which category of enhancement to add.
    //   1 = TOP, 2 = MID, 3 = BOTTOM.
    // @Return: bool - returns true if successful
    // -----------------------------------------------------------
    public bool Add( VRHand hand, uint category )
    {
        bool status;

        switch ( hand )
        {
            case VRHand.LEFT:
                status = AddLeft( category );
                break;

            case VRHand.RIGHT:
                status = AddRight( category );
                break;

            case VRHand.ANY:
            case VRHand.UNKNOWN:
            default:
                Debug.LogError( "Trying to add enhancement but hand is UNKNOWN." );
                status = false;
                break;
        }

        return status;
    }
        
    // -----------------------------------------------------------
    // @Summary: Decrements integer in array position specified
    //   by category. The integer represents the level of 
    //   enhancement that is currently owned.
    // @Param: hand - which hand to sell an enhancement for.
    // @Param: category - which category of enhancement to add.
    //   1 = TOP, 2 = MID, 3 = BOTTOM.
    // @Return: bool - returns true if successful
    // -----------------------------------------------------------
    public bool Sell( VRHand hand, uint category )
    {
        bool status = false;

        switch ( hand )
        {
            case VRHand.LEFT:
                status = SellLeft( category );
                break;

            case VRHand.RIGHT:
                status = SellRight( category );
                break;

            case VRHand.UNKNOWN:
            default:
                Debug.LogError( "Trying to sell enhancement but hand is UNKNOWN." );
                status = false;
                break;
        }

        return status;
    }

    // -----------------------------------------------------------
    // @Summary: Sets enhancement level on left hand.
    // @Param: category - which category of enhancement to add.
    //   1 = TOP, 2 = MID, 3 = BOTTOM.
    // @Return: bool - returns true if successful
    // -----------------------------------------------------------
    public bool SetLeft( uint category, uint level )
    {
        bool status = true;

        if ( category >= 0 && category <= 3 && left[category] < 2 )
        {
            left[category] = level;
            Debug.Log( "Added new enhancement" );
        }
        else
        {
            status = false;
            Debug.LogError( "Trying to add an enhancement that is out of range. Must be between 0 and 3" );
        }

        return status;
    }

    // -----------------------------------------------------------
    // @Summary: Sets enhancement level on right hand.
    // @Param: category - which category of enhancement to add.
    //   1 = TOP, 2 = MID, 3 = BOTTOM.
    // @Return: bool - returns true if successful
    // -----------------------------------------------------------
    public bool SetRight( uint category, uint level )
    {
        bool status = true;

        if ( category >= 0 && category <= 3 && right[category] < 2 )
        {
            right[category] = level;
            Debug.Log( "Added new enhancement" );
        }
        else
        {
            status = false;
            Debug.LogError( "Trying to add an enhancement that is out of range. Must be between 0 and 3" );
        }

        return status;
    }

    // -----------------------------------------------------------
    // @Summary: Adds new enhancement to left hand.
    // @Param: category - which category of enhancement to add.
    //   1 = TOP, 2 = MID, 3 = BOTTOM.
    // @Return: bool - returns true if successful
    // -----------------------------------------------------------
    public bool AddLeft( uint category )
    {
        bool status = true;

        if ( category >= 0 && category <= 3 && left[category] < 2 )
        {
            left[category]++;
            Debug.Log( "Added new enhancement" );
        }
        else
        {
            status = false;
            Debug.LogError( "Trying to add an enhancement that is out of range. Must be between 0 and 3" );
        }

        return status;
    }

    // -----------------------------------------------------------
    // @Summary: Adds new enhancement to right hand.
    // @Param: category - which category of enhancement to add.
    //   1 = TOP, 2 = MID, 3 = BOTTOM.
    // @Return: bool - returns true if successful
    // -----------------------------------------------------------
    public bool AddRight( uint category )
    {
        bool status = true;

        if ( category >= 0 && category <= 3 && right[category] <= 2 )
        {
            right[category]++;
        }
        else
        {
            status = false;
            Debug.LogError( "Trying to add an enhancement that is out of range. Must be between 0 and 3" );
        }

        return status;
    }

    // -----------------------------------------------------------
    // @Summary: Sells new enhancement on left hand.
    // @Param: category - which category of enhancement to sell.
    //   1 = TOP, 2 = MID, 3 = BOTTOM.
    // @Return: bool - returns true if successful
    // -----------------------------------------------------------
    public bool SellLeft( uint category )
    {
        bool status = true;

        if ( category >= 0 && category <= 3 && left[category] > 0)
        {
            left[category]--;
            Debug.Log( "Added new enhancement" );
        }
        else
        {
            status = false;
            Debug.LogError( "Trying to sell an enhancement that is out of range. Must be between 0 and 3" );
        }

        return status;
    }

    // -----------------------------------------------------------
    // @Summary: Sells new enhancement to right hand.
    // @Param: category - which category of enhancement to sell.
    //   1 = TOP, 2 = MID, 3 = BOTTOM.
    // @Return: bool - returns true if successful
    // -----------------------------------------------------------
    public bool SellRight( uint category )
    {
        bool status = true;

        if ( category >= 0 && category <= 3 && right[category] > 0 )
        {
            right[category]--;
        }
        else
        {
            status = false;
            Debug.LogError( "Trying to sell an enhancement that is out of range. Must be between 0 and 3" );
        }

        return status;
    }

    // -----------------------------------------------------------
    // @Summary: Checks if the hand has a specific enhancement 
    //    level.
    // @Param: hand - hand to check.
    // @Param: category - which category of enhancement to add.
    //   1 = TOP, 2 = MID, 3 = BOTTOM.
    // @Param: desiredEnahncementLevel - enhancement level to 
    //   check.
    // @Return: bool - returns true if the enhancement is owned.
    // -----------------------------------------------------------
    public bool Check( VRHand hand, uint category, uint desiredEnhancementLevel )
    {
        bool enhancementActive = Get(hand, category) >= desiredEnhancementLevel;
        return enhancementActive;
    }

    // -----------------------------------------------------------
    // @Summary: Gets the hands enhancement level. 
    // @Param: hand - hand to retrieve. If hand is any, it 
    //   returns whichever hand has greater enhancement level.
    // @Param: category - which category of enhancement to get.
    //   1 = TOP, 2 = MID, 3 = BOTTOM.
    // @Return: uint - returns the enhancement level.
    // -----------------------------------------------------------
    public uint Get( VRHand hand, uint category )
    {
        uint enhancement = 0;

        if ( category >= 0 && category <= 3 )
        {
            uint leftEnhancement = left[category];
            uint rightEnhancement = right[category];

            if ( hand == VRHand.LEFT )
            {
                enhancement = leftEnhancement;
            }
            else if ( hand == VRHand.RIGHT )
            {
                enhancement = rightEnhancement;
            }
            else if(hand == VRHand.ANY)
            {
                enhancement = ( rightEnhancement > leftEnhancement ) ? rightEnhancement : leftEnhancement;
            }
        }
        else
        {
            Debug.LogError( "Trying to read an enhancement that is out of range. Must be between 0 and 3" );
        }

        return enhancement;
    }
}