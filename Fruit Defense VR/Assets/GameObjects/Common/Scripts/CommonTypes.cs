// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using System;
using UnityEngine;
using System.Collections.Generic;


namespace CommonTypes
{
    public enum FireType
    {
        UNKNOWN = -1,
        AUTO,
        SEMI,
        BURST
    }

    public enum VRHand
    {
        UNKNOWN = -1,
        ANY,
        RIGHT,
        LEFT
    }

    public enum WeaponClass
    {
        UNKNOWN = -1,
        LIGHT,
        HEAVY,
        MAGIC
    }

    public enum State
    {
        UNKNOWN = -1,
        ROUND_IN_PROGRESS,
        ROUND_END,
        GAME_WON,
        GAME_LOST,
        PAUSE,
    }

    public enum Curve
    {
        NONE,
        LINEAR,
        EXPONENTIAL,
        POLYNOMIAL,
        COSINE
    }

    public enum Axis
    {
        UNKNOWN,
        X,
        Y,
        Z
    }

    public enum Difficulty
    {
        UNKNOWN,
        EASY,
        MEDIUM,
        HARD,
        EXPERT,
        IMPOSSIBLE,
        TUTORIAL
    }

    public enum Map
    {
        UNKNOWN,
        MAIN_MENU,
        LOADING_SCREEN,
        COZY_CAMPFIRE,
        TUTORIAL
    }

    public enum Ctrl
    {
        ONLINE,
        OFFLINE,
        UNKNOWN
    }

    [Serializable]
    public struct Round
    {
        public int number;
        public List<Wave> waves;
    }

    [Serializable]
    public struct Wave
    {
        public Queue<GameObject> queue;
        public float time;
        public float spacing;

        public uint NumFruit() { return (uint)queue.Count; }
    }

    public class LightEnhancements
    {
        public static uint CASH = 1;
        public static uint DETECTION = 2;
        public static uint HIT = 3;
    }

    public enum Control
    {
        Fire,
        Hand,
        One,
        Two,
        Pause
    }

    public enum ButtonType
    {
        TRIGGER,
        HAND,
        TOP,
        BOT,
        MENU
    }

    public enum ControllerState
    {
        NOT_CONNECTED = 0,
        CONNECTING = 10,
        CONNECTED = 15
    }

    public class HandedType<T>
    {

        public T left;
        public T right;

        public HandedType()
        {
            left = default(T);
            right = default(T);
        }

        public HandedType( T type )
        {
            left = type;
            right = type;

        }

        public HandedType( T l, T r )
        {
            left = l;
            right = r;
        }

        public T this[VRHand hand]
        {
            get 
            {
                if (hand == VRHand.RIGHT) return right;
                else if (hand == VRHand.LEFT) return left;
                else return default(T);
            }
            set 
            {
                if (hand == VRHand.RIGHT) right = value;
                else if (hand == VRHand.LEFT) left = value;
            }
        }

        public T Get( VRHand hand )
        {
            if ( hand == VRHand.LEFT ) return left;
            else if ( hand == VRHand.RIGHT ) return right;
            return default(T);
        }

        public void Set( VRHand hand, T val )
        {
            if ( hand == VRHand.LEFT ) { left = val; }
            else if ( hand == VRHand.RIGHT ) { right = val; }
            else if ( hand == VRHand.ANY ) { left = val; right = val; }
        }

        public void Set(T l, T r)
        {
            left = l;
            right = r;
        }
    }

    /// <summary>
    /// Since unity doesn't flag the Vector3 as serializable, we
    /// need to create our own version. This one will automatically convert
    /// between Vector3 and SerializableVector3
    /// </summary>
    [System.Serializable]
    public class SerializableVector3
    {
        public float x;
        public float y;
        public float z;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rX"></param>
        /// <param name="rY"></param>
        /// <param name="rZ"></param>
        public SerializableVector3(float rX, float rY, float rZ)
        {
            x = rX;
            y = rY;
            z = rZ;
        }

        /// <summary>
        /// Returns a string representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("[{0}, {1}, {2}]", x, y, z);
        }

        /// <summary>
        /// Automatic conversion from SerializableVector3 to Vector3
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator Vector3(SerializableVector3 rValue)
        {
            return new Vector3(rValue.x, rValue.y, rValue.z);
        }

        /// <summary>
        /// Automatic conversion from Vector3 to SerializableVector3
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator SerializableVector3(Vector3 rValue)
        {
            return new SerializableVector3(rValue.x, rValue.y, rValue.z);
        }
    }
}