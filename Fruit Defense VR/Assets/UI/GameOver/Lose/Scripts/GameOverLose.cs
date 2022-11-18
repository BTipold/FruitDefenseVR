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
using CommonTypes;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverLose : MonoBehaviour
{
    public void MainMenu()
    {
        Debug.Log("Taking you to the Main Menu...");
        GameState.SetMap( Map.MAIN_MENU );
        SceneManager.LoadScene("LoadingScreen");
    }

    public void Restart()
    {
        Debug.Log("Reloading scene...");
        /*        Scene scene = SceneManager.GetActiveScene();
                DataManager.SetMap( scene.name );*/
        SceneManager.LoadScene("LoadingScreen");
    }
}
