using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonTypes;
using UnityEngine.UI;
using TMPro;

public class FullTreeView : MonoBehaviour
{
    public class UIWeaponButton
    {
        public BaseWeapon weapon;
        public Image weaponIcon;
        public Image buttonBackground;
        public Image connector;
        public TextMeshProUGUI cost;
        public TextMeshProUGUI name;
        public int row;
        public int col;
        public int index;
        public UIWeaponButton top = null;
        public UIWeaponButton bot = null;
        public UIWeaponButton prev = null;
    };

    private UIWeaponButton[] uiWeaponButtons = new UIWeaponButton[15];

    public GameObject[] WeaponButtons;
    public Sprite[] buttons;
    public Sprite[] connectors;

    public GameObject mSellUpgrade;
    public GameObject mClassMenu;
    public TextMeshProUGUI mMoney;
    public TextMeshProUGUI mSellUpConfirm;
    public TextMeshProUGUI mSellWpnConfirm;

    private Player mPlayer;
    private VRHand mHand;
    private BaseWeapon mCurrentWeapon;
    private UIWeaponButton mCurrentUIButton;
    private int recursionIndex;
    private int mCurRow;
    private int mCurCol;
    private int mCurIndex;

    private uint sellCost = 0;
    private float sellWpnFactor = 0.8f;
    private float sellUpgFactor = 0.7f;

    public TextMeshProUGUI mDesc;
    public TextMeshProUGUI mDamage;
    public TextMeshProUGUI mAbility;
    public TextMeshProUGUI mFireRate;

    public GameObject mDamageBar;
    public GameObject mAbilityBar;
    public GameObject mFireRateBar;

    private const int RED      = 0;
    private const int GREEN    = 1;
    private const int BLUE     = 2;
    private const int GRAY     = 3;
    private const int COL_MULT = 4;

    // Start is called before the first frame update
    private void OnEnable()
    {
        mPlayer = Player.GetInstance();
        setWeapons();
        // base case:
        for( int i = 1; i < 15; i++)
        {
            setUIWeaponButtons(WeaponButtons[i], i);
            uiWeaponButtons[i].weaponIcon.sprite = uiWeaponButtons[i].weapon.mImage;
            uiWeaponButtons[i].name.text = uiWeaponButtons[i].weapon.mName;
            uiWeaponButtons[i].cost.text = uiWeaponButtons[i].weapon.mCost.ToString();
            debugUiWeaponButton(uiWeaponButtons[i]);
        }
        Image[] images = WeaponButtons[0].GetComponentsInChildren<Image>();
        uiWeaponButtons[0].buttonBackground = images[0];
        uiWeaponButtons[0].weaponIcon = images[1];
        // set new root

        TextMeshProUGUI[] texts = WeaponButtons[0].GetComponentsInChildren<TextMeshProUGUI>();
        uiWeaponButtons[0].name = texts[0];
        uiWeaponButtons[0].cost = texts[1];
        debugUiWeaponButton(uiWeaponButtons[0]);

        uiWeaponButtons[0].weaponIcon.sprite = uiWeaponButtons[0].weapon.mImage;
        uiWeaponButtons[0].name.text = uiWeaponButtons[0].weapon.mName;
        uiWeaponButtons[0].cost.text = uiWeaponButtons[0].weapon.mCost.ToString();
        Refresh();
    }
    void debugUiWeaponButton(UIWeaponButton uiWeaponButton)
    {
        if (uiWeaponButton.top != null)
            Debug.Log("Cur: " + uiWeaponButton.weapon.mId.ToString() + ", Top: " + uiWeaponButton.top.weapon.mId.ToString());
        if (uiWeaponButton.bot != null)
            Debug.Log("Cur: " + uiWeaponButton.weapon.mId.ToString() + ", Bot: " + uiWeaponButton.bot.weapon.mId.ToString());
        if (uiWeaponButton.prev != null)
            Debug.Log("Cur: " + uiWeaponButton.weapon.mId.ToString() + ", Prev: " + uiWeaponButton.prev.weapon.mId.ToString());
    }

    /* Function that updates the contents of the menu */
    void Refresh()
    {
        mCurrentWeapon = mPlayer.GetWeaponScript<BaseWeapon>( mHand);

        // set colors:
        setBlue(mCurrentUIButton);
        if (mCurrentUIButton.top != null)
        {
            setRedGreen(mCurrentUIButton.top);
        }
        if (mCurrentUIButton.bot != null)
        {
            setRedGreen(mCurrentUIButton.bot);
        }
        UpdateStats();

        UpdateSellPrices();
        // check if sell upgrade should be there.
        if (mCurrentWeapon.mPrPrevUpgrade == null)
            mSellUpgrade.SetActive(false);
        else
            mSellUpgrade.SetActive(true);

        mMoney.text = "$ " + mPlayer.GetCash().ToString();
    }

    void setRedGreen(UIWeaponButton uiWeaponButton)
    {
        int colour;
        Debug.Log("Red/Green");
        debugUiWeaponButton(uiWeaponButton);
        if (uiWeaponButton.weapon.mCost > mPlayer.GetCash())
        {
            colour = RED;
        }
        else
        {
            colour = GREEN;
        }
        uiWeaponButton.buttonBackground.sprite = buttons[colour];
        if (uiWeaponButton.connector != null)
        {
            uiWeaponButton.connector.sprite = connectors[getConnectorID(colour, uiWeaponButton)];
        }
        if (uiWeaponButton.top != null)
        {
            setRedGreen(uiWeaponButton.top);
        }
        if (uiWeaponButton.bot != null)
        {
            setRedGreen(uiWeaponButton.bot);
        }
    }
    void setBlue(UIWeaponButton uiWeaponButton)
    {
        Debug.Log("Blue");
        debugUiWeaponButton(uiWeaponButton);
        uiWeaponButton.buttonBackground.sprite = buttons[BLUE];
        if (uiWeaponButton.connector != null)
        {
            uiWeaponButton.connector.sprite = connectors[getConnectorID(BLUE, uiWeaponButton)];
        }
        if (uiWeaponButton.prev != null)
        {
            setBlue(uiWeaponButton.prev);
            if (uiWeaponButton.prev.top == uiWeaponButton)
            {
                if (uiWeaponButton.prev.bot != null)
                {
                    setGray(uiWeaponButton.prev.bot);
                }
            }
            else
            {
                setGray(uiWeaponButton.prev.top);
            }
        }

    }
    void setGray(UIWeaponButton uiWeaponButton)
    {
        Debug.Log("Gray");
        debugUiWeaponButton(uiWeaponButton);
        uiWeaponButton.buttonBackground.sprite = buttons[GRAY];
        if (uiWeaponButton.connector != null)
        {
            uiWeaponButton.connector.sprite = connectors[getConnectorID(GRAY, uiWeaponButton)];
        }

        if (uiWeaponButton.top != null)
        {
            setGray(uiWeaponButton.top);
        }
        if (uiWeaponButton.bot != null)
        {
            setGray(uiWeaponButton.bot);
        }
    }

    private void UpdateSellPrices()
    {
        mSellUpConfirm.text = "Are you sure you want to sell your current upgrade for $" + ((uint)(mCurrentWeapon.mCost * sellUpgFactor)).ToString() + "?";
        mSellWpnConfirm.text = "Are you sure you want to sell your current weapon for $" + GetSellPrice().ToString() + "?";
    }

    private uint GetSellPrice()
    {
        BaseWeapon weapon = mCurrentWeapon;
        sellCost = mCurrentWeapon.mCost;

        if (mCurrentWeapon.mPrPrevUpgrade == null)
        {
            return ((uint)(weapon.mCost * sellWpnFactor));
        }

        do
        {
            weapon = weapon.mPrPrevUpgrade.GetComponent<BaseWeapon>();
            sellCost += weapon.mCost;
        }
        while (weapon.mPrPrevUpgrade != null);

        return (uint)(sellCost * sellWpnFactor);
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
        mCurrentUIButton = mCurrentUIButton.prev;
        Refresh();
    }

    public void UpgradeUp(int id)
    {
        if (mCurrentUIButton.index == id)
        {
            if (mPlayer.BuyUpgradeTop(mHand))
            {
                mCurrentUIButton = mCurrentUIButton.top;
                Refresh();
            }
        }
    }

    public void UpgradeDn(int id)
    {
        if (mCurrentUIButton.index == id)
        {
            if (mPlayer.BuyUpgradeBottom(mHand))
            {
                mCurrentUIButton = mCurrentUIButton.bot;
                Refresh();
            }
        }
    }

    public void UpdateStats(int index)
    {
        BaseWeapon weapon = uiWeaponButtons[index].weapon;

        mDesc.text = weapon.mDescription;
        mDamage.text = weapon.mDamage.ToString();
        mAbility.text = weapon.mAbilities.ToString();
        mFireRate.text = weapon.mFireRate.ToString();

        mDamageBar.GetComponent<Image>().fillAmount = weapon.mDamage / 20f;
        mAbilityBar.GetComponent<Image>().fillAmount = weapon.mAbilities / 100f;
        mFireRateBar.GetComponent<Image>().fillAmount = weapon.mFireRate / 1000f;
    }

    public void UpdateStats()
    {
        BaseWeapon weapon = mCurrentUIButton.weapon;

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

    private void setWeapons()
    {
        mCurrentWeapon = mPlayer.GetWeaponScript<BaseWeapon>( mHand);
        string mID = mCurrentWeapon.mId;
        mCurRow = mID[2] - '0';
        mCurCol = mID[1] - '0';

        BaseWeapon tempWeapon = mCurrentWeapon;

        while (tempWeapon.mPrPrevUpgrade)
        // get root weapon
        {
            tempWeapon = tempWeapon.mPrPrevUpgrade.GetComponent<BaseWeapon>();
        }
        recursionIndex = 0;
        setWeaponsRecursive(tempWeapon);

    }

    void setWeaponsRecursive(BaseWeapon curWeapon)
    {
        /* Array of weapons buttons mapping where the index is position of weapon in tree (ie 0 -> weapon 1,1)
         *          2   3   4
         *      1  
         *          5   6   7
         *   0 
         *          9   10  11 
         *      8
         *          12  13  14
         */
        uiWeaponButtons[recursionIndex] = new UIWeaponButton();
        uiWeaponButtons[recursionIndex].weapon = curWeapon;
        uiWeaponButtons[recursionIndex].row = curWeapon.mId[2] - '0';
        uiWeaponButtons[recursionIndex].col = curWeapon.mId[1] - '0';
        uiWeaponButtons[recursionIndex].index = recursionIndex;

        if (uiWeaponButtons[recursionIndex].row == mCurRow && uiWeaponButtons[recursionIndex].col == mCurCol)
        {
            mCurrentUIButton = uiWeaponButtons[recursionIndex];
        }

        Debug.Log(uiWeaponButtons[recursionIndex].col.ToString() + uiWeaponButtons[recursionIndex].row.ToString());
        int index = recursionIndex;

        if (curWeapon.mPrNextUpgradeTop)
        {
            int top_index = recursionIndex;
            recursionIndex++;
            setWeaponsRecursive(curWeapon.mPrNextUpgradeTop.GetComponent<BaseWeapon>());
            uiWeaponButtons[top_index+1].prev = uiWeaponButtons[top_index];
            uiWeaponButtons[top_index].top = uiWeaponButtons[top_index + 1];
        }
        if (curWeapon.mPrNextUpgradeBottom)
        {
            recursionIndex++;
            int bot_index = recursionIndex;
            setWeaponsRecursive(curWeapon.mPrNextUpgradeBottom.GetComponent<BaseWeapon>());
            uiWeaponButtons[index].bot = uiWeaponButtons[bot_index];
            uiWeaponButtons[bot_index].prev = uiWeaponButtons[index];
        }
        return;
    }
    void setUIWeaponButtons(GameObject buttonRoot, int index)
    {
        Image[] images = buttonRoot.GetComponentsInChildren<Image>();
        uiWeaponButtons[index].connector = images[0];
        uiWeaponButtons[index].buttonBackground = images[1];
        // set new root
        buttonRoot = uiWeaponButtons[index].buttonBackground.gameObject;

        uiWeaponButtons[index].weaponIcon = buttonRoot.GetComponentsInChildren<Image>()[1];
        TextMeshProUGUI[] texts = buttonRoot.GetComponentsInChildren<TextMeshProUGUI>();
        uiWeaponButtons[index].name = texts[0];
        uiWeaponButtons[index].cost = texts[1];
    }
    
    int getConnectorID(int colour, UIWeaponButton button)
    {
        Debug.Log("Colour is " + colour.ToString());
        if (button.col < 4)
        {
            if (button.prev.top == button)
            {
                Debug.Log("Colour out is " + colour.ToString());
                return colour;
            }
            else
            {
                Debug.Log("Colour out is " + (COL_MULT + colour).ToString());
                return colour + COL_MULT;
            }
        }
        else
        {
            Debug.Log("Colour out is " + (2 * COL_MULT + colour).ToString());
            return 2 * COL_MULT + colour;
        }
    }
}
