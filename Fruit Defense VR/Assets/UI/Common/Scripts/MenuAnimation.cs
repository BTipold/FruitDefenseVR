// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAnimation : MonoBehaviour
{
    public Animator mAnimator;
    public string closeAnim;
    public string openAnim;

    // Start is called before the first frame update
    
    void OnEnable()
    {
        if ( mAnimator )
        {
            mAnimator.SetTrigger("Open");
            mAnimator.ResetTrigger("Close");
        }
    }

    // Update is called once per frame
    void close()
    {
        mAnimator.SetTrigger("Close");
        mAnimator.ResetTrigger("Open");
    }

    void disable()
    {
        gameObject.SetActive(false);
    }
}
