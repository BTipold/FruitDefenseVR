// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using CommonTypes;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Threading;
using System.Security.Cryptography;
using System.Linq;

public class ManageTutorial : MonoBehaviour
{
    // -----------------------------------------------------------
    // Member Variables
    // -----------------------------------------------------------
    private static PersistentDataType<bool> mIsTutorialCompleted = new PersistentDataType<bool>(true, "istutorialcomplete");

    private GameState mGameState = null;
    private SpawnManager mSpawnManager = null;
    private Dictionary<uint, Func<IEnumerator>> mRoundFPtrMap = new Dictionary<uint, Func<IEnumerator>>();
    private Dictionary<uint, Func<IEnumerator>> mInBtwnFPtrMap = new Dictionary<uint, Func<IEnumerator>>();
    private CoroutineManager mCoroutineManager = null;

    public ManageGame mGameManager = null;
    public GameObject mUi = null;
    public GameObject mRoundArrowsPrefab = null;
    public GameObject mHelperArrowPrefab = null;
    public GameObject mPopupReload1 = null; 
    public GameObject mPopupReload2 = null; 
    public GameObject mPopup1 = null;
    public GameObject mPopup2 = null;
    public GameObject mRoundMenu = null;
    public GameObject mWelcomeMessage = null;
    public GameObject mTutorialEndPopup = null;

    static ManageTutorial()
    {
        // If property isn't in the database, assign the default value "incomplete"
        mIsTutorialCompleted.RegisterLoadFailedHandler(() =>
        {
            mIsTutorialCompleted.Assign(false);
            mIsTutorialCompleted.Save();
        });
    }

    public static bool IsTutorialComplete()
    {
        mIsTutorialCompleted.Load();
        return mIsTutorialCompleted;
    }

    // -----------------------------------------------------------
    // @Summary: Handler function for state change event. Takes
    //   action for each new event.
    // @Param: e - Event data which contains the new state
    //   and previous state.
    // @Return: void.
    // -----------------------------------------------------------
    void HandleStateChange(EventStateChange e)
    {
        switch (e.newState)
        {
            case State.PAUSE:
                Pause();
                Debug.Log( "Game Paused" );
                break;

            case State.ROUND_IN_PROGRESS:
                Debug.Log( "Round Started" );
                var roundCoroutine = mRoundFPtrMap.GetValueOrDefault(mGameState.GetRoundNumber(), null);
                if (roundCoroutine != null)
                {
                    StartCoroutine(roundCoroutine());
                } else {
                    mGameState.SetState(State.GAME_WON);
                }
                break;

            case State.ROUND_END:
                Debug.Log( "In-between Rounds" );
                if (mCoroutineManager != null) mCoroutineManager.KillAll();
                if ( mGameState.GetRoundNumber() >= mGameState.GetMaxRound() )
                {
                    mGameState.SetState( State.GAME_WON );
                    break;
                }

                var menuCoroutine = mInBtwnFPtrMap.GetValueOrDefault(mGameState.GetRoundNumber(), null);
                if (menuCoroutine != null)
                {
                    StartCoroutine(menuCoroutine());
                } else {
                    mGameState.SetState(State.GAME_WON);
                }
                break;

            case State.GAME_WON:
                mIsTutorialCompleted.Assign(true);
                mIsTutorialCompleted.Save();
                Instantiate(mTutorialEndPopup, mUi.transform);
                Debug.Log( "Game won." );
                break;

            case State.GAME_LOST:
                Debug.Log( "Game lost." );
                break;

            case State.UNKNOWN:
                Debug.Log( "Unknown game state." );
                break;

            default:
                Debug.Log( "Game state not a valid enum" );
                break;
        }
    }

    private void Pause()
    {

    }

    // -----------------------------------------------------------
    // @Summary: Creates an arrow popup with text. Popup will be
    //   centered around the game object of focus.
    // @Param: btn - btn of focus.
    // @Param: angle - angle to set the arrow.
    // @Param: message - popup text.
    // @Return: GameObject - returns the arrow popup obj.
    // -----------------------------------------------------------
    private GameObject CreateArrowPopup(GameObject btn, float angle, string message = "")
    {
        GameObject arrowObj = Instantiate(mHelperArrowPrefab, btn.transform.parent);
        arrowObj.transform.position = btn.transform.position;
        arrowObj.transform.Translate(new Vector3(0, 0, -0.4f));
        arrowObj.transform.parent.SetAsLastSibling();

        TutorialArrow arrow = arrowObj.GetComponent<TutorialArrow>();
        if (arrow != null)
        {
            arrow.Rotate(angle);
            arrow.SetMessage(message);
        }

        return arrowObj;
    }

    // -----------------------------------------------------------
    // @Summary: Round one
    //            - Directional arrows
    //            - Popup warning when 2 fruit spawn
    //            - Popup warning when fruit nears spawn
    //            - Reload instructions
    // @Return: IEnumerator.
    // -----------------------------------------------------------

    IEnumerator RoundOne()
    {
        Debug.Log("RoundOne Tutorial");

        Player p = Player.GetInstance();
        BaseLight w = p.GetWeaponScript<BaseLight>(VRHand.ANY);
        w.Disable();

        GameObject arrows = Instantiate(mRoundArrowsPrefab);
        PopupController arrowsCtrl = arrows.AddComponent<PopupController>();
        arrowsCtrl.DestroyAfter(5);
        arrowsCtrl.DoOnDestroy(() =>
        {
            mSpawnManager.StartRound();
            w.Enable();
        });

        mCoroutineManager.DoWhen(() => {
            mGameState.PauseTime();
            GameObject popup = Instantiate(mPopup1, mUi.transform);
            PopupController ctrl = popup.AddComponent<PopupController>();
            ctrl.DoOnDestroy(() => mGameState.UnPauseTime());
        }, () => mSpawnManager.GetNumOfFruit() >= 2);

        mCoroutineManager.DoWhen(() => {
            mGameState.PauseTime();
            GameObject popup = Instantiate(mPopup2, mUi.transform);
            PopupController ctrl = popup.AddComponent<PopupController>();
            ctrl.DoOnDestroy(() => mGameState.UnPauseTime());
        }, () => {
            bool b = false;
            if (mSpawnManager.GetNumOfFruit() > 0)
            {
                var fruit = mSpawnManager.GetFruit().First();
                var path = fruit.GetComponent<FollowPath>();
                b = path && path.GetPositionAsProgress() > 75.0f;
            }
            return b;
        });

        mCoroutineManager.DoWhen(() => {
            mGameState.PauseTime();
            GameObject reload1Popup = Instantiate(mPopupReload1, mUi.transform);
            PopupController reload1PopupCtrl = reload1Popup.AddComponent<PopupController>();
            reload1PopupCtrl.DestroyWhen(() => w.IsReloading());
            reload1PopupCtrl.DoOnDestroy(() => {
                mGameState.UnPauseTimeForMs(400);
                GameObject reload2Popup = Instantiate(mPopupReload2, mUi.transform);
                PopupController reload2PopupCtrl = reload2Popup.AddComponent<PopupController>();
                reload2PopupCtrl.DestroyWhen(() => !w.IsReloading());
                reload2PopupCtrl.DoOnDestroy(() => mGameState.UnPauseTime());
            });
        }, () => w.GetAmmo() == 0);

        yield return null;
    }

    // -----------------------------------------------------------
    // @Summary: Round two
    //            - 
    //            - 
    //            - 
    //            - 
    // @Return: IEnumerator.
    // -----------------------------------------------------------
    IEnumerator RoundTwo()
    {
        Debug.Log("RoundOne Tutorial");

        Player p = Player.GetInstance();
        BaseLight w = p.GetWeaponScript<BaseLight>(VRHand.ANY);
        w.Disable();

        GameObject arrows = Instantiate(mRoundArrowsPrefab);
        PopupController arrowsCtrl = arrows.AddComponent<PopupController>();
        arrowsCtrl.DestroyAfter(5);
        arrowsCtrl.DoOnDestroy(() =>
        {
            mSpawnManager.StartRound();
            w.Enable();
        });

        yield return new WaitUntil(() => mSpawnManager.GetNumOfFruit() >= 2);
    }

    /*    IEnumerator RoundThree()
        {

        }

        IEnumerator RoundFour()
        {

        }*/

    // -----------------------------------------------------------
    // @Summary: Menu zero
    //            - Welcome message
    //            - Select hand
    //            - Navigate class menu
    //            - Buy handgun
    //            - Start round
    // @Return: IEnumerator.
    // -----------------------------------------------------------
    IEnumerator InBetweenZero()
    {
        GameObject arrow = null;

        bool halt = true;
        GameObject welcomeMsg = Instantiate(mWelcomeMessage, mUi.transform);
        BaseMenu welcomeMsgBaseMenu = welcomeMsg.GetComponent<BaseMenu>();
        welcomeMsgBaseMenu.DoOnPress("Continue", () => halt = false);
        yield return new WaitUntil(() => halt == false);

        halt = true;
        Destroy(welcomeMsg);
        GameObject roundMenu = Instantiate(mRoundMenu, mUi.transform);
        BaseMenu roundMenuBaseMenu = roundMenu.GetComponent<BaseMenu>();
        var btn = roundMenuBaseMenu.SpotlightButton("RightWeapon");
        arrow = CreateArrowPopup(btn, 0, "Let's upgrade the right hand...");
        roundMenuBaseMenu.DoOnPress("RightWeapon", () => halt = false);
        roundMenuBaseMenu.DoOnPress("LeftWeapon", () => halt = false);
        yield return new WaitUntil(() => halt == false);

        halt = true;
        Destroy(arrow);
        roundMenuBaseMenu.RestoreButtons();
        btn = roundMenuBaseMenu.SpotlightButton("NextPage");
        arrow = CreateArrowPopup(btn, 0, "Pick a primary weapon!");
        roundMenuBaseMenu.DoOnPress("NextPage", () => halt = false);
        yield return new WaitUntil(() => halt == false);

        halt = true;
        Destroy(arrow);
        roundMenuBaseMenu.RestoreButtons();
        btn = roundMenuBaseMenu.SpotlightButton("PrevPage");
        arrow = CreateArrowPopup(btn, 180, "Go back!");
        roundMenuBaseMenu.DoOnPress("PrevPage", () => halt = false);
        yield return new WaitUntil(() => halt == false);

        halt = true;
        Destroy(arrow);
        roundMenuBaseMenu.RestoreButtons();
        btn = roundMenuBaseMenu.SpotlightButton("Weapon");
        arrow = CreateArrowPopup(btn, 90, "Buy the light handgun");
        roundMenuBaseMenu.DoOnPress("Weapon", () => halt = false);
        yield return new WaitUntil(() => halt == false);

        halt = true;
        Destroy(arrow);
        roundMenuBaseMenu.RestoreButtons();
        btn = roundMenuBaseMenu.SpotlightButton("Back");
        arrow = CreateArrowPopup(btn, 0, "go back");
        roundMenuBaseMenu.DoOnPress("Back", () => halt = false);
        yield return new WaitUntil(() => halt == false);

        halt = true;
        Destroy(arrow);
        roundMenuBaseMenu.RestoreButtons();
        btn = roundMenuBaseMenu.SpotlightButton("Back");
        arrow = CreateArrowPopup(btn, 0, "go back");
        roundMenuBaseMenu.DoOnPress("Back", () => halt = false);
        yield return new WaitUntil(() => halt == false);

        halt = true;
        Destroy(arrow);
        roundMenuBaseMenu.RestoreButtons();
        btn = roundMenuBaseMenu.SpotlightButton("NextRound");
        arrow = CreateArrowPopup(btn, 0, "Start the round");
        roundMenuBaseMenu.DoOnPress("NextRound", () => halt = false);
        yield return new WaitUntil(() => halt == false);
    }

    // -----------------------------------------------------------
    // @Summary: Menu one
    //            - Navigate upgrade menu
    //            - Buy handgun upgrade
    //            - Start round
    // @Return: IEnumerator.
    // -----------------------------------------------------------
    IEnumerator InBetweenOne()
    {
        Player.GetInstance().SetCash(550);
        bool halt = true;
        GameObject roundMenu = Instantiate(mRoundMenu, mUi.transform);
        BaseMenu roundMenuBaseMenu = roundMenu.GetComponent<BaseMenu>();
        var btn = roundMenuBaseMenu.SpotlightButton("Upgrades");
        GameObject arrow = CreateArrowPopup(btn, 0, "Let's upgrade the right weapon...");
        roundMenuBaseMenu.DoOnPress("Upgrades", () => halt = false);
        yield return new WaitUntil(() => halt == false);

        halt = true;
        Destroy(arrow);
        roundMenuBaseMenu.RestoreButtons();
        btn = roundMenuBaseMenu.SpotlightButton("RightWeapon");
        arrow = CreateArrowPopup(btn, 0, "select right hand...");
        roundMenuBaseMenu.DoOnPress("RightWeapon", () => halt = false);
        yield return new WaitUntil(() => halt == false);

        halt = true;
        Destroy(arrow);
        roundMenuBaseMenu.RestoreButtons();
        btn = roundMenuBaseMenu.SpotlightButton("UpgradeUp");
        arrow = CreateArrowPopup(btn, 0, "Purchase the top-path upgrade!");
        roundMenuBaseMenu.DoOnPress("UpgradeUp", () => halt = false);
        yield return new WaitUntil(() => halt == false);

        halt = true;
        Destroy(arrow);
        roundMenuBaseMenu.RestoreButtons();
        btn = roundMenuBaseMenu.SpotlightButton("Back");
        arrow = CreateArrowPopup(btn, 0, "Go back...");
        roundMenuBaseMenu.DoOnPress("Back", () => halt = false);
        yield return new WaitUntil(() => halt == false);

        halt = true;
        Destroy(arrow);
        roundMenuBaseMenu.RestoreButtons();
        btn = roundMenuBaseMenu.SpotlightButton("Back");
        arrow = CreateArrowPopup(btn, 0, "Go back...");
        roundMenuBaseMenu.DoOnPress("Back", () => halt = false);
        yield return new WaitUntil(() => halt == false);


        halt = true;
        Destroy(arrow);
        roundMenuBaseMenu.RestoreButtons();
        btn = roundMenuBaseMenu.SpotlightButton("NextRound");
        arrow = CreateArrowPopup(btn, 0, "Start the round");
        roundMenuBaseMenu.DoOnPress("NextRound", () => halt = false);
        yield return new WaitUntil(() => halt == false);
    }

    /*
        IEnumerator InBetweenThree()
        {

        }

        IEnumerator InBetweenFour()
        {

        }*/


    // -----------------------------------------------------------
    // Override the Awake() method.
    // -----------------------------------------------------------
    private void Awake()
    {
        // Get a reference to the SpawnManager script
        mSpawnManager = GetComponentInChildren<SpawnManager>();
        mRoundFPtrMap[1U] = RoundOne;
        mRoundFPtrMap[2U] = RoundTwo;
        mInBtwnFPtrMap[0U] = InBetweenZero;
        mInBtwnFPtrMap[1U] = InBetweenOne;
/*        mRoundFPtrMap[3U] = RoundThree();
        mRoundFPtrMap[4U] = RoundFour();*/

    }

    // -----------------------------------------------------------
    // Override the Start() method.
    // -----------------------------------------------------------
    void Start()
    {
        if (GameState.GetDifficulty() == Difficulty.TUTORIAL)
        {
            mGameManager.enabled = false;
            UIManager ui = mUi.GetComponentInChildren<UIManager>();
            ui.enabled = false;
            Destroy(ui);
        }
        else
        {
            enabled = false;
            return;
        }

        mCoroutineManager = new CoroutineManager();
        mGameState = GameState.GetInstance();
        mGameState.SetState( State.ROUND_END );
        Time.timeScale = 1f;
    }

    // -----------------------------------------------------------
    // Override the Update() function.
    // -----------------------------------------------------------
    private void Update()
    {

    }

    // -----------------------------------------------------------
    // Override the OnEnable() function.
    // -----------------------------------------------------------
    private void OnEnable()
    {
        Events.Instance.AddListener<EventStateChange>( HandleStateChange );
    }

    // -----------------------------------------------------------
    // Override the OnDisable() function.
    // -----------------------------------------------------------
    private void OnDisable()
    {
        Events.Instance.RemoveListener<EventStateChange>( HandleStateChange );
    }
}