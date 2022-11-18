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
    // Private attributes
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
    public TextAsset mRawCsvFile;
    public CsvReader mCsv;

    private List<Round> mRounds = new List<Round>();

    public void Init()
    {
        mCsv = new CsvReader( mRawCsvFile );
        mRounds.Clear();
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
        wave.time       = float.Parse( mCsv.GetDataUnderHeading( "Time", rowNum ) ); ;
        wave.spacing    = float.Parse( mCsv.GetDataUnderHeading( "Spacing", rowNum ) ); ;

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

    public uint GetNumRounds()
    {
        return (uint)mRounds.Count;
    }

    private void Awake()
    {

    }
}
