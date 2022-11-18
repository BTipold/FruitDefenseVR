// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------

using CommonTypes;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class EnhancementsMenu : MonoBehaviour
{
    private Player mPlayer = null;
    private VRHand mHand = VRHand.UNKNOWN;

    public GameObject[] mTop;
    public GameObject[] mMid;
    public GameObject[] mBot;
    public GameObject mSellUpgrade;
    public GameObject[] mButtonTop;
    public GameObject[] mButtonMid;
    public GameObject[] mButtonBot;
    public GameObject[] mButtonTopBk;
    public GameObject[] mButtonMidBk;
    public GameObject[] mButtonBotBk;

  
    public Sprite[] buttons;
    /* {0:grn-ln, 1:red-ln
        2:gry-ln, 3:blu-ln
        4:grn-bk, 5:red-bk
        6:gry-bk, 7:blu-bk}
    */
    private uint grnLn = 0;
    private uint redLn = 1;
    private uint grayLn = 2;
    private uint blueLn = 3;
    private uint grnBk = 4;
    private uint redBk = 5;
    private uint grayBk = 6;
    private uint blueBk = 7;

    public TextMeshProUGUI[] mCostTop;
    public TextMeshProUGUI[] mCostTopO;
    public TextMeshProUGUI[] mCostMid;
    public TextMeshProUGUI[] mCostMidO;
    public TextMeshProUGUI[] mCostBot;
    public TextMeshProUGUI[] mCostBotO;
    public TextMeshProUGUI[] mNameTop;
    public TextMeshProUGUI[] mNameTopO;
    public TextMeshProUGUI[] mNameMid;
    public TextMeshProUGUI[] mNameMidO;
    public TextMeshProUGUI[] mNameBot;
    public TextMeshProUGUI[] mNameBotO;
    public TextMeshProUGUI mMoney;
    public TextMeshProUGUI mCurrentHand;
    public TextMeshProUGUI mSellUpConfirm;
    public TextMeshProUGUI mSellWpnConfirm;

    public TextMeshProUGUI mDesc;

    private Color32 grnRGB = new Color32(68, 118, 70, 255);
    private Color32 redRGB = new Color32(118, 68, 68, 255);
    private Color32 grayRGB = new Color32(88, 88, 88, 255);
    private Color32 blueRGB = new Color32(68, 110, 118, 255);

    public Animator mSelectionAnim;
   
    private uint sellCost = 0;
    private float sellPathFactor = 0.8f;
    private float sellSelecFactor = 0.7f;

    private WeaponClass mClass;
    private uint mTopLevel;
    private uint mMidLevel;
    private uint mBotLevel;
    private bool[] isClosed = { false, false, false };

    private Enhancement mSelectedEnhancement = null;



    public void UpgradeTop(int level)
    {
        // Buy next Enhancement Top Path
        uint cur_level = mPlayer.GetCurrentEnhancmentLevel(mHand, 1);
        if (cur_level == level - 1)
        {
            uint cost = GetTop(mClass, (uint)level - 1).cost;
            mPlayer.AddEnhancement(mHand, cost, 1); // Hand, Cost, Category
            mSelectedEnhancement = GetTop(mClass, (uint)level - 1);
            SetAnimState(level);
            Refresh();
        }
        else if (cur_level == level)
        {
            SetAnimState(level); // Top states 1-3
            mSelectedEnhancement = GetTop(mClass, (uint)level - 1);
        }
    }

    public void UpgradeMid(int level)
    {
        // Buy next Enhancement Mid Path
        uint cur_level = mPlayer.GetCurrentEnhancmentLevel(mHand, 2);
        if (cur_level == level - 1)
        {
            uint cost = GetMid(mClass, (uint)level-1).cost;
            mPlayer.AddEnhancement(mHand, cost, 2); // Hand, Cost, Category
            mSelectedEnhancement = GetMid(mClass, (uint)level-1);
            SetAnimState(level + 3);
            Refresh();
        }
        else if (cur_level == level)
        {
            SetAnimState(level + 3); // Mid states 4-6
            mSelectedEnhancement = GetMid(mClass, (uint)level-1);
        }
    }

    public void UpgradeBot(int level)
    {
        // Buy next Enhancement Mid Path
        uint cur_level = mPlayer.GetCurrentEnhancmentLevel(mHand, 3);
        if (cur_level == level - 1)
        {
            uint cost = GetBot(mClass, (uint)level - 1).cost;
            mPlayer.AddEnhancement(mHand, cost, 3); // Hand, Cost, Category
            mSelectedEnhancement = GetBot(mClass, (uint)level - 1);
            SetAnimState(level + 6);
            Refresh();
        }
        else if (cur_level == level)
        {
            SetAnimState(level + 6); // Bot states 7-9
            mSelectedEnhancement = GetBot(mClass, (uint)level - 1);
        }
    }

    public void SellPath()
    {
        // Sell Path
        Refresh();
    }

    public void SellSelected()
    {
        // Sell Selected
        Refresh();
    }

    private void UpdateSellPrices()
    {
       // mSellUpConfirm.text = "Are you sure you want to sell your selected enhancement for $" + ((uint)(curWeapon.mCost * sellUpgFactor)).ToString() + "?";
       // mSellWpnConfirm.text = "Are you sure you want to sell your selected path for $" + GetSellPrice().ToString() + "?";
    } 

/*    private uint GetSellPrice()
    {
        Enhancement enhancement = curWeapon;
        sellCost = curWeapon.mCost;

        if (curWeapon.mPrPrevUpgrade == null)
        {
            return ((uint) (weapon.mCost * sellPathFactor));
        }
        
        do
        {
            weapon = weapon.mPrPrevUpgrade.GetComponent<BaseWeapon>();
            sellCost += weapon.mCost;
        }
        while (weapon.mPrPrevUpgrade != null);

        return (uint)(sellCost * sellPathFactor);
    }
*/
    // HOVER FNS //
    public void OnTopHover(int index)
    {
        UpdateDescription(GetTop(mClass, (uint)index - 1));
        // SetHoverTrigger("UpHover");
    }

    public void OnMidHover(int index)
    {
        UpdateDescription(GetMid(mClass, (uint)index - 1));
        // SetHoverTrigger("DnHover");
    }

    public void OnBotHover(int index)
    {
        UpdateDescription(GetBot(mClass, (uint)index - 1));
        // SetHoverTrigger("CuHover");
    }
    // END OF HOVER FNS //

    public void OffHover()
    {
        UpdateDescription(mSelectedEnhancement);
        // set description to the selected upgrade.
    }

    private void SetAnimState(int stateNum) // helper function to set and reset all hover triggers
    {
        mSelectionAnim.SetInteger("Selected", stateNum);
    }

    public void Refresh()
    {
        // get enhancement levels
        mTopLevel = mPlayer.GetCurrentEnhancmentLevel(mHand, 1);
        mMidLevel = mPlayer.GetCurrentEnhancmentLevel(mHand, 2);
        mBotLevel = mPlayer.GetCurrentEnhancmentLevel(mHand, 3);
        
        // set closed paths (if any) 
        // paths are re-opened in the sell function.
        if (mTopLevel > 0 && mMidLevel > 0)
        {
            isClosed[2] = true;
        }
        else if (mTopLevel > 0 && mBotLevel > 0)
        {
            isClosed[1] = true;
        }
        else if (mBotLevel > 0 && mMidLevel > 0)
        {
            isClosed[0] = true;
        }

        // set colours
        setTopButtonColours(isClosed[0]);
        setMidButtonColours(isClosed[1]);
        setBotButtonColours(isClosed[2]);

        UpdateSellPrices();
        // check if sell upgrade should be there.
        /*if (curWeapon.mPrPrevUpgrade == null)
            mSellUpgrade.SetActive(false);
        else
            mSellUpgrade.SetActive(true);*/

        mMoney.text = "$ " + mPlayer.GetCash().ToString();
        if (mHand == VRHand.RIGHT)
        {
            mCurrentHand.text = "Hand: Right";
        }

        else
        {
            mCurrentHand.text = "Hand: LEFT";
        }

    }

    private void setGrayButton(GameObject buttonbk, GameObject line, TextMeshProUGUI costOutline, TextMeshProUGUI nameOutline)
    {
        if (line != null)
        {
            line.GetComponent<Image>().sprite = buttons[grayLn];
        }
        buttonbk.GetComponent<Image>().sprite = buttons[grayBk];
        costOutline.outlineColor = nameOutline.outlineColor = grayRGB;
    }
    private void setBlueButton(GameObject buttonbk, GameObject line, TextMeshProUGUI costOutline, TextMeshProUGUI nameOutline)
    {
        if (line != null)
        {
            line.GetComponent<Image>().sprite = buttons[blueLn];
        }
        buttonbk.GetComponent<Image>().sprite = buttons[blueBk];
        costOutline.outlineColor = nameOutline.outlineColor = blueRGB;
    }
    private void setRedGreenButton(Enhancement enhancement, GameObject buttonbk, GameObject line, TextMeshProUGUI costOutline, TextMeshProUGUI nameOutline)
    {
        if (mPlayer.GetCash() < enhancement.cost)
        // player can't afford the next upgrade
        {
            if (line != null)
            {
                line.GetComponent<Image>().sprite = buttons[redLn];
            }
            buttonbk.GetComponent<Image>().sprite = buttons[redBk];
            costOutline.outlineColor = nameOutline.outlineColor = redRGB;
        }
        else
        {
            if (line != null)
            {
                line.GetComponent<Image>().sprite = buttons[grnLn];
            }
            buttonbk.GetComponent<Image>().sprite = buttons[grnBk];
            costOutline.outlineColor = nameOutline.outlineColor = grnRGB;
        }
    }

    private void setTopButtonColours(bool closed)
    {
        if (closed)
        {
            // if path is closed, all are set to gray
            for (uint i = 0; i < 3; i++)
            {
                setGrayButton(mButtonTopBk[i], mTop[i], mCostTopO[i], mNameTopO[i]);
            }
        }
        else
        {
            for (uint i = 0; i < 3; i++)
            {
                // mTopLevel is on [0,3], i is on [0,2]
                // when mTopLevel == 0, i = 0 would be red/green
                // when mTopLevel == 1, i = 1 would be red/green, i = 0 would be blue, i = 2 would be gray
                if (i < mTopLevel)
                {
                    setBlueButton(mButtonTopBk[i], mTop[i], mCostTopO[i], mNameTopO[i]);
                    mButtonTopBk[i].GetComponent<Animator>().SetTrigger("Pressable");
                }
                else if (i == mTopLevel)
                {
                    setRedGreenButton(GetTop(mClass, mTopLevel), mButtonTopBk[i], mTop[i], mCostTopO[i], mNameTopO[i]);
                    mButtonTopBk[i].GetComponent<Animator>().SetTrigger("Pressable");
                }
                else // i > mTopLevel
                {
                    setGrayButton(mButtonTopBk[i], mTop[i], mCostTopO[i], mNameTopO[i]);
                    mButtonTopBk[i].GetComponent<Animator>().ResetTrigger("Pressable");
                }
            }
        }
        return;
    }
    private void setMidButtonColours(bool closed)
    {
        if (closed)
        {
            // if path is closed, all are set to gray
            for (uint i = 0; i < 3; i++)
            {
                setGrayButton(mButtonMidBk[i], mMid[i], mCostMidO[i], mNameMidO[i]);
            }
        }
        else
        {
            for (uint i = 0; i < 3; i++)
            {
                // mMidLevel is on [0,3], i is on [0,2]
                // when mMidLevel == 0, i = 0 would be red/green
                // when mMidLevel == 1, i = 1 would be red/green, i = 0 would be blue, i = 2 would be gray
                if (i < mMidLevel)
                {
                    setBlueButton(mButtonMidBk[i], mMid[i], mCostMidO[i], mNameMidO[i]);
                    mButtonMidBk[i].GetComponent<Animator>().SetTrigger("Pressable");
                }
                else if (i == mMidLevel)
                {
                    setRedGreenButton(GetMid(mClass, mMidLevel), mButtonMidBk[i], mMid[i], mCostMidO[i], mNameMidO[i]);
                    mButtonMidBk[i].GetComponent<Animator>().SetTrigger("Pressable");
                }
                else // i > mMidLevel
                {
                    setGrayButton(mButtonMidBk[i], mMid[i], mCostMidO[i], mNameMidO[i]);
                    mButtonMidBk[i].GetComponent<Animator>().ResetTrigger("Pressable");
                }
            }
        }
        return;
    }
    private void setBotButtonColours(bool closed)
    {
        if (closed)
        {
            // if path is closed, all are set to gray
            for (uint i = 0; i < 3; i++)
            {
                setGrayButton(mButtonBotBk[i], mBot[i], mCostBotO[i], mNameBotO[i]);
            }
        }
        else
        {
            for (uint i = 0; i < 3; i++)
            {
                // mBotLevel is on [0,3], i is on [0,2]
                // when mBotLevel == 0, i = 0 would be red/green
                // when mBotLevel == 1, i = 1 would be red/green, i = 0 would be blue, i = 2 would be gray
                if (i < mBotLevel)
                {
                    setBlueButton(mButtonBotBk[i], mBot[i], mCostBotO[i], mNameBotO[i]);
                    mButtonBotBk[i].GetComponent<Animator>().SetTrigger("Pressable");
                }
                else if (i == mBotLevel)
                {
                    setRedGreenButton(GetBot(mClass, mBotLevel), mButtonBotBk[i], mBot[i], mCostBotO[i], mNameBotO[i]);
                    mButtonBotBk[i].GetComponent<Animator>().SetTrigger("Pressable");

                }
                else // i > mBotLevel
                {
                    setGrayButton(mButtonBotBk[i], mBot[i], mCostBotO[i], mNameBotO[i]);
                    mButtonBotBk[i].GetComponent<Animator>().ResetTrigger("Pressable");
                }
            }
        }
        return;
    }

    private void setEnhancementData(Enhancement enhancement, GameObject button, TextMeshProUGUI cost, TextMeshProUGUI costOutline, TextMeshProUGUI name, TextMeshProUGUI nameOutline)
    {
        cost.text = costOutline.text = "$ " + enhancement.cost.ToString();
        name.text = nameOutline.text = enhancement.title;
        button.GetComponent<Image>().sprite = enhancement.icon;
    }

    private void setTopEnhancementData()
    {
        for (uint i = 0; i < 3; i++)
        {
            setEnhancementData(GetTop(mClass, i), mButtonTop[i], mCostTop[i], mCostTopO[i], mNameTop[i], mNameTopO[i]);
        }
    }
    private void setMidEnhancementData()
    {
        for (uint i = 0; i < 3; i++)
        {
            setEnhancementData(GetMid(mClass, i), mButtonMid[i], mCostMid[i], mCostMidO[i], mNameMid[i], mNameMidO[i]);
        }
    }
    private void setBotEnhancementData()
    {
        for (uint i = 0; i < 3; i++)
        {
            setEnhancementData(GetBot(mClass, i), mButtonBot[i], mCostBot[i], mCostBotO[i], mNameBot[i], mNameBotO[i]);
        }
    }

    private void UpdateDescription(Enhancement enhancement)
    {
        mDesc.text = enhancement.description;
    }

    public void setHand(VRHand hand)
    {
        mHand = hand;
    }

    private void OnEnable()
    {
        mPlayer = Player.GetInstance();
        // set class (class getter)
        mClass = 0;

        // set all image sprites and cost/name values
        setTopEnhancementData();
        setMidEnhancementData();
        setBotEnhancementData();
        setGrayButton(mButtonTopBk[0], mTop[0], mCostTopO[0], mNameTopO[0]);
        mSelectedEnhancement = GetBot(mClass, 1);
        mSelectedEnhancement.description = "";
        Refresh();
    }


    // --------------------------------------------------------
    public EnhancementGroup[] enhancements;
    private Dictionary<WeaponClass, EnhancementGroup> mEnhancementMap = new Dictionary<WeaponClass, EnhancementGroup>();

    private void Awake( )
    {
        foreach (EnhancementGroup enhancement in enhancements)
        {
            mEnhancementMap.Add( enhancement.weaponClass, enhancement );
        }
    }

    private Enhancement GetTop( WeaponClass wClass, uint level )
    {
        return mEnhancementMap[wClass].top[level];
    }

    private Enhancement GetMid( WeaponClass wClass, uint level )
    {
        return mEnhancementMap[wClass].mid[level];
    }

    private Enhancement GetBot( WeaponClass wClass, uint level )
    {
        return mEnhancementMap[wClass].bot[level];
    }

    // Example Method
    private bool BuyEnhancement( )
    {
        uint cost = GetTop(WeaponClass.LIGHT, 1).cost;
        return mPlayer.AddEnhancement( mHand, cost, LightEnhancements.CASH ); // Hand, Cost, Category
    }

    // Example Method
    private bool SellEnhancement( )
    {
        uint cost = GetTop(WeaponClass.LIGHT, 1).cost;
        return mPlayer.SellEnhancement( VRHand.LEFT, cost, LightEnhancements.CASH );
    }

    // Example Method
    private void GetEnhancmentInfo()
    {
        WeaponClass wClass = mPlayer.GetWeaponScript<BaseWeapon>(mHand).mWeaponClass;
        Enhancement cash1 = GetTop(wClass, 0);

        uint cost = cash1.cost;
        string descr = cash1.description;
        string title = cash1.title;
        Sprite icon = cash1.icon;
    }
}
