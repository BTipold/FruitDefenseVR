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
using CommonTypes;
using UnityEngine.UI;

public class UpgradeMenu : MonoBehaviour
{
    private static bool mUpgradeOrEnhancement = true;
    public  GameObject mClassMenu;
    public  GameObject mUpgradeHand;
    public  GameObject mEnhancementMenu;
    public  GameObject mLeftRight;
    public  GameObject mFullTreeView;

    public string mCurrentMenu = "Upgrade";
    
    private Player mPlayer = null;

    public void Right()
    {
        mUpgradeHand.GetComponent<UpgradeHand>().setHand(VRHand.RIGHT);
        mEnhancementMenu.GetComponent<EnhancementsMenu>().setHand(VRHand.RIGHT);
        mClassMenu.GetComponent<ClassMenu>().setHand(VRHand.RIGHT);
        mFullTreeView.GetComponent<FullTreeView>().setHand(VRHand.RIGHT);

        if (!mPlayer.GetWeaponObj(VRHand.RIGHT))
        {
            Debug.Log("Taking you to class selection...");
            mClassMenu.SetActive(true);
            mClassMenu.GetComponent<TransitionAnimation>().ScaleIn();
        }
        else if (mUpgradeOrEnhancement == true)
        {
            Debug.Log("Taking you to the Upgrade Menu...");
            mUpgradeHand.SetActive(true);
            mUpgradeHand.GetComponent<TransitionAnimation>().ScaleIn();
        }
        else
        {
            Debug.Log("Taking you to the Enhancement Menu...");
            mEnhancementMenu.SetActive(true);
            mEnhancementMenu.GetComponent<TransitionAnimation>().ScaleIn();
        }
        mLeftRight.GetComponent<TransitionAnimation>().ScaleOut();
    }

    public void Left()
    {
        mUpgradeHand.GetComponent<UpgradeHand>().setHand(VRHand.LEFT);
        mEnhancementMenu.GetComponent<EnhancementsMenu>().setHand(VRHand.LEFT);
        mClassMenu.GetComponent<ClassMenu>().setHand(VRHand.LEFT);
        mFullTreeView.GetComponent<FullTreeView>().setHand(VRHand.LEFT);
        if (!mPlayer.GetWeaponObj(VRHand.LEFT))
        {
            Debug.Log("Taking you to class selection...");
            mClassMenu.SetActive(true);
            mClassMenu.GetComponent<TransitionAnimation>().ScaleIn();
        }
        else if (mCurrentMenu == "Upgrade")
        {
            Debug.Log("Taking you to the Upgrade Menu...");
            mUpgradeHand.SetActive(true);
            mUpgradeHand.GetComponent<TransitionAnimation>().ScaleIn();
        }
        else
        {
            Debug.Log("Taking you to the Enhancement Menu...");
            mEnhancementMenu.SetActive(true);
            mEnhancementMenu.GetComponent<TransitionAnimation>().ScaleIn();
        }
        mLeftRight.GetComponent<TransitionAnimation>().ScaleOut();
    }

    public void SwitchHands()
    {
        if (mUpgradeHand.GetComponent<UpgradeHand>().GetHand() == VRHand.RIGHT)
        {
            mUpgradeHand.GetComponent<UpgradeHand>().setHand(VRHand.LEFT);
            mEnhancementMenu.GetComponent<EnhancementsMenu>().setHand(VRHand.LEFT);
            mClassMenu.GetComponent<ClassMenu>().setHand(VRHand.LEFT);
        }
        else if (mUpgradeHand.GetComponent<UpgradeHand>().GetHand() == VRHand.LEFT)
        {
            mUpgradeHand.GetComponent<UpgradeHand>().setHand(VRHand.RIGHT);
            mEnhancementMenu.GetComponent<EnhancementsMenu>().setHand(VRHand.RIGHT);
            mClassMenu.GetComponent<ClassMenu>().setHand(VRHand.RIGHT);
        }
    }

    public void AddCash()
    {
        mPlayer.AddCash(500);
    }

    public void SetMenu(bool input)
    {
        mUpgradeOrEnhancement = input;
    }

    private void Start()
    {
        mPlayer = Player.GetInstance();
    }

}

