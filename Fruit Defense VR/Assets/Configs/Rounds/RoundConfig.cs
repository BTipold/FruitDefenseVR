// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using System.Collections.Generic;
using UnityEngine;
using CommonTypes;
using System.Linq;


[CreateAssetMenu(menuName = "RoundConfig")]
public class RoundConfig : ScriptableObject
{
    // -----------------------------------------------------------
    // Member Variables
    // -----------------------------------------------------------
    public GameObject mPfRedApple;
    public GameObject mPfGreenApple;
    public GameObject mPfLime;
    public GameObject mPfLemon;
    public GameObject mPfPeach;
    public GameObject mPfPear;
    public GameObject mPfMango;
    public GameObject mPfOrange;
    public GameObject mPfWatermelon;
    public GameObject mPfBasket;
    public GameObject mPfCart;
    public GameObject mPfGhostRedApple;
    public GameObject mPfGhostGreenApple;
    public GameObject mPfGhostLime;
    public GameObject mPfGhostLemon;
    public GameObject mPfGhostPeach;
    public GameObject mPfGhostPear;
    public GameObject mPfGhostMango;
    public GameObject mPfGhostOrange;
    public GameObject mPfGhostWatermelon;
    public TextAsset mRawCsvFile;
    public CsvReader mCsv;

    private List<Round> mRounds = new List<Round>();
    private Dictionary<int, string> mHints = new Dictionary<int, string>();

    public void Init()
    {
        mCsv = new CsvReader( mRawCsvFile );
        mRounds.Clear();
        mHints.Clear();
        BuildRounds();
    }
    
    private void BuildRounds()
    {
        // Create the first round and add it to the list
        AddNewRound( 1 );

        // Start row at 1 because row 0 contains the headers.
        for ( int row = 1; row < mCsv.NumRows(); row++ )
        {
            // Get the round number for current row
            int roundNum = int.Parse(mCsv.GetDataUnderHeading( "Round", row ));

            // If it's a new round, we need to add a new round to list.
            if ( mRounds.Last().number != roundNum )
            {
                AddNewRound( roundNum );
                string hint = mCsv.GetDataUnderHeading( "Hint", row );
                mHints.Add( roundNum, hint );
            }

            // Now extract the wave properties and add to round.
            Wave wave = CreateWave(row);
            mRounds.Last().waves.Add( wave );
        }
    }

    private void AddNewRound( int number )
    {
        Round round;
        round.number = number;
        round.waves = new List<Wave>();
        mRounds.Add( round );
    }

    private Wave CreateWave( int rowNum )
    {
        Wave wave;
        wave.queue      = new Queue<GameObject>();
        wave.time       = float.Parse( mCsv.GetDataUnderHeading("Time", rowNum) );
        wave.spacing    = float.Parse( mCsv.GetDataUnderHeading("Spacing", rowNum) );

        int redApples   = int.Parse(mCsv.GetDataUnderHeading("Red Apple", rowNum));
        int greenApples = int.Parse(mCsv.GetDataUnderHeading("Green Apple", rowNum));
        int limes       = int.Parse(mCsv.GetDataUnderHeading("Lime", rowNum));
        int lemons      = int.Parse(mCsv.GetDataUnderHeading("Lemon", rowNum));
        int peaches     = int.Parse(mCsv.GetDataUnderHeading("Peach", rowNum));
        int pears       = int.Parse(mCsv.GetDataUnderHeading("Pear", rowNum));
        int mangos      = int.Parse(mCsv.GetDataUnderHeading("Mango", rowNum));
        int oranges     = int.Parse(mCsv.GetDataUnderHeading("Orange", rowNum));
        int watermelon  = int.Parse(mCsv.GetDataUnderHeading("Watermelon", rowNum));
        int baskets     = int.Parse(mCsv.GetDataUnderHeading("Basket", rowNum));
        int carts       = int.Parse(mCsv.GetDataUnderHeading("Cart", rowNum));

        int ghostRedApples   = int.Parse(mCsv.GetDataUnderHeading("Ghost Red Apple", rowNum));
        int ghostGreenApples = int.Parse(mCsv.GetDataUnderHeading("Ghost Green Apple", rowNum));
        int ghostLimes       = int.Parse(mCsv.GetDataUnderHeading("Ghost Lime", rowNum));
        int ghostLemons      = int.Parse(mCsv.GetDataUnderHeading("Ghost Lemon", rowNum));
        int ghostPeaches     = int.Parse(mCsv.GetDataUnderHeading("Ghost Peach", rowNum));
        int ghostPears       = int.Parse(mCsv.GetDataUnderHeading("Ghost Pear", rowNum));
        int ghostMangos      = int.Parse(mCsv.GetDataUnderHeading("Ghost Mango", rowNum));
        int ghostOranges     = int.Parse(mCsv.GetDataUnderHeading("Ghost Orange", rowNum));
        int ghostWatermelon  = int.Parse(mCsv.GetDataUnderHeading("Ghost Watermelon", rowNum));

        AddFruitToWaveEvenly( ref wave.queue, mPfRedApple, redApples );
        AddFruitToWaveEvenly( ref wave.queue, mPfGreenApple, greenApples );
        AddFruitToWaveEvenly( ref wave.queue, mPfLime, limes );
        AddFruitToWaveEvenly( ref wave.queue, mPfLemon, lemons );
        AddFruitToWaveEvenly( ref wave.queue, mPfPeach, peaches );
        AddFruitToWaveEvenly( ref wave.queue, mPfPear, pears );
        AddFruitToWaveEvenly( ref wave.queue, mPfMango, mangos );
        AddFruitToWaveEvenly( ref wave.queue, mPfOrange, oranges );
        AddFruitToWaveEvenly( ref wave.queue, mPfWatermelon, watermelon );
        AddFruitToWaveEvenly( ref wave.queue, mPfBasket, baskets );
        AddFruitToWaveEvenly( ref wave.queue, mPfCart, carts );
        AddFruitToWaveEvenly( ref wave.queue, mPfGhostRedApple, ghostRedApples );
        AddFruitToWaveEvenly( ref wave.queue, mPfGhostGreenApple, ghostGreenApples );
        AddFruitToWaveEvenly( ref wave.queue, mPfGhostLime, ghostLimes );
        AddFruitToWaveEvenly( ref wave.queue, mPfGhostLemon, ghostLemons );
        AddFruitToWaveEvenly( ref wave.queue, mPfGhostPeach, ghostPeaches );
        AddFruitToWaveEvenly( ref wave.queue, mPfGhostPear, ghostPears );
        AddFruitToWaveEvenly( ref wave.queue, mPfGhostMango, ghostMangos );
        AddFruitToWaveEvenly( ref wave.queue, mPfGhostOrange, ghostOranges );
        AddFruitToWaveEvenly( ref wave.queue, mPfGhostWatermelon, ghostWatermelon );

        return wave;
    }

    private void AddFruitToWaveEvenly( ref Queue<GameObject> queue, GameObject fruitPf, int number )
    {
        if ( number > 0 )
        {
            Queue<GameObject> fruitToAdd = MakeListOfObjects(fruitPf, number);
            queue = Utils.CombineQueues( fruitToAdd, queue );
        }
    }

    private Queue<GameObject> MakeListOfObjects( GameObject obj, int count )
    {
        Queue<GameObject> list = new Queue<GameObject>();

        for (int i = 0; i < count; i++ )
        {
            list.Enqueue( obj );
        }

        return list;
    }

    public List<Round> GetRounds()
    {
        return mRounds;
    }

    public string GetStaticHint( uint round )
    {
        string hint = "";
        if( mHints.ContainsKey((int)round) )
        {
            hint = mHints[(int)round];
        }
        return hint;
    }

    public uint GetNumRounds()
    {
        return (uint)mRounds.Count;
    }

    private void Awake()
    {

    }
}
