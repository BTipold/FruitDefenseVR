// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using CommonTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SpawnManager : MonoBehaviour
{
    // -----------------------------------------------------------
    // Member Variables
    // -----------------------------------------------------------
    private RoundConfig mRoundConfig;
    private List<Round> mRounds;
    private uint mNumFruitToSpawn = 0;
    private bool mRoundInProgress = false;
    private bool mSpawnLock = false;
    private bool mPendingStart = false;
    private GameState mGameState = null;
    private Queue<GameObject> mSpawnBuffer = new Queue<GameObject>();
    private List<GameObject> mFruit = new List<GameObject>();
    private Dictionary<GameObject, float> mFruitSizes = new Dictionary<GameObject, float>();
    private Player mPlayer = null;

    // -----------------------------------------------------------
    // @Summary: Starts co-routine to begin spawning.
    // @Return: void
    // -----------------------------------------------------------
    public void StartRound()
    {
        if ( !mRoundInProgress )
        {
            uint roundNumber = mGameState.GetRoundNumber();
            mNumFruitToSpawn = CountFruitInRound( roundNumber );
            mRoundInProgress = true;
            mPendingStart = false;

            // Start spawning fruit
            StartCoroutine( BeginSpawning( roundNumber ) );
        }
    }

    // -----------------------------------------------------------
    // @Summary: Counts number of total fruit that will need to 
    //   to be spawned in the current round.
    // @Param: roundNumber - current round number.
    // @Return: uint - number of fruit to spawn. 
    // -----------------------------------------------------------
    private uint CountFruitInRound(uint roundNumber)
    {
        uint numOfFruit = 0;

        Round round = mRounds[(int)roundNumber - 1];
        foreach ( Wave wave in round.waves )
        {
            numOfFruit += wave.NumFruit();
        }

        Debug.Log( "Spawning " + numOfFruit + " fruit." );
        return numOfFruit;
    }

    // -----------------------------------------------------------
    // @Summary: Co-routine which starts sub co-routines. The sub
    //   routines are responsible for spawning each wave in the 
    //   round. 
    // @Param: roundNumber - current round number.
    // @Return: IEnumerator
    // -----------------------------------------------------------
    IEnumerator BeginSpawning( uint roundNumber )
    {
        Round round = mRounds[(int)roundNumber - 1];
        foreach ( Wave wave in round.waves )
        {
            StartCoroutine( SpawnWave( wave, wave.time ) );
        }
        
        yield return null;
    }

    // -----------------------------------------------------------
    // @Summary: Co-routine responsible for spawning each fruit
    //   in the wave. 
    // @Param: wave - reference to the "wave" object. Wave 
    //   contains which fruit to spawn, number of fruit to 
    //   spawn, spacing, and delay. 
    // @Param: delay - starts spawning after the delay. 
    // @Return: IEnumerator
    // -----------------------------------------------------------
    IEnumerator SpawnWave( Wave wave, float delay = 0f )
    {
        // Delay start of the wave according to round design.
        yield return new WaitForSeconds( delay );

        while ( wave.NumFruit() > 0 )
        {
            // Delay spawning until start time
            yield return new WaitForSeconds( wave.spacing );
        
            // Sit here waiting until the SpawnLock has lifted
            while ( mSpawnLock )
            {
                yield return null;
            }
        
            // Lock spawning so no other co-routine can spawn
            mSpawnLock = true;

            // Get next fruit from the queue.
            GameObject nextSpawn = wave.queue.Dequeue();
            if ( !SpawnFruit(nextSpawn) )
            {
                // If the spawn area is blocked, add it to the spawn queue
                mSpawnBuffer.Enqueue( nextSpawn );
            }
        
            // Spawning done, lift the lock condition
            mSpawnLock = false;
        }
        
        yield return null;
    }

    // -----------------------------------------------------------
    // @Summary: Function spawns a fruit if there is no blockage 
    //   at the spawn point. If there is an obstruction, the 
    //   function returns false. 
    // @Param: level - fruit level
    // @Return: returns true if enemy spawned, false if blockage.
    // -----------------------------------------------------------
    bool SpawnFruit( GameObject fruitPf )
    {
        bool obstruction = CheckForSpawnOverlap(fruitPf);

        if ( !obstruction )
        {
            var fruit = Instantiate( fruitPf, gameObject.transform );
            var baseFruit = fruit.GetComponent<BaseFruit>();
            baseFruit.DoOnDestroy(FruitDestroyCallback);
            mFruit.Add(fruit);
            
            mNumFruitToSpawn--;
            Debug.Log("Fruit to spawn " + (mNumFruitToSpawn + 1) + " => " + mNumFruitToSpawn);
        }

        // We want to return TRUE if obstruction is FALSE.
        return !obstruction;
    }

    public List<GameObject> GetFruit()
    {
        return mFruit;
    }

    public int GetNumOfFruit()
    {
        return mFruit.Count;
    }

    // -----------------------------------------------------------
    // @Summary: will return true if there are overlapping 
    //   fruit at the spawn point. Will return false if there 
    //   are no fruit on the spawn point.
    // @Param: fruitLevel - needed to retreive that fruit's 
    //   collider. 
    // @Return: void
    // -----------------------------------------------------------
    private bool CheckForSpawnOverlap( GameObject fruitPf )
    {
        Vector3 spawnPoint = FruitPath.GetInstance().GetSpawnPoint();
        float colliderRadius = GetColliderSize(fruitPf);

        Collider[] colliders = Physics.OverlapSphere(spawnPoint, colliderRadius, ~Utils.IgnoreBitmask("Fruit"));
        return ( colliders.Length != 0 );
    }

    // -----------------------------------------------------------
    // @Summary: Checks if there is anything in the buffer. If 
    //   there is, then it will spawn it if possible. Fruit's in
    //   the buffer will be spawned immidiately when clear.
    // @Return: void
    // -----------------------------------------------------------
    private void ProcessBuffer()
    {
        if (mSpawnBuffer.Count > 0)
        {
            if( SpawnFruit(mSpawnBuffer.Peek()) )
            {
                mSpawnBuffer.Dequeue();
            }
        }
    }

    // -----------------------------------------------------------
    // @Summary: Checks the bounding box size of a prefab before 
    //   it is cloned.
    // @Param: prefab - the prefab to check the size of.
    // @Return: float - max size of prefab as a float.
    // -----------------------------------------------------------
    private float GetColliderSize(GameObject prefab)
    {
        float size = 0.0f;

        if ( mFruitSizes.ContainsKey(prefab) )
        {
            size = mFruitSizes[prefab];
        }
        else
        {
            mFruitSizes[prefab] = GetColliderSizeOfPrefab( prefab );
        }

        return size;
    }

    // -----------------------------------------------------------
    // @Summary: Measures the max size of a prefab with 
    //   a collider object. 
    // @Param:  obj - gameObject with collider.
    // @Return: float - max size of collider on obj. If the 
    //   object does not have a collider, it returns -1. 
    // -----------------------------------------------------------
    private float GetColliderSizeOfPrefab( GameObject prefab )
    {
        float colliderSize = -1;

        // Unity doesn't let you check the size of a prefab's 
        // collider so we need to instantiate it first. 
        GameObject objInst = Instantiate(prefab);
        Collider collider = objInst.GetComponent<Collider>();

        if ( collider != null )
        {
            colliderSize = collider.bounds.max.magnitude;
        }

        Destroy( objInst );

        return colliderSize;
    }

    private void HandleStateChange(EventStateChange e)
    {
        mPlayer = Player.GetInstance();
        switch (e.newState)
        {
            case State.ROUND_IN_PROGRESS:
                if (e.prevState != State.PAUSE)
                {
                    if (!mRoundInProgress)
                    {
                        mPendingStart = true;
                    }
                }
                break;

            case State.PAUSE:
            case State.GAME_LOST:
            case State.GAME_WON:
            default:
                break;
        }
    }

    private void FruitDestroyCallback(GameObject fruit)
    {
        mFruit.RemoveAll(f => f.gameObject == fruit);
    }

    // -----------------------------------------------------------
    // Override the Start() method. 
    // -----------------------------------------------------------
    void Start()
    {
        mGameState = GameState.GetInstance();
        mPlayer = Player.GetInstance();
        gameObject.transform.position = FruitPath.GetInstance().GetSpawnPoint();
        mRoundConfig = mGameState.GetRoundConfig();
        mRounds = mRoundConfig.GetRounds();
    }

    // -----------------------------------------------------------
    // Override the Update() method. 
    // -----------------------------------------------------------
    private void Update()
    {
        // Check if there is any fruit in the buffer to spawn.
        ProcessBuffer();

        // Check if fruit are done spawning. 
        if ( mNumFruitToSpawn <= 0 && !mPendingStart 
            && mGameState.GetState() == State.ROUND_IN_PROGRESS)
        {
            // Check if there is nothing in the fruit container.
            if ( gameObject.transform.childCount <= 0 )
            {
                mRoundInProgress = false;

                // Create instance of saveload to interface with the save system
                SaveLoadInterface saveInterface = new SaveLoadInterface();
                saveInterface.Save();

                // Add end of round cash
                mPlayer.AddCash(100);
                
                uint cashEnhancementLevel = mPlayer.GetCurrentEnhancmentLevel(VRHand.ANY, LightEnhancements.CASH);
                if (cashEnhancementLevel >= 1)
                {
                    mPlayer.AddCash(250);
                }
                if (cashEnhancementLevel >= 2)
                {
                    mPlayer.AddCash(500);
                }

                // Set game state
                mGameState.SetState(State.ROUND_END);
            }
        }
    }

    // -----------------------------------------------------------
    // Override the OnEnable() function.
    // -----------------------------------------------------------
    private void OnEnable()
    {
        Events.Instance.AddListener<EventStateChange>(HandleStateChange);
    }

    // -----------------------------------------------------------
    // Override the OnDisable() function.
    // -----------------------------------------------------------
    private void OnDisable()
    {
        Events.Instance.RemoveListener<EventStateChange>(HandleStateChange);
    }
}
