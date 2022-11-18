// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------

using UnityEngine;
using UnityEngine.SceneManagement;
using CommonTypes;

public class PauseManager : MonoBehaviour
{
    public void Start()
    {
    }
    public void MainMenu()
    {
        Debug.Log("Taking you to the Main Menu...");
        GameState.SetMap( Map.MAIN_MENU );
        SceneManager.LoadScene("LoadingScreen");
    }

    public void Resume()
    {
        Debug.Log("Resuming Game...");
        GetComponent<TransitionAnimation>().ScaleOut();
        GameState.GetInstance().UnPause();
    }

    public void TeleportMode()
    {
        //Player.GetInstance().TeleportMode();
        //Resume();
        Player.GetInstance().AddCash( 500 );
    }
}

