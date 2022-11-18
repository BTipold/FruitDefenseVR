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

public class GameOverWinText : MonoBehaviour
{
    private string roundNumS;
    private TextMeshProUGUI mRoundNum;
    // Start is called before the first frame update
    void Start()
    {
        mRoundNum = GetComponent<TextMeshProUGUI>();
        roundNumS = "You Beat " + PlayerPrefs.GetString("Scene") + " on " + PlayerPrefs.GetString("Difficulty") + "!";
        mRoundNum.text = roundNumS;
    }

}
