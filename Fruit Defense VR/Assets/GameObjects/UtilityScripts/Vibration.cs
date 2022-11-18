using UnityEngine;
using System;
using CommonTypes;


public class Vibration
{
    // -----------------------------------------------------------
    // @Summary: vibration preset for constructing a raw byte 
    //   array from scratch. 
    // -----------------------------------------------------------
    [Serializable]
    public struct VibrationPreset
    {
        public float amplitude;
        public float duration;
        public float midpoint;
        public Curve up;
        public Curve down;
    }

    private byte[] mBuffer;

    // -----------------------------------------------------------
    // @Summary: getter for raw data.
    // @Return: byte[] - returns byte array.
    // -----------------------------------------------------------
    public byte[] GetData()
    {
        return mBuffer;
    }

    // -----------------------------------------------------------
    // @Summary: validity check.
    // @Return: bool - returns true if buffer is initialized.
    // -----------------------------------------------------------
    public bool IsValid()
    {
        return mBuffer != null;
    }

    // -----------------------------------------------------------
    // @Summary: getter for vibration samples.
    // @Return: int - returns number of samples in vibration.
    // -----------------------------------------------------------
    public int GetLength()
    {
        if ( mBuffer == null )
        {
            return -1;
        } else {
            return mBuffer.Length;
        }
    }

    // -----------------------------------------------------------
    // @Summary: constructs a vibration from a preset.
    // @Param: amplitude - betwen 0 and 1.
    // @Param: duration - in seconds.
    // @Param: midpoint - point at which max value is reached.
    // @Param: up - curve type to use when ramping up.
    // @Param: down - curve type to use when ramping down.
    // @Return: void
    // -----------------------------------------------------------
    public Vibration( VibrationPreset preset )
    {
        // Convert duration into number of samples, init array
        int samples = (int)(preset.duration / 0.002f);

        if (samples > 0)
        {
            mBuffer = new byte[samples];

            // index of highest amplitude
            int peakIndex = (int)(preset.midpoint * samples);

            // Set bytes as curve increases
            for ( int i = 0; i < peakIndex; i++ )
            {
                float x = i / peakIndex;
                mBuffer[i] = Utils.CalculateVibrationCurve( preset.amplitude, x, preset.up );
            }

            // Set bytes as curve decreases
            for ( int i = peakIndex; i < samples; i++ )
            {
                float x = 1.0f - ((i - peakIndex) / (float)(samples - peakIndex));
                mBuffer[i] = Utils.CalculateVibrationCurve( preset.amplitude, x, preset.down );
            }
        }
    }

    // -----------------------------------------------------------
    // @Summary: constructor that takes an audio clip and 
    //   converts it to a byte array.
    // @Param: clip - audio clip data
    // @Return: void
    // -----------------------------------------------------------
    public Vibration( AudioClip clip )
    {
        float[] data = new float[clip.samples];
        mBuffer = new byte[clip.samples];

        bool success = clip.GetData(data, 0);
        if (success)
        {
            for ( int i = 0; i < data.Length; ++i )
            {
                mBuffer[i] = (byte) Mathf.Round(Mathf.Abs( data[i]) * 255.0f);
            }
        }
    }

    // -----------------------------------------------------------
    // @Summary: constructor that takes a raw byte array.
    // @Param: buffer - raw byte array
    // @Return: void
    // -----------------------------------------------------------
    public Vibration( byte[] buffer )
    {
        mBuffer = buffer;
    }
}
