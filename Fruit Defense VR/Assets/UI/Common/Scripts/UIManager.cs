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


public class UIManager : MonoBehaviour
{
    public  GameObject mMainCanvas;
    public  GameObject mMenuHandler;
    public  GameObject mPrPauseMenu;
    public  GameObject mPrRoundMenu;
    public GameObject mPrGameOverWin;
    public GameObject mPrGameOverLose;

    private GameObject mObPauseMenu = null;
    private GameObject mObRoundMenu = null;
    private GameObject mObGameOverWin = null;
    private GameObject mObGameOverLose = null;

    private Vector3 mMenuPosition;
    Player mPlayer = null;


    public void Resume()
    {
        Debug.Log("Resuming Game...");
        Destroy(mObPauseMenu);
        mObPauseMenu = null;
        GameState.GetInstance().UnPause();
    }

    void Pause()
    {
        positionMenu( mMenuHandler, 5 );
        mObPauseMenu = Instantiate(mPrPauseMenu, mMainCanvas.transform);
        // positionMenu(mObPauseMenu, 3);
        GameState.GetInstance().Pause();
    }

    void GameOverWin()
    {
        positionMenu(mMenuHandler, 5);
        mObGameOverWin = Instantiate(mPrGameOverWin, mMainCanvas.transform);
        //positionMenu(mObGameOverWin, 4);
    }

    void GameOverLose()
    {
        positionMenu(mMenuHandler, 5);
        mObGameOverLose = Instantiate(mPrGameOverLose, mMainCanvas.transform);
        //positionMenu(mObGameOverLose, 4);
    }

    void RoundEnd()
    {
        positionMenu(mMenuHandler, 5);
        mObRoundMenu = Instantiate(mPrRoundMenu, mMainCanvas.transform);
        //positionMenu( mObRoundMenu, 4 );
    }

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

    private void HandleStateChange( EventStateChange e )
    {
        mPlayer = Player.GetInstance();
        switch ( e.newState )
        {
            case State.ROUND_END:
                if ( e.prevState != State.PAUSE)
                {
                    RoundEnd();
                }
                break;

            case State.GAME_LOST:
                if (e.prevState != State.PAUSE)
                {
                    GameOverLose();
                }
                break;

            case State.GAME_WON:
                if (e.prevState != State.PAUSE)
                {
                    GameOverWin();
                }
                break;

            case State.PAUSE:
            default:
                break;
        }
    }
    private void handleMenuBtnDown( InputAction.CallbackContext context )
    { 
        if (mObPauseMenu == null && GameState.GetInstance().GetState() == State.ROUND_IN_PROGRESS)
        {
            Pause();
        }
        else
        {
            Resume();
        }
    }

    // -----------------------------------------------------------
    // Override the OnEnable() function.
    // -----------------------------------------------------------
    private void OnEnable()
    {
        Events.Instance.AddListener<EventStateChange>(HandleStateChange);
        ControllerProxy.Instance().RegisterCallbackMenuDown(VRHand.LEFT, handleMenuBtnDown);
    }

    // -----------------------------------------------------------
    // Override the OnDisable() function.
    // -----------------------------------------------------------
    private void OnDisable()
    {
        Events.Instance.RemoveListener<EventStateChange>(HandleStateChange);
        ControllerProxy.Instance().RemoveCallbackMenuDown(VRHand.LEFT, handleMenuBtnDown);
    }
    private void Start()
    {
        mPlayer = Player.GetInstance();
    }
}
