// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using CommonTypes;
using UnityEngine;
using UnityEngine.InputSystem;


public class MainMenuUIManager : MonoBehaviour
{
    public GameObject mMainCanvas;
    public GameObject mMenuHandler;
    public GameObject mPrTutorialMainMenu;
    public GameObject mPrMainMenu;

    private GameObject mObMenu = null;

    private Vector3 mMenuPosition;
    private Player mPlayer = null;


    // -----------------------------------------------------------
    // @Summary: Function to move a menu exactly infront of the
    //   player and translate it back by an amount.
    // @Param: menu - GameObject of the menu to modify.
    // @Param: translation - float indicating how far back to set
    //   the menu. 
    // @Return: void
    // -----------------------------------------------------------
    void positionMenu( GameObject menu, float translation )
    {
        float playerRotation = mPlayer.GetPlayerPosition().eulerAngles.y;
        menu.transform.rotation = Quaternion.Euler( 0, playerRotation, 0 );
        menu.transform.position = new Vector3(mPlayer.GetPlayerPosition().position.x, mPlayer.GetPlayerPosition().position.y + 1.0f, mPlayer.GetPlayerPosition().position.z);
        menu.transform.position += menu.transform.forward * translation;
    }
    // -----------------------------------------------------------
    // Instatiate either the tutorial menu or regular menu
    // -----------------------------------------------------------
    private void OnEnable()
    {
        //positionMenu(mMenuHandler, 5);
        //DataManager.SaveTutorial(true);
/*        if (DataManager.LoadTutorial())
            mObMenu = Instantiate(mPrMainMenu, mMainCanvas.transform);
        else
            mObMenu = Instantiate(mPrMainMenu, mMainCanvas.transform);*/
    }

    private void Start()
    {
        mPlayer = Player.GetInstance();
    }
}
