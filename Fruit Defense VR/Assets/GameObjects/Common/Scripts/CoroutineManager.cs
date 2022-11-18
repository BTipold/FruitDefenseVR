using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class CoroutineManager
{
    private HashSet<uint> mCoroutines = new HashSet<uint>();
    private CoroutineBackend mBackend = null;

    public CoroutineManager()
    {
        mBackend = CoroutineBackend.Instance();
    }

    public uint DoWhen(Action doThis, Func<bool> whenThis)
    {
        uint handle = mBackend.DoWhen(doThis, whenThis, (id) => {
            mCoroutines.Remove(id);
        });
        mCoroutines.Add(handle);
        return handle;
    }

    public void DoAfter(Action doThis, float seconds)
    {
        mBackend.DoAfter(doThis, seconds);
    }

    public void KillAll()
    {
        foreach (uint id in mCoroutines)
        {
            mBackend.Kill(id);
        }
    }
}
