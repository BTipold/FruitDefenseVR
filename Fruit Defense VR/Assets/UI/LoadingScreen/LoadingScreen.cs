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
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public Image mLoadingBar;
    private string mScene;

    // Start is called before the first frame update
    void Start()
    {
        mLoadingBar.fillAmount = 0;
        mScene = GameState.GetMap().ToString();
        StartCoroutine( LoadScene() );
    }

    // Load Scene
    IEnumerator LoadScene()
    {
        AsyncOperation scene = SceneManager.LoadSceneAsync(mScene);
        while (scene.progress < 1)
        {
            mLoadingBar.fillAmount = scene.progress;
            yield return new WaitForEndOfFrame();
        }
    }
}
