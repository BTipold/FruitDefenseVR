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

public class RoundMenu : BaseMenu
{
    Player mPlayer;
    public GameObject mHandMenu;
    public GameObject mRoundMenu;
    private GameState mGameState = null;

    private void Start()
    {
        mGameState = GameState.GetInstance();
        if (mGameState.GetRoundNumber() == 0)
        {
            Debug.Log("Setting first round hand menu");
            mHandMenu.SetActive(true);
            mRoundMenu.SetActive(false);
        }
    }

    public void MainMenu()
    {
        Debug.Log("Taking you to the Main Menu...");
        GameState.SetMap(Map.MAIN_MENU);
        SceneManager.LoadScene(Map.LOADING_SCREEN.ToString());
    }

    public void NextRound()
    {
        Debug.Log("Starting the Next Round...");
        GameState.GetInstance().NextRound();
        Destroy(this.gameObject);
    }

    public void Restart()
    {
        Debug.Log( "Reloading scene..." );

        // Create instance of saveload to interface with the save system
        SaveLoadInterface saveInterface = new SaveLoadInterface();
        saveInterface.RemoveCurrentSave();

        SceneManager.LoadScene(Map.LOADING_SCREEN.ToString());
    }
}
