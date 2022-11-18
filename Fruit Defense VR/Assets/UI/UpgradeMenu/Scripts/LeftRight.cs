using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonTypes;
using UnityEngine.UI;
using TMPro;

public class LeftRight : MonoBehaviour
{
    private BaseWeapon leftWeapon;
    private BaseWeapon rightWeapon;
    private Player mPlayer;
    private VRHand mHand;

    private const uint red = 0;
    private const uint green = 1;
    private const uint blue = 2;
    private Color32 redRGB = new Color32(118, 68, 68, 255);
    private Color32 greenRGB = new Color32(68, 118, 70, 255);
    private Color32 blueRGB = new Color32(68, 110, 118, 255);

    public GameObject mLeftWeaponObj;
    public GameObject mRightWeaponObj;
    public GameObject mLeftBk;
    public GameObject mRightBk;
    public Sprite nullSprite;
    public TextMeshProUGUI mLeftHandName;
    public TextMeshProUGUI mRightHandName;

    public Sprite[] buttons;

    // Start is called before the first frame update
    void OnEnable()
    {
        // Left Hand
        mPlayer = Player.GetInstance();
        if (!mPlayer.GetWeaponObj(VRHand.LEFT))
        {
            mLeftWeaponObj.GetComponent<Image>().sprite = nullSprite;
        }
        else
        {
            leftWeapon = mPlayer.GetWeaponScript<BaseWeapon>( VRHand.LEFT);
            mLeftWeaponObj.GetComponent<Image>().sprite = leftWeapon.mImage;
            BaseWeapon leftTop = null;
            BaseWeapon leftBot = null ;
            if (leftWeapon.mPrNextUpgradeTop)
            {
                leftTop = leftWeapon.mPrNextUpgradeTop.GetComponent<BaseWeapon>();
            }
            if (leftWeapon.mPrNextUpgradeBottom)
            {
                leftBot = leftWeapon.mPrNextUpgradeBottom.GetComponent<BaseWeapon>();
            }

            setButtons(leftTop, leftBot, mLeftBk, mLeftHandName);
        }
        // Right Hand
        if (!mPlayer.GetWeaponObj(VRHand.RIGHT))
        {
            mRightWeaponObj.GetComponent<Image>().sprite = nullSprite;
        }
        else
        {
            rightWeapon = mPlayer.GetWeaponScript<BaseWeapon>( VRHand.RIGHT);
            mRightWeaponObj.GetComponent<Image>().sprite = rightWeapon.mImage;
            BaseWeapon rightTop = null;
            BaseWeapon rightBottom = null;
            if (!(!rightWeapon.mPrNextUpgradeTop))
            {
                rightTop = rightWeapon.mPrNextUpgradeTop.GetComponent<BaseWeapon>();
            }
            if (!(!rightWeapon.mPrNextUpgradeBottom))
            {
                rightBottom = rightWeapon.mPrNextUpgradeBottom.GetComponent<BaseWeapon>();
            }

            setButtons(rightTop, rightBottom, mRightBk, mRightHandName);
        }
       
    }

    private void setButtons(BaseWeapon up, BaseWeapon down, GameObject buttonbk, TextMeshProUGUI handName)
    {
        if (!up) 
        // no upgrades left
        {
            buttonbk.GetComponent<Image>().sprite = buttons[blue];
            handName.outlineColor = blueRGB;
            return;
        }
        else if (mPlayer.GetCash() > up.mCost)
        // player can afford a new upgrade
        {
            buttonbk.GetComponent<Image>().sprite = buttons[green];
            handName.outlineColor = greenRGB;
            return;
        }
        else if (!down)
        // only one upgrade, player can't afford the upgrade
        {
            buttonbk.GetComponent<Image>().sprite = buttons[red];
            handName.outlineColor = redRGB;
            return;
        }
        else if (mPlayer.GetCash() > down.mCost)
        // player can afford the down upgrade (which exists)
        {
            buttonbk.GetComponent<Image>().sprite = buttons[green];
            handName.outlineColor = greenRGB;
            return;
        }
        else
        // player cannot afford any upgrades
        {
            buttonbk.GetComponent<Image>().sprite = buttons[red];
            handName.outlineColor = redRGB;
            return;
        }

    }
}
