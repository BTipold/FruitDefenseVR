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
using TMPro;

public class GetLives : MonoBehaviour
{
    public TextMeshProUGUI mLives;

    private void OnEnable()
    {
        mLives.text = "¬ " + GameState.GetInstance().GetHealth().ToString();
    }


    void Start()
    {
        mLives.text = "¬ " + GameState.GetInstance().GetHealth().ToString();
    }
}
