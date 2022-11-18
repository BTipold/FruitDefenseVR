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
using TMPro;
using CommonTypes;
using UnityEngine.UI;

public class ClassMenu : MonoBehaviour
{
    private Player mPlayer = null;
    private VRHand mHand = VRHand.UNKNOWN;

    public GameObject mWeaponBkgd;
    public GameObject mUpgradeHand;

    public Sprite mRedButton;
    public Sprite mGrnButton;

    public GameObject[] mWeapons;
    private string[] mWeaponNames = { "guns", "rockets", "magic", "laser guns" };
    private uint curIndex = 0; // 0:light, 1:heavy, 2:magic, 3:demolitions
    private uint maxIndex = 1;

    public GameObject mButton;

    public TextMeshProUGUI mCost;
    public TextMeshProUGUI mCostO;
    public TextMeshProUGUI mName;
    public TextMeshProUGUI mNameO;
    public TextMeshProUGUI mClassO;
    public TextMeshProUGUI mClass;
    public TextMeshProUGUI mMoney;

    public TextMeshProUGUI mDesc;
    public TextMeshProUGUI mDamage;
    public TextMeshProUGUI mAbility;
    public TextMeshProUGUI mFireRate;

    public GameObject mDamageBar;
    public GameObject mAbilityBar;
    public GameObject mFireRateBar;

    private BaseWeapon mWeapon;

    private Color32 grnRGB = new Color32(68, 118, 70, 255);
    private Color32 redRGB = new Color32(118, 68, 68, 255);

    public void Buy()
    {
        bool bought = mPlayer.BuyBase(mHand, mWeapons[curIndex]);
        if (bought)
        {
            mUpgradeHand.SetActive(true);
            mUpgradeHand.GetComponent<TransitionAnimation>().ScaleIn();
            gameObject.GetComponent<TransitionAnimation>().ScaleOut();
        }
    }

    private void Refresh(uint page)
    {
        mWeapon = mWeapons[page].GetComponent<BaseWeapon>();
        setColors(mWeapon, mButton, mWeaponBkgd, mCost, mCostO, mName, mNameO);
        UpdateStats(mWeapon);
        mMoney.text = "$ " + mPlayer.GetCash().ToString();
        mClass.text = mClassO.text = mWeaponNames[page];
    }

    public void NextPage()
    {
        if (curIndex < maxIndex)
            curIndex += 1;
        else curIndex = 0;
        Refresh(curIndex);
    }

    public void PrevPage()
    {
        if (curIndex > 0)
            curIndex -= 1;
        else curIndex = maxIndex;
        Refresh(curIndex);
    }

    private void setColors(BaseWeapon weapon, GameObject button, GameObject menu, TextMeshProUGUI cost, TextMeshProUGUI costOutline, TextMeshProUGUI name, TextMeshProUGUI nameOutline)
    {
        if (mPlayer.GetCash() < weapon.mCost)
        // player can't afford the next upgrade
        {
            menu.GetComponent<Image>().sprite = mRedButton;
            costOutline.outlineColor = nameOutline.outlineColor = redRGB;
        }
        else
        {
            menu.GetComponent<Image>().sprite = mGrnButton;
            cost.text = costOutline.text = weapon.mCost.ToString();
            costOutline.outlineColor = nameOutline.outlineColor = grnRGB;
        }
        // update weapon image
        cost.text = costOutline.text = "$ " + weapon.mCost.ToString();
        name.text = nameOutline.text = weapon.mName;
        button.GetComponent<Image>().sprite = weapon.mImage;
        button.GetComponent<Button>().enabled = !weapon.mComingSoon;
        button.GetComponent<Animator>().enabled = !weapon.mComingSoon;
    }

    private void UpdateStats(BaseWeapon weapon)
    {
        mDesc.text = weapon.mDescription;
        mDamage.text = weapon.mDamage.ToString();
        mAbility.text = weapon.mAbilities.ToString();
        mFireRate.text = weapon.mFireRate.ToString();

        mDamageBar.GetComponent<Image>().fillAmount = weapon.mDamage / 100f;
        mAbilityBar.GetComponent<Image>().fillAmount = weapon.mAbilities / 100f;
        mFireRateBar.GetComponent<Image>().fillAmount = weapon.mFireRate / 1000f;
    }

    public void setHand(VRHand hand)
    {
        mHand = hand;
    }

    private void OnEnable()
    {
        mPlayer = Player.GetInstance();
        Refresh(curIndex);
    }

    private void Start()
    {
        Refresh(curIndex);
    }


}