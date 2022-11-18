// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using UnityEngine;
using CommonTypes;
using TMPro;


public class HintGenerator : MonoBehaviour
{
    // -----------------------------------------------------------
    // Attributes
    // -----------------------------------------------------------
    public TextMeshProUGUI mHintGUI = null;

    private bool mAbilityHintPlayed = false;
    private bool mMagicHintPlayed = false;
    private bool mOtherHintPlayed = false;
    private bool mOther1HintPlayed = false;
    private Player mPlayer = null;
    private RoundConfig mRoundConfig = null;
    private GameState mGameState = null;

    // -----------------------------------------------------------
    // @Summary: generates a hint from the config or a dynamic 
    //   hint if the criteria is met.
    // @Param: roundNumber - current round number.
    // @Return: string - returns hint as a string. 
    // -----------------------------------------------------------
    public string GetNextHint( uint roundNum )
    {
        string hint = GetDynamicHint();

        if (hint == null)
        {
            hint = mRoundConfig.GetStaticHint( roundNum );
        }

        return hint;
    }

    // -----------------------------------------------------------
    // @Summary: Checks if there are any special cases that need
    //   special hints to be generated.
    // @Return: string - hint text. 
    // -----------------------------------------------------------
    private string GetDynamicHint()
    {
        string hint = null;

        // Check for hint overrides
        if ( !mAbilityHintPlayed )
        {
            if ( mPlayer.HasWeaponInHand(VRHand.RIGHT) )
            {
                if ( mPlayer.GetWeaponScript<BaseWeapon>( VRHand.RIGHT).mAbilities > 0 )
                {
                    hint = "Your right hand weapon has an ability! Activate ability by pressing B";
                    mAbilityHintPlayed = true;
                }
            }
            else if ( mPlayer.HasWeaponInHand(VRHand.LEFT) )
            {
                if ( mPlayer.GetWeaponScript<BaseWeapon>( VRHand.LEFT).mAbilities > 0 )
                {
                    hint = "Your left hand weapon has an ability! Activate ability by pressing Y";
                    mAbilityHintPlayed = true;
                }
            }
        }

        if ( !mMagicHintPlayed )
        {
        }

        if ( !mOtherHintPlayed )
        {
        }

        if ( !mOther1HintPlayed )
        {
        }

        return hint;
    }

    // -----------------------------------------------------------
    // Override the Awake() function. 
    // -----------------------------------------------------------
    private void Awake()
    {
        mPlayer = Player.GetInstance();
        mGameState = GameState.GetInstance();
    }

    // -----------------------------------------------------------
    // Override the Start() function.
    // -----------------------------------------------------------
    private void Start()
    {
    }

    // -----------------------------------------------------------
    // Override the OnEnable() function.
    // -----------------------------------------------------------
    private void OnEnable()
    {
        mRoundConfig = mGameState.GetRoundConfig( );
        uint roundNumber = mGameState.GetRoundNumber();
        mHintGUI.text = GetNextHint( roundNumber );
    }
}
