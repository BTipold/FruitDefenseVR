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

public class GetCurrency : MonoBehaviour
{
    private Player mPlayer = null;
    public TextMeshProUGUI mMoney;

    private void OnEnable()
    {
        mMoney.text = "$ " + Player.GetInstance().GetCash().ToString();
    }


    void Start()
    {
        mPlayer = Player.GetInstance();
        mMoney.text = "$ " + mPlayer.GetCash().ToString();
    }
}
