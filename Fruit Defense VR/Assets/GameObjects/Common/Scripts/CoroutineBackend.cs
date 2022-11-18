using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class CoroutineBackend : MonoBehaviour
{
    private Dictionary<uint, IEnumerator> mCoroutines = new Dictionary<uint, IEnumerator>();
    private static CoroutineBackend mInstance = null;
    private uint mNextFreeNumber = 0;

    public static CoroutineBackend Instance()
    {
        return mInstance;
    }

    public uint DoWhen(Action doThis, Func<bool> whenThis, Action<uint> whenDone)
    {
        uint id = GetNextNumber();
        IEnumerator coroutine()
        {
            yield return new WaitUntil(whenThis);
            doThis();
            mCoroutines.Remove(id);
            whenDone(id);
        }
        IEnumerator handle = coroutine();
        mCoroutines.Add(id, handle);
        StartCoroutine(handle);
        return id;
    }

    public void DoAfter(Action doThis, float seconds)
    {
        IEnumerator coroutine()
        {
            yield return new WaitForSeconds(seconds);
            doThis();
        }
        IEnumerator handle = coroutine();
        StartCoroutine(handle);
    }

    public bool Kill(uint id)
    {
        bool status = false;
        var coroutine = mCoroutines.GetValueOrDefault(id, null);
        if (coroutine != null)
        {
            status = true;
            StopCoroutine(coroutine);
        }
        return status;
    }

    private uint GetNextNumber()
    {
        return mNextFreeNumber++;
    }
    private void Awake()
    {
        mInstance = this;
    }
}
