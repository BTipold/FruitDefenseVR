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
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CommonTypes;

public class Popup2 : BaseMenu
{
    public void MainMenu()
    {
        Debug.Log("Taking you to the Main Menu...");
        GameState.SetMap(Map.MAIN_MENU);
        SceneManager.LoadScene("LoadingScreen");
    }

    public void Continue()
    {
        Destroy(this.gameObject);
    }
}
