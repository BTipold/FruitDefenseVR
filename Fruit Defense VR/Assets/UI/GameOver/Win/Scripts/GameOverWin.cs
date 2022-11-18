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
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverWin : MonoBehaviour
{
    public void MainMenu()
    {
        Debug.Log("Taking you to the Main Menu...");
        PlayerPrefs.SetString("Scene", "MainMenu");
        SceneManager.LoadScene("LoadingScreen");
    }

    public void NextRound()
    {
        Debug.Log("Starting the Next Round...");
        GameState.GetInstance().NextRound();
        Destroy(this.gameObject);
    }
    public void Restart()
    {
        Debug.Log("Reloading scene...");
        Scene scene = SceneManager.GetActiveScene();
        PlayerPrefs.SetString("Scene", scene.name);
        SceneManager.LoadScene("LoadingScreen");
    }
}
