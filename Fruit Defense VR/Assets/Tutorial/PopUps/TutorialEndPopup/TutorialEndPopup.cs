// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using CommonTypes;

public class TutorialEndPopup : BaseMenu
{
    public void MainMenu()
    {
        Debug.Log("Taking you to the Main Menu...");
        GameState.SetMap(Map.MAIN_MENU);
        SceneManager.LoadScene("LoadingScreen");
    }

    public void Restart()
    {
        // Create instance of saveload to interface with the save system
        SaveLoadInterface saveInterface = new SaveLoadInterface();
        saveInterface.RemoveCurrentSave();
        
        Debug.Log("Reloading scene...");
        GameState.SetMap(Map.TUTORIAL);
        SceneManager.LoadScene("LoadingScreen");
    }
}
