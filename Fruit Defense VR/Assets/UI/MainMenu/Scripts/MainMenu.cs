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
using System;

public class MainMenu : MonoBehaviour
{
    private Map mMap;

    //public GameObject NewOrResume; // New or Resume Game Popup

    public void Play()
    {
        Debug.Log("Starting game...");
        if ( !ManageTutorial.IsTutorialComplete() )
        {
            GameState.SetDifficulty(Difficulty.TUTORIAL);
        }
        SceneManager.LoadScene(Map.LOADING_SCREEN.ToString());
    }

    public void setMap( string mapStr )
    {
        Map map = (Map)Enum.Parse(typeof(Map), mapStr, true);
        GameState.SetMap(map);
        mMap = map;
    }

    public void setDifficulty(string difficultyStr)
    {
        Difficulty difficulty = (Difficulty)Enum.Parse(typeof(Difficulty), difficultyStr, true);
        GameState.SetDifficulty(difficulty);

        if (SaveLoadInterface.SaveFileExists(mMap, difficulty))
        {
            //NewOrResume.SetActive(true);
            Play();
        }
        else
        {
            Play();
        }
    }
}
