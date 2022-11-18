// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using CommonTypes;
using UnityEngine;
using System.Collections;

public sealed class GameState : MonoBehaviour
{
    // -----------------------------------------------------------
    // Member Variables
    // -----------------------------------------------------------
    public RoundConfig mEasy;
    public RoundConfig mMedium;
    public RoundConfig mHard;
    public RoundConfig mTutorial;

    private PersistentDataType<uint> mRoundNumber = new PersistentDataType<uint>(0, "round");
    private PersistentDataType<uint> mHealth = new PersistentDataType<uint>(100, "health");

    private uint mMaxRound = 50;
    private State mState = State.UNKNOWN;
    private State mStateBeforePause = State.UNKNOWN;
    private RoundConfig mRoundConfig;

    private static Difficulty mDifficulty = Difficulty.EASY;
    private static Map mMap = Map.UNKNOWN;
    private static GameState mInstance = null;

    // -----------------------------------------------------------
    // @Summary: getter for singleton instance.
    // @Return: returns reference to the ONLY instance of this 
    //   class. 
    // -----------------------------------------------------------
    public static GameState GetInstance()
    {
        return mInstance;
    }

    // -----------------------------------------------------------
    // @Summary: getter for state. Tells you current game state. 
    // @Return: returns State enum.
    // -----------------------------------------------------------
    public State GetState()
    {
        return mState;
    }

    // -----------------------------------------------------------
    // @Summary: getter for round config. 
    // @Return: returns the round config.
    // -----------------------------------------------------------
    public RoundConfig GetRoundConfig()
    {
        return mRoundConfig;
    }

    // -----------------------------------------------------------
    // @Summary: getter for current round number. 
    // @Return: uint - returns current round number.
    // -----------------------------------------------------------
    public uint GetRoundNumber()
    {
        return mRoundNumber;
    }

    // -----------------------------------------------------------
    // @Summary: getter for max round. 
    // @Return: uint - returns max round.
    // -----------------------------------------------------------
    public uint GetMaxRound()
    {
        return mMaxRound;
    }

    // -----------------------------------------------------------
    // @Summary: getter for current health. 
    // @Return: uint - returns health.
    // -----------------------------------------------------------
    public uint GetHealth()
    {
        return mHealth;
    }

    // -----------------------------------------------------------
    // @Summary: Setter for game state. This function also raises
    //   a new event indicating that a state change was made. 
    // @Param: newState - new state to switch to. 
    // @Return: void
    // -----------------------------------------------------------
    public void SetState( State newState )
    {
        if ( mState != newState )
        {
            // Raise an event signalling the game state has changed.
            EventStateChange e = new EventStateChange( newState, mState );
            Events.Instance.Raise<EventStateChange>( e );

            mState = newState;
        }
    }

    // -----------------------------------------------------------
    // @Summary: Pauses the game. Saves state the game was in 
    //   before pausing the game so we can go back to this state
    //   after. 
    // @Return: void
    // -----------------------------------------------------------
    public void Pause()
    {
        if ( mState != State.PAUSE )
        {
            Time.timeScale = 0f;
            mStateBeforePause = mState;
            SetState( State.PAUSE );
        }
    }

    // -----------------------------------------------------------
    // @Summary: Resumes the game. Restores state to the state 
    //   the game was in before pause. 
    // @Return: void
    // -----------------------------------------------------------
    public void UnPause()
    {
        if ( mState == State.PAUSE )
        {
            Time.timeScale = 1f;
            SetState( mStateBeforePause );
        }
    }

    // -----------------------------------------------------------
    // @Summary: Setter for current round number.
    // @Param: roundNumber - new round number to be set. 
    // @Return: void
    // -----------------------------------------------------------
    public void SetRound( uint roundNumber )
    {
        mRoundNumber.Assign(roundNumber);
    }

    // -----------------------------------------------------------
    // @Summary: Setter for max round.
    // @Param: roundNumber - new max round to be set. 
    // @Return: void
    // -----------------------------------------------------------
    public void SetMaxRound( uint roundNumber )
    {
        mMaxRound = roundNumber;
    }

    // -----------------------------------------------------------
    // @Summary: Setter for health.
    // @Param: health - new health value to set. 
    // @Return: void
    // -----------------------------------------------------------
    public void SetHealth( uint health )
    {
        mHealth.Assign(health);
    }

    // -----------------------------------------------------------
    // @Summary: decreases health by specified amount.
    // @Param: health - value to decrement by.
    // @Return: void
    // -----------------------------------------------------------
    public void DecrementHealth( uint health )
    {
        int newHealth = (int)(uint)mHealth - (int)health;

        if ( newHealth <= 0 )
        {
            mHealth.Assign(0);
            SetState( State.GAME_LOST );
        }
        else
        {
            mHealth.Assign((uint)newHealth);
        }
        Debug.Log( "Health: " + mHealth );
    }

    // -----------------------------------------------------------
    // @Summary: increases health by specified amount.
    // @Param: health - value to increase by.
    // @Return: void
    // -----------------------------------------------------------
    public void IncreaseHealth( uint health )
    {
        mHealth.Assign( mHealth + health);
    }

    // -----------------------------------------------------------
    // @Summary: increases the round number by one.
    // @Return: void
    // -----------------------------------------------------------
    public void NextRound()
    {
        mRoundNumber.Assign(mRoundNumber + 1);
        SetState(State.ROUND_IN_PROGRESS);
        Debug.Log( "New Round: " + mRoundNumber );
    }

    public static Difficulty GetDifficulty()
    {
        return mDifficulty;
    }

    public static void SetDifficulty(Difficulty difficulty)
    {
        mDifficulty = difficulty;
    }

    public static Map GetMap()
    {
        return mMap;
    }

    public static void SetMap(Map map)
    {
        mMap = map;
    }

    // -----------------------------------------------------------
    // @Summary: Selects the correct round config for the 
    //   difficulty. 
    // @Return: void
    // -----------------------------------------------------------
    private void SelectRoundConfig()
    {
        Debug.Log( "Difficulty=[" + mDifficulty.ToString() + "]" );
        switch (mDifficulty)
        {
            case Difficulty.EASY:
                mRoundConfig = mEasy;
                break;

            case Difficulty.MEDIUM:
                mRoundConfig = mMedium;
                break;

            case Difficulty.HARD:
                mRoundConfig = mHard;
                break;

            case Difficulty.TUTORIAL:
                mRoundConfig = mTutorial;
                break;

            default:
                mRoundConfig = mEasy;
                break;
        }
    }

    // -----------------------------------------------------------
    // @Summary: increases health by specified amount.
    // @Param: health - value to increase by.
    // @Return: void
    // -----------------------------------------------------------
    public void PauseTime()
    {
        Time.timeScale = 0;
    }

    // -----------------------------------------------------------
    // @Summary: increases health by specified amount.
    // @Param: health - value to increase by.
    // @Return: void
    // -----------------------------------------------------------
    public void UnPauseTime()
    {
        Time.timeScale = 1;
    }

    // -----------------------------------------------------------
    // @Summary: increases health by specified amount.
    // @Param: health - value to increase by.
    // @Return: void
    // -----------------------------------------------------------
    public void PauseTimeForMs(uint milliseconds)
    {
        PauseTime();
        IEnumerator UnPauseAfterDelay()
        {
            yield return new WaitForSeconds(milliseconds / 1000f);
            UnPauseTime();
        }
        StartCoroutine(UnPauseAfterDelay());
    }

    // -----------------------------------------------------------
    // @Summary: increases health by specified amount.
    // @Param: health - value to increase by.
    // @Return: void
    // -----------------------------------------------------------
    public void UnPauseTimeForMs(uint milliseconds)
    {
        UnPauseTime();
        IEnumerator PauseAfterDelay()
        {
            yield return new WaitForSeconds(milliseconds / 1000f);
            PauseTime();
        }
        StartCoroutine(PauseAfterDelay());
    }

    // -----------------------------------------------------------
    // Override the Awake() method.
    // -----------------------------------------------------------
    void Awake()
    {
        if ( mInstance == null )
        {
            mInstance = this;
        }

        SelectRoundConfig();
        mRoundConfig.Init();
        mMaxRound = mRoundConfig.GetNumRounds();

        // Create instance of saveload to interface with the save system
        SaveLoadInterface saveInterface = new SaveLoadInterface();

        saveInterface.RegisterSaveableObject(mHealth);
        saveInterface.RegisterSaveableObject(mRoundNumber);
    }

    // -----------------------------------------------------------
    // Override the Start() method.
    // -----------------------------------------------------------
    void Start()
    {
        if ( mDifficulty == Difficulty.TUTORIAL ) {
            gameObject.GetComponent<ManageTutorial>().enabled = true;
        } else {
            gameObject.GetComponent<ManageTutorial>().enabled = false;
        }
    }
}
