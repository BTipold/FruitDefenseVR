// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using CommonTypes;
using UnityEngine;


public class ManageGame : MonoBehaviour
{
    // -----------------------------------------------------------
    // Member Variables
    // -----------------------------------------------------------
    private GameState mGameState = null;
    private SpawnManager mSpawnManager = null;

    // -----------------------------------------------------------
    // @Summary: Handler function for state change event. Takes
    //   action for each new event.
    // @Param: e - Event data which contains the new state
    //   and previous state.
    // @Return: void.
    // -----------------------------------------------------------
    void HandleStateChange(EventStateChange e)
    {
        // Create instance of saveload to interface with the save system
        SaveLoadInterface saveInterface = new SaveLoadInterface();

        switch (e.newState)
        {
            case State.PAUSE:
                Debug.Log( "Game Paused" );
                break;

            case State.ROUND_IN_PROGRESS:
                mSpawnManager.StartRound();
                Debug.Log( "Round Started" );
                break;

            case State.ROUND_END:
                if( mGameState.GetRoundNumber() >= mGameState.GetMaxRound() )
                {
                    mGameState.SetState( State.GAME_WON );
                }

                Debug.Log( "In-between Rounds" );
                break;

            case State.GAME_WON:
                saveInterface.RemoveCurrentSave();
                Debug.Log( "Game won." );
                break;

            case State.GAME_LOST:
                saveInterface.RemoveCurrentSave();
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

    // -----------------------------------------------------------
    // Override the Awake() method.
    // -----------------------------------------------------------
    private void Awake()
    {
        // Get a reference to the SpawnManager script
        mSpawnManager = GetComponentInChildren<SpawnManager>();
    }

    // -----------------------------------------------------------
    // Override the Start() method.
    // -----------------------------------------------------------
    void Start()
    {
        mGameState = GameState.GetInstance();
        mGameState.SetState( State.ROUND_END );
        Time.timeScale = 1f;

        SaveLoadInterface saveInterface = new SaveLoadInterface();
        saveInterface.Load();
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