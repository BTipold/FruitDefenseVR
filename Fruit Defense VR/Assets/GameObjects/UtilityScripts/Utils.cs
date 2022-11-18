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
using System.Collections.Generic;


public class Utils
{
    // -----------------------------------------------------------
    // @Summary: generates a bitmask that will ignore specified 
    //   layers and allow all others. 
    // @Param: layers - variable arguments. You can pass in 
    //   multiple layers to ignore. 
    // @Return: int - bitmask as an integer.
    // -----------------------------------------------------------
    public static int IgnoreBitmask( params string[] layers )
    {
        int bitmask = 0;

        foreach ( string layer in layers )
        {
            bitmask |= ( 1 << LayerMask.NameToLayer( layer ) );
        }

        return ~bitmask;
    }

    // -----------------------------------------------------------
    // @Summary: returns enum of correct type corresponding to 
    //   the string you passed in. 
    // @Param: value - enum value in string form. 
    // @Return: T - generic, will return any enum.
    // -----------------------------------------------------------
    public static T GetEnumFromStr<T>( string value )
    {
        T enum_to_return = default(T);

        foreach ( T item in Enum.GetValues( typeof(T) ) )
        {
            if ( item.ToString().Equals( value.Trim() ) )
            {
                enum_to_return = item;
            }
        }
        return enum_to_return;
    }

    // -----------------------------------------------------------
    // @Summary: Function to generate y value (between 0 and 255)
    //   for a given x.
    // @Param: amplitude - between 0 and 1 - scales the output.
    // @Param: x - between 0 and 1 - input.
    // @Param: curveType - the type of curve to use.
    // @Return: byte - y value (aka final vibration value)
    // -----------------------------------------------------------
    public static byte CalculateVibrationCurve( float amplitude, float x, Curve curveType )
    {
        byte y = 0;

        switch ( curveType )
        {
            case Curve.LINEAR:
                // y = amplitude * 0xFF * x
                y = (byte)( 0xFF * amplitude * x );
                break;

            case Curve.EXPONENTIAL:
                // y = amplitude * 0xFF * e^x
                y = (byte)( Math.Pow( 0xFF * amplitude, x ) );
                break;

            case Curve.COSINE:
                // y = amplitude * 0xFF / 2 * cos( PI x - PI ) + amplitude * 0xFF / 2
                y = (byte)( 0xFF / 2 * amplitude * ( Math.Cos( Math.PI * x - Math.PI ) ) + 0xFF / 2 * amplitude );
                break;

            case Curve.POLYNOMIAL:
                // y = -amplitude * 0xFF * (x - 1)^2 + amplitude * 0xFF
                y = (byte)( -0xFF * amplitude * Math.Pow( x - 1, 2 ) + 0xFF * amplitude );
                break;

            case Curve.NONE:
            default:
                // y = amplitude * 0xFF
                y = (byte)( 0xFF * amplitude );
                break;
        }

        return y;
    }


    public static Queue<T> CombineQueues<T>( Queue<T> listA, Queue<T> listB )
    {
        // List to return
        Queue<T> combinedList = new Queue<T>();

        // Set references to the longer/shorter lists out of A or B
        Queue<T> longerList = (listA.Count > listB.Count) ? listA : listB;
        Queue<T> shorterList = (listA.Count > listB.Count) ? listB : listA;

        // If the shorter list is empty, just quickly return the bigger list.
        if ( shorterList.Count == 0)
        {
            return longerList;
        }

        // Ratio of longerCount : shorterCount
        float listLengthRatio = (float)longerList.Count / (float)shorterList.Count;
        int longListCount = 0;
        int shortListCount = 0;

        while ( longListCount < longerList.Count ||
            shortListCount < shorterList.Count )
        {
            // Add extra elements until the lists are even (taking into
            // account the ratio of different lengths).
            while ( longListCount < shortListCount * listLengthRatio )
            {
                if ( longerList.Count > 0 )
                {
                    combinedList.Enqueue( longerList.Dequeue() );
                    longListCount++;
                }
            }

            // Add one element from the shorter list.
            if ( shorterList.Count > 0 )
            {
                combinedList.Enqueue( shorterList.Dequeue() );
                shortListCount++;
            }
        }

        return combinedList;
    }
}
