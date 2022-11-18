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

public class UpgradeHand : MonoBehaviour
{
    private Player mPlayer = null;
    private VRHand mHand = VRHand.UNKNOWN;

    public GameObject mUpgradeUp;
    public GameObject mUpgradeDn;
    public GameObject mUpgradeCur;
    public GameObject mCurrentWeapon;
    public GameObject mSellUpgrade;
    public GameObject mClassMenu;
    public GameObject mButtonUp;
    public GameObject mButtonDn;
    public GameObject mButtonUpBkg;
    public GameObject mButtonDnBkg;
  
    public Sprite[] buttons;
    /* {1:grn-up, 2:red-up
        3:grn-up, 4:red-up
        5:grn-ln, 6:red-ln
        7:grn-bk, 8:red-bk} */

    public TextMeshProUGUI mCostUp;
    public TextMeshProUGUI mCostUpO;
    public TextMeshProUGUI mCostDn;
    public TextMeshProUGUI mCostDnO;
    public TextMeshProUGUI mNameUp;
    public TextMeshProUGUI mNameUpO;
    public TextMeshProUGUI mNameDn;
    public TextMeshProUGUI mNameDnO;
    public TextMeshProUGUI mNameCr;
    public TextMeshProUGUI mNameCrO;
    public TextMeshProUGUI mMoney;
    public TextMeshProUGUI mSellUpConfirm;
    public TextMeshProUGUI mSellWpnConfirm;

    public TextMeshProUGUI mCurrentHand;
    public GameObject mSwitchHandButton;

    public TextMeshProUGUI mDesc;
    public TextMeshProUGUI mDamage;
    public TextMeshProUGUI mAbility;
    public TextMeshProUGUI mFireRate;

    public GameObject mDamageBar;
    public GameObject mAbilityBar;
    public GameObject mFireRateBar;
    
    private BaseWeapon curWeapon;
    private BaseWeapon upWeapon;
    private BaseWeapon dnWeapon;

    private Color32 grnRGB = new Color32(68, 118, 70, 255);
    private Color32 redRGB = new Color32(118, 68, 68, 255);

    public Animator mAnimatorUp;
    public Animator mAnimatorDn;
    public Animator mAnimatorCu;

    private uint sellCost = 0;
    private float sellWpnFactor = 0.8f;
    private float sellUpgFactor = 0.7f;

    public void UpgradeUp()
    {
        mPlayer.BuyUpgradeTop(mHand);
        Refresh();
    }

    public void UpgradeDn()
    {
        mPlayer.BuyUpgradeBottom(mHand);
        Refresh();
    }

    public void SellWeapon()
    {
        mPlayer.SellWeapon(mHand, GetSellPrice());
        mClassMenu.SetActive(true);
        gameObject.SetActive(false);
    }

    public void SellUpgrade()
    {
        mPlayer.SellUpgrade(mHand);
        Refresh();
        CurHover();
    }

    private void UpdateSellPrices()
    {
        mSellUpConfirm.text = "Are you sure you want to sell your current upgrade for $" + ((uint)(curWeapon.mCost * sellUpgFactor)).ToString() + "?";
        mSellWpnConfirm.text = "Are you sure you want to sell your current weapon for $" + GetSellPrice().ToString() + "?";
    } 

    private uint GetSellPrice()
    {
        BaseWeapon weapon = curWeapon;
        sellCost = curWeapon.mCost;

        if (curWeapon.mPrPrevUpgrade == null)
        {
            return ((uint) (weapon.mCost * sellWpnFactor));
        }
        
        do
        {
            weapon = weapon.mPrPrevUpgrade.GetComponent<BaseWeapon>();
            sellCost += weapon.mCost;
        }
        while (weapon.mPrPrevUpgrade != null);

        return (uint)(sellCost * sellWpnFactor);
    }

    // HOVER FNS //
    public void OnUpHover()
    {
        UpdateStats(upWeapon);
        // SetHoverTrigger("UpHover");
    }

    public void OnDnHover()
    {
        UpdateStats(dnWeapon);
        // SetHoverTrigger("DnHover");
    }

    public void CurHover()
    {
        UpdateStats(curWeapon);
        // SetHoverTrigger("CuHover");
    }
    // END OF HOVER FNS //

    public void Refresh()
    {
        curWeapon = mPlayer.GetWeaponScript<BaseWeapon>( mHand);

        if (!curWeapon.mPrNextUpgradeTop)
        {
            NoUpgrades();
        }
        else if (!curWeapon.mPrNextUpgradeBottom)
        {
            OneUpgrade();
        }
        else
        {
            TwoUpgrades();
        }


        UpdateSellPrices();
        // check if sell upgrade should be there.
        if (curWeapon.mPrPrevUpgrade == null)
            mSellUpgrade.SetActive(false);
        else
            mSellUpgrade.SetActive(true);

        mMoney.text = "$ " + mPlayer.GetCash().ToString();

        if (!mPlayer.GetWeaponObj(VRHand.LEFT) || !mPlayer.GetWeaponObj(VRHand.RIGHT))
        {
            mSwitchHandButton.GetComponent<Button>().interactable = false;
        }
        else
        {
            mSwitchHandButton.GetComponent<Button>().interactable = true;
        }
        if (mHand == VRHand.RIGHT)
        {
            mCurrentHand.text = "Hand: Right"; 
        }

        else
        {
            mCurrentHand.text = "Hand: LEFT";
        }

    }

    private void TwoUpgrades()
    {
        // set refs to upgrades
        upWeapon = curWeapon.mPrNextUpgradeTop.GetComponent<BaseWeapon>();
        dnWeapon = curWeapon.mPrNextUpgradeBottom.GetComponent<BaseWeapon>();

        // set the buttons
        setButtons(upWeapon, mButtonUp, mButtonUpBkg, mUpgradeUp, 0, 1, mCostUp, mCostUpO, mNameUp, mNameUpO);
        setButtons(dnWeapon, mButtonDn, mButtonDnBkg, mUpgradeDn, 2, 3, mCostDn, mCostDnO, mNameDn, mNameDnO);

        // update current buttons
        mCurrentWeapon.GetComponent<Image>().sprite = curWeapon.mImage;
        mNameCr.text = mNameCrO.text = curWeapon.mName;
        UpdateStats(curWeapon);

        // set positions
        mUpgradeDn.SetActive(true);
        mUpgradeUp.SetActive(true);
        mUpgradeUp.transform.localPosition     = new Vector3(-98.3f,  40f, 0f);
        mUpgradeCur.transform.localPosition = new Vector3(-150.4f, 0f, 0f);
        mUpgradeCur.transform.localScale    = new Vector3(1f, 1f, 1f);

    }

    private void OneUpgrade()
    {
        // update next button
        upWeapon = curWeapon.mPrNextUpgradeTop.GetComponent<BaseWeapon>();
        setButtons(upWeapon, mButtonUp, mButtonUpBkg, mUpgradeUp, 4, 5, mCostUp, mCostUpO, mNameUp, mNameUpO);

        // update current buttons
        mCurrentWeapon.GetComponent<Image>().sprite = curWeapon.mImage;
        mNameCr.text = mNameCrO.text = curWeapon.mName;
        UpdateStats(curWeapon);

        // update positions
        mUpgradeUp.transform.localPosition     = new Vector3(-85f, 0f, 0f);
        mUpgradeCur.transform.localPosition = new Vector3(-165f, 0f, 0f);
        mUpgradeCur.transform.localScale    = new Vector3(1f, 1f, 1f);
        mUpgradeDn.SetActive(false);
        mUpgradeUp.SetActive(true);
    }

    private void NoUpgrades()
    {
        mCurrentWeapon.GetComponent<Image>().sprite = curWeapon.mImage;
        mNameCr.text = mNameCrO.text = curWeapon.mName;
        UpdateStats(curWeapon);

        mUpgradeCur.transform.localPosition = new Vector3(-112.5f, 0f, 0f);
        Debug.Log(mUpgradeCur.transform.localScale);
        mUpgradeCur.transform.localScale = new Vector3(1.66f, 1.66f, 1f);
        Debug.Log(mUpgradeCur.transform.localScale);
        mUpgradeDn.SetActive(false);
        mUpgradeUp.SetActive(false);
    }                                               

    private void setButtons(BaseWeapon weapon, GameObject button, GameObject buttonbk, GameObject menu, uint green, uint red, TextMeshProUGUI cost, TextMeshProUGUI costOutline, TextMeshProUGUI name, TextMeshProUGUI nameOutline)
    {
        if (mPlayer.GetCash() < weapon.mCost)
        // player can't afford the next upgrade
        {
            menu.GetComponent<Image>().sprite = buttons[red];
            buttonbk.GetComponent<Image>().sprite = buttons[7];
            costOutline.outlineColor = nameOutline.outlineColor = redRGB;
        }
        else
        {
            menu.GetComponent<Image>().sprite = buttons[green];
            buttonbk.GetComponent<Image>().sprite = buttons[6];
            costOutline.outlineColor = nameOutline.outlineColor = grnRGB;
        }
        // update weapon image
        cost.text = costOutline.text = "$ " + weapon.mCost.ToString();
        name.text = nameOutline.text = weapon.mName;
        button.GetComponent<Image>().sprite = weapon.mImage;
    }

    private void UpdateStats(BaseWeapon weapon)
    {
        mDesc.text = weapon.mDescription;
        mDamage.text = weapon.mDamage.ToString();
        mAbility.text = weapon.mAbilities.ToString();
        mFireRate.text = weapon.mFireRate.ToString();

        mDamageBar.GetComponent<Image>().fillAmount = weapon.mDamage / 20f;
        mAbilityBar.GetComponent<Image>().fillAmount = weapon.mAbilities / 100f;
        mFireRateBar.GetComponent<Image>().fillAmount = weapon.mFireRate / 1000f;
    }

    public void setHand(VRHand hand)
    {
        mHand = hand;
    }

    public VRHand GetHand()
    {
        return mHand;
    }

    private void OnEnable()
    {
        mPlayer = Player.GetInstance();
        Refresh();
    }


}
