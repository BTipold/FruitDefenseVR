// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using CommonTypes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


public sealed class Player : MonoBehaviour
{
    // Inspector reference to VR head camera transform
    public Transform mPlayerPosition = null;

    // Inspector reference to controller prefabs
    public GameObject mControllerPrefabL = null;
    public GameObject mControllerPrefabR = null;

    // Inspector reference to XRBaseController component
    public XRBaseController mControllerL = null;
    public XRBaseController mControllerR = null;

    // Inspector reference to XRRayInteractor component
    public XRRayInteractor mRayR = null;
    public XRRayInteractor mRayL = null;

    // Inspector reference to pointer visual
    public XRInteractorLineVisual mRayVisualL = null;
    public XRInteractorLineVisual mRayVisualR = null;

    // Inspector reference to the weapon prefab map. 
    // This is used to find weapons to instantiate them.
    public WeaponPrefabMap mWeaponPrefabMap = null;

    // To be ticked if player is used in the main menu.
    // This is used to bypass some game specific logic
    // that would otherwise fail.
    public bool mMainMenuMode = false;

    // Spawn location of player, and center point of the playspace.
    private Vector3 mPlayerOrigin = new Vector3();

    // Left and right Rays, stored in HandedType
    private HandedType<XRRayInteractor> mRay = new HandedType<XRRayInteractor>(null);

    // Left and right XRBaseController, stored in HandedType
    private HandedType<XRBaseController> mXrController = new HandedType<XRBaseController>(null);

    // Left and right controller prefabs, stored in HandedType
    private HandedType<GameObject> mControllerPrefab = new HandedType<GameObject>(null);

    // Left and right weapon anchors, stored in HandedType
    private HandedType<Transform> mWeaponAnchor = new HandedType<Transform>(null);

    // Left and right hand anchors, stored in HandedType
    private HandedType<Transform> mHandAnchor = new HandedType<Transform>(null);

    // Left and right weapons, stored in HandedType
    private HandedType<GameObject> mWeapon = new HandedType<GameObject>(null);

    // Left and right controller model instances, stored in HandedType
    private HandedType<GameObject> mControllerInstance = new HandedType<GameObject>(null);

    // Stores enhancements settings, and manages buying/selling them
    private EnhancementManager mEnhancements = new EnhancementManager();

    // Allows EnhancementManager to be saved to disk
    private PersistentDataType<EnhancementManager> mSaveableEnhancements;

    // Allows player origin to be saved to disk
    private PersistentDataType<SerializableVector3> mSaveableOrigin;

    // Allows Left Weapon KEY to be saved to disk
    private PersistentDataType<string> mSaveableLWeapon;

    // Allows Right Weapon KEY to be saved to disk
    private PersistentDataType<string> mSaveableRWeapon;

    // Allows cash to be saved to disk
    public PersistentDataType<uint> mCash;

    // Singleton instance, set in Awake()
    private static Player mInstance = null;

    // Portion of cash you get back for selling something
    private const float SELL_FACTOR = 0.7f;

    // -----------------------------------------------------------
    // @Summary: getter for the singleton instance. Use this to 
    //   access the player's attributes. 
    // @Return: returns the one instance of Player.
    // -----------------------------------------------------------
    public static Player GetInstance()
    {
        return mInstance;
    }

    // -----------------------------------------------------------
    // @Summary: getter for the player's current position.
    // @Return: returns transform of CentreEyeCamera.
    // -----------------------------------------------------------
    public Transform GetPlayerPosition()
    {
        return mPlayerPosition;
    }

    // -----------------------------------------------------------
    // @Summary: checks if the player has a weapon in the
    //   requested hand or cached in correct hand.
    // @Param: hand - hand to check.
    // @Return: bool - returns true if hand has a weapon.
    // -----------------------------------------------------------
    public bool HasWeapon( VRHand hand )
    {
        bool hasWeapon = ( GetWeaponObj(hand) != null );
        return hasWeapon;
    }

    // -----------------------------------------------------------
    // @Summary: checks if the player has a weapon in the 
    //   requested hand. 
    // @Param: hand - hand to check.
    // @Return: bool - returns true if hand has a weapon. 
    // -----------------------------------------------------------
    public bool HasWeaponInHand( VRHand hand )
    {
        bool hasWeapon = GetWeaponObj(hand) != null;
        return hasWeapon;
    }

    // -----------------------------------------------------------
    // @Summary: sells one of players weapons.
    // @Param: hand - sells this HANDs weapon.
    // @Return: void.
    // -----------------------------------------------------------
    public void SellUpgrade( VRHand hand )
    {
        AddCash((uint)( GetWeaponScript<BaseWeapon>(hand).mCost * SELL_FACTOR ));
        var newWeapon = GetWeaponScript<BaseWeapon>(hand).Sell();
        string id = newWeapon.GetComponent<BaseWeapon>().mId;
        ChangeWeaponByID(hand, id);
        ShowOrHideControllers(hand);
    }

    // -----------------------------------------------------------
    // @Summary: sells one of players weapons.
    // @Param: hand - sells this HANDs weapon.
    // @Return: void.
    // -----------------------------------------------------------
    public void SellWeapon( VRHand hand, uint sellPrice )
    {
        AddCash(sellPrice);
        GetWeaponScript<BaseWeapon>(hand).Sell();
        ShowOrHideControllers(hand);
    }

    // -----------------------------------------------------------
    // @Summary: buys the specified base gun. 
    // @Param: hand - sets this HANDs weapon.
    // @Param: weapon - prefab of weapon to buy.
    // @Return: bool - true or false if bought.
    // -----------------------------------------------------------
    public bool BuyBase(VRHand hand, GameObject weapon)
    {
        uint upgradeCost = weapon.GetComponent<BaseWeapon>().mCost;
        string id = weapon.GetComponent<BaseWeapon>().mId;

        if (mCash >= upgradeCost)
        {
            ChangeWeaponByID(hand, id);
            RemoveCash(upgradeCost);
        }
        return mCash >= upgradeCost;
    }

    // -----------------------------------------------------------
    // @Summary: buys the top path upgrade. 
    // @Param: hand - upgrades this HANDs weapon.
    // @Return: void
    // -----------------------------------------------------------
    public bool BuyUpgradeTop( VRHand hand )
    {
        uint upgradeCost = GetWeaponScript<BaseWeapon>(hand).GetUpgradeTop().GetComponent<BaseWeapon>().mCost;
        string id = GetWeaponScript<BaseWeapon>(hand).GetUpgradeTop().GetComponent<BaseWeapon>().mId;

        if (mCash >= upgradeCost)
        {
            ChangeWeaponByID(hand, id);
            RemoveCash(upgradeCost);
        }
        return mCash >= upgradeCost;
    }

    // -----------------------------------------------------------
    // @Summary: buys the bottom path upgrade. 
    // @Param: hand - upgrades this HANDs weapon.
    // @Return: void
    // -----------------------------------------------------------
    public bool BuyUpgradeBottom( VRHand hand )
    {
        uint upgradeCost = GetWeaponScript<BaseWeapon>(hand).GetUpgradeBottom().GetComponent<BaseWeapon>().mCost;
        string id = GetWeaponScript<BaseWeapon>(hand).GetUpgradeBottom().GetComponent<BaseWeapon>().mId;

        if (mCash >= upgradeCost)
        {
            ChangeWeaponByID(hand, id);
            RemoveCash(upgradeCost);
        }
        return mCash >= upgradeCost;
    }

    // -----------------------------------------------------------
    // @Summary: Adds an enhancment. Need to the specifiy the 
    //   cost because the cost is saved in the UI. 
    // @Param: hand - the hand to add enhancement to.
    // @Param: cost - cost of enhancement.
    // @Param: category - has to be between 0 to 2. 
    // @Return: bool - returns true if purchase is successful.
    // -----------------------------------------------------------
    public bool AddEnhancement( VRHand hand, uint cost, uint category )
    {
        bool purchaseSuccess = false;

        if ( mCash >= cost )
        {
            purchaseSuccess = mEnhancements.Add(hand, category);

            if ( purchaseSuccess )
            {
                RemoveCash(cost);
            }
        }

        return purchaseSuccess;
    }

    // -----------------------------------------------------------
    // @Summary: Sells an enhancment. Need to the specifiy the 
    //   cost because the cost is saved in the UI. 
    // @Param: hand - the hand to add enhancement to.
    // @Param: cost - cost of enhancement.
    // @Param: category - has to be between 0 to 2. 
    // @Return: bool - returns true if sell is successful.
    // -----------------------------------------------------------
    public bool SellEnhancement( VRHand hand, uint cost, uint category )
    {
        bool sellSuccessful = mEnhancements.Sell(hand, category);

        if ( sellSuccessful )
        {
            AddCash((uint)( cost * SELL_FACTOR ));
        }

        return sellSuccessful;
    }

    // -----------------------------------------------------------
    // @Summary: Retrieves the current enhancement level owned.
    // @Param: hand - the hand to check.
    // @Param: category - has to be between 0 to 2. 
    // @Return: uint - returns the level of enhancement owned for
    //    specified hand and category.
    // -----------------------------------------------------------
    public uint GetCurrentEnhancmentLevel( VRHand hand, uint category )
    {
        return mEnhancements.Get(hand, category);
    }

    // -----------------------------------------------------------
    // @Summary: Checks if an enhancement is owned.
    // @Param: hand - the hand to check.
    // @Param: category - has to be between 0 to 2. 
    // @Param: level - has to be between 0 to 2. 
    // @Return: bool - returns true if enhancement is owned.
    // -----------------------------------------------------------
    public bool HasEnhancement( VRHand hand, uint category, uint level )
    {
        return mEnhancements.Check(hand, category, level);
    }

    // -----------------------------------------------------------
    // @Summary: getter for the player's current weapon.
    // @Param: hand - lets you select right or left weapon.
    // @Return: returns GameObject of gun with BaseWeapon script.
    // -----------------------------------------------------------
    public GameObject GetWeaponObj( VRHand hand )
    {
        return mWeapon[hand];
    }

    // -----------------------------------------------------------
    // @Summary: getter for the player's current weapon. 
    // @Param: hand - lets you select right or left weapon.
    // @Return: returns the SCRIPT of the weapon (BaseWeapon). 
    // -----------------------------------------------------------
    public T GetWeaponScript<T>( VRHand hand ) where T : BaseWeapon
    {
        T w = null;
        if (hand == VRHand.ANY)
        {
            w = GetWeaponScript<T>(VRHand.RIGHT);
            if (!w)
            {
                w = GetWeaponScript<T>(VRHand.LEFT);
            }
        }
        else
        {
            if (GetWeaponObj(hand))
            {
                w = GetWeaponObj(hand).GetComponent<T>();
            }
        }

        return w;
    }

    // -----------------------------------------------------------
    // @Summary: getter for the player's current cash amount. 
    // @Return: returns the players cash as uint.
    // -----------------------------------------------------------
    public uint GetCash()
    {
        return mCash;
    }

    // -----------------------------------------------------------
    // @Summary: sets the origin of the laser pointer.
    // @Param: hand - hand to change ray of. 
    // @Param: transform - transform reference to use as origin. 
    // @Return: void
    // -----------------------------------------------------------
    private void SetRayOrigin(VRHand hand, Transform transform)
    {
        if (transform != null)
        {
            mRay[hand].rayOriginTransform = transform;
        }
    }

    // -----------------------------------------------------------
    // @Summary: sets player weapon. 
    // @Param: hand - hand to attach the gun to. 
    // @Param: weapon - gameObject of weapon prefab. 
    // @Return: void
    // -----------------------------------------------------------
    private void InternalSetWeapon(VRHand hand, GameObject weaponPrefab)
    {
        if (mWeapon[hand]) Destroy(mWeapon[hand]);

        if (weaponPrefab != null)
        {
            var newWeapon = Instantiate(weaponPrefab, mWeaponAnchor[hand]);
            var baseWeaponScript = newWeapon.GetComponent<BaseWeapon>();
            baseWeaponScript.SetHand(hand);
            baseWeaponScript.Disable();
            SetRayOrigin(hand, baseWeaponScript.mLaserEmitter.transform);
            mXrController[hand].model = newWeapon.transform;
            mWeapon[hand] = newWeapon;
        }
        ShowOrHideControllers(hand);
    }

    // -----------------------------------------------------------
    // @Summary: saves the ID to player class and calls internal
    //   setWeapon to instantiate the weapon. 
    // @Param: hand - hand to attach the gun to.
    // @Param: weaponId - weapon ID such as L11, H32, M44 etc. 
    // @Return: void
    // -----------------------------------------------------------
    public void ChangeWeaponByID(VRHand hand, string weaponId)
    {
        if (hand == VRHand.ANY || hand == VRHand.UNKNOWN) return;
        if (hand == VRHand.LEFT) mSaveableLWeapon.Assign(weaponId);
        if (hand == VRHand.RIGHT) mSaveableRWeapon.Assign(weaponId);

        var weapon = mWeaponPrefabMap.GetPrefabFromWeaponId(weaponId);
        InternalSetWeapon(hand, weapon);
    }

    // -----------------------------------------------------------
    // @Summary: sets cash to player.
    // @Param: cash - amount of cash to set.
    // @Return: void
    // -----------------------------------------------------------
    public void SetCash( uint cash )
    {
        mCash.Assign(cash);
    }

    // -----------------------------------------------------------
    // @Summary: adds cash to player.
    // @Param: cash - amount of cash to add. 
    // @Return: void
    // -----------------------------------------------------------
    public void AddCash( uint cash )
    {
        mCash.Assign(mCash + cash);
    }

    // -----------------------------------------------------------
    // @Summary: removes amount of cash from player. 
    // @Param: cash - amount of cash to remove. 
    // @Return: void
    // -----------------------------------------------------------
    public void RemoveCash( uint cash )
    {
        mCash.Assign(mCash - cash);
    }

    // -----------------------------------------------------------
    // @Summary: moves player and bounds to the position of the 
    //   player prefab. Called when game is initalized or reset.
    // @Return: void
    // -----------------------------------------------------------
    public void ResetPlayerPosition()
    {
        gameObject.transform.position = mPlayerOrigin;
    }

    // -----------------------------------------------------------
    // @Summary: moves player and bounds to new position. Only 
    //   moves the player in the XZ plane.
    // @Param: newPosition - vector3 representing new player pos. 
    // @Return: void
    // -----------------------------------------------------------
    public void Teleport( Vector3 newPosition )
    {
        Vector3 newXZPos = new Vector3(newPosition.x, gameObject.transform.position.y, newPosition.z);
        gameObject.transform.position = newXZPos;
    }

    // -----------------------------------------------------------
    // @Summary: puts us into teleport state. Enables the script.
    // @Return: void
    // -----------------------------------------------------------
    public void TeleportMode()
    {
        RePositionPlayer repositionScript = GetComponent<RePositionPlayer>();
        repositionScript.enabled = true;
    }

    // -----------------------------------------------------------
    // @Summary: resets the controller calibration to the last
    //   saved settings.
    // @Return: void
    // -----------------------------------------------------------
    public void ResetControllerCalibration()
    {
        // Save the new controller offsets
        var settings = Settings.Instance();
        settings.SetControllerOffset(VRHand.LEFT, new Vector3(0f, 0f, 0f));
        settings.SetControllerRotation(VRHand.LEFT, new Vector3(0f, 0f, 0f));
        settings.SetControllerOffset(VRHand.RIGHT, new Vector3(0f, 0f, 0f));
        settings.SetControllerRotation(VRHand.RIGHT, new Vector3(0f, 0f, 0f));

        mWeaponAnchor[VRHand.LEFT].transform.localPosition = new Vector3(0f, 0f, 0f);
        mWeaponAnchor[VRHand.LEFT].transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        mWeaponAnchor[VRHand.RIGHT].transform.localPosition = new Vector3(0f, 0f, 0f);
        mWeaponAnchor[VRHand.RIGHT].transform.localRotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
    }

    // -----------------------------------------------------------
    // @Summary: enables controller calibration and shows the 
    //   controller model. 
    // @Return: void
    // -----------------------------------------------------------
    public void StartControllerCalibration()
    {
        ControllerProxy.Instance().RegisterCallbackTriggerDown(VRHand.LEFT, HandleLFireBtnDown);
        ControllerProxy.Instance().RegisterCallbackTriggerUp(VRHand.LEFT, HandleLFireBtnUp);
        ControllerProxy.Instance().RegisterCallbackTriggerDown(VRHand.RIGHT, HandleRFireBtnDown);
        ControllerProxy.Instance().RegisterCallbackTriggerUp(VRHand.RIGHT, HandleRFireBtnUp);

        // Show the controller model
        mControllerInstance[VRHand.LEFT] = Instantiate(mControllerPrefabL, mHandAnchor[VRHand.LEFT]);
        mControllerInstance[VRHand.RIGHT] = Instantiate(mControllerPrefabR, mHandAnchor[VRHand.RIGHT]);
    }

    // -----------------------------------------------------------
    // @Summary: Cancels controller calibration and sets the 
    //   controller offsets back to previous saved settings.
    // @Return: void
    // -----------------------------------------------------------
    public void CancelControllerCalibration()
    {
        ControllerProxy.Instance().RemoveCallbackTriggerDown(VRHand.LEFT, HandleLFireBtnDown);
        ControllerProxy.Instance().RemoveCallbackTriggerUp(VRHand.LEFT, HandleLFireBtnUp);
        ControllerProxy.Instance().RemoveCallbackTriggerDown(VRHand.RIGHT, HandleRFireBtnDown);
        ControllerProxy.Instance().RemoveCallbackTriggerUp(VRHand.RIGHT, HandleRFireBtnUp);

        // Delete the controller models
        ShowOrHideControllers(VRHand.RIGHT);
        ShowOrHideControllers(VRHand.LEFT);

        var settings = Settings.Instance();
        mWeaponAnchor[VRHand.LEFT].localPosition = settings.GetControllerOffset(VRHand.LEFT);
        mWeaponAnchor[VRHand.LEFT].localRotation = Quaternion.Euler(settings.GetControllerRotation(VRHand.LEFT));
        mWeaponAnchor[VRHand.RIGHT].localPosition = settings.GetControllerOffset(VRHand.RIGHT);
        mWeaponAnchor[VRHand.RIGHT].localRotation = Quaternion.Euler(settings.GetControllerRotation(VRHand.RIGHT));
    }

    // -----------------------------------------------------------
    // @Summary: Ends the controller calibration state by 
    //   removing the controller models and saving the offsets in
    //   persistent storage. 
    // @Return: void
    // -----------------------------------------------------------
    public void EndControllerCalibration()
    {
        ControllerProxy.Instance().RemoveCallbackTriggerDown(VRHand.LEFT, HandleLFireBtnDown);
        ControllerProxy.Instance().RemoveCallbackTriggerUp(VRHand.LEFT, HandleLFireBtnUp);
        ControllerProxy.Instance().RemoveCallbackTriggerDown(VRHand.RIGHT, HandleRFireBtnDown);
        ControllerProxy.Instance().RemoveCallbackTriggerUp(VRHand.RIGHT, HandleRFireBtnUp);

        // Delete the controller models
        ShowOrHideControllers(VRHand.RIGHT);
        ShowOrHideControllers(VRHand.LEFT);

        // Save the new controller offsets
        var settings = Settings.Instance();
        settings.SetControllerOffset(VRHand.LEFT, mWeaponAnchor[VRHand.LEFT].localPosition);
        settings.SetControllerRotation(VRHand.LEFT, mWeaponAnchor[VRHand.LEFT].localRotation.eulerAngles);
        settings.SetControllerOffset(VRHand.RIGHT, mWeaponAnchor[VRHand.RIGHT].localPosition);
        settings.SetControllerRotation(VRHand.RIGHT, mWeaponAnchor[VRHand.RIGHT].localRotation.eulerAngles);
    }

    // -----------------------------------------------------------
    // @Summary: Shows hand model if no weapon is equipped, 
    //   otherwise hide. 
    // @Param: hand - hand to show/hide
    // @Return: void
    // -----------------------------------------------------------
    public void ShowOrHideControllers( VRHand hand )
    {
        if (mWeapon[hand] != null && mControllerInstance[hand] != null)
        {
            Destroy(mControllerInstance[hand]);
            mControllerInstance[hand] = null;
        }
        else if (mWeapon[hand] == null)
        {
            if ((mControllerInstance[hand] == null) && mControllerInstance[hand] == null)
            {
                Debug.Log("ShowOrHideHandModel: Instantiating " + hand.ToString() + "Model");
                mControllerInstance[hand] = Instantiate(mControllerPrefab[hand], mHandAnchor[hand]);
                SetRayOrigin(hand, mControllerInstance[hand].transform);
            }
        }
    }

    public bool LaserArbitrationTable(VRHand hand)
    {
        GameState s = GameState.GetInstance();
        var baseLight = GetWeaponScript<BaseLight>(hand);

        // If weapon has a laser, and round is in progress, disable laser
        bool disableDueToWeaponLaser =
               baseLight
            && baseLight.HasWeaponLaser()
            && (s.GetState() == State.ROUND_IN_PROGRESS);

        // If round is in progress, and its not easy mode, disable laser
        bool disableDueToGameState = 
               (s.GetState() == State.ROUND_IN_PROGRESS)
            && (GameState.GetDifficulty() != Difficulty.EASY);

        // If conditions are not met, enable laser
        return !disableDueToWeaponLaser && !disableDueToGameState;
    }

    // -----------------------------------------------------------
    // @Summary:  
    // @Param: e - event callback context.
    // @Return: void
    // -----------------------------------------------------------
    void HandleRFireBtnDown(InputAction.CallbackContext e)
    {
        if (mWeapon[VRHand.RIGHT] != null)
        {
            mWeaponAnchor[VRHand.RIGHT].parent = null; 
        }
    }

    // -----------------------------------------------------------
    // @Summary:  
    // @Param: e - event callback context.
    // @Return: void
    // -----------------------------------------------------------
    void HandleRFireBtnUp(InputAction.CallbackContext e)
    {
        mWeaponAnchor[VRHand.RIGHT].parent = mHandAnchor[VRHand.RIGHT];
    }

    // -----------------------------------------------------------
    // @Summary:  
    // @Param: e - event callback context.
    // @Return: void
    // -----------------------------------------------------------
    void HandleLFireBtnDown(InputAction.CallbackContext e)
    {
        // Only enable calibration if there is a weapon in the left hand
        if (mWeapon[VRHand.LEFT] != null)
        {
            mWeaponAnchor[VRHand.LEFT].parent = null;
        }
    }

    // -----------------------------------------------------------
    // @Summary:  
    // @Param: e - event callback context.
    // @Return: void
    // -----------------------------------------------------------
    void HandleLFireBtnUp(InputAction.CallbackContext e)
    {
        mWeaponAnchor[VRHand.LEFT].parent = mHandAnchor[VRHand.LEFT];
    }

    // -----------------------------------------------------------
    // @Summary: Called when controller goes online. Enables the
    //   weapon that hand has one. 
    // @Param: e - event callback data.
    // @Return: void
    // -----------------------------------------------------------
    void HandleControllerOnline( EventControllerOnline e )
    {
        if(mWeapon[e.hand])
        {
            mWeapon[e.hand].SetActive(true);
        }
    }

    // -----------------------------------------------------------
    // @Summary: Called when controller goes offline. Disables the
    //   weapon that hand has one. 
    // @Param: e - event callback data
    // @Return: void
    // -----------------------------------------------------------
    void HandleControllerOffline( EventControllerOffline e )
    {
        if (mWeapon[e.hand])
        {
            mWeapon[e.hand].SetActive(false);
        }
    }

    // -----------------------------------------------------------
    // Override the Awake() method. 
    // -----------------------------------------------------------
    private void Awake()
    {
        if ( mInstance == null )
        {
            mInstance = this;
        }

        mRay[VRHand.LEFT] = mRayR;
        mRay[VRHand.RIGHT] = mRayL;

        mXrController[VRHand.LEFT] = mControllerL;
        mXrController[VRHand.RIGHT] = mControllerR;

        mControllerPrefab[VRHand.LEFT] = mControllerPrefabL;
        mControllerPrefab[VRHand.RIGHT] = mControllerPrefabR;

        mWeaponAnchor[VRHand.LEFT] = mXrController[VRHand.LEFT].modelParent;
        mWeaponAnchor[VRHand.RIGHT] = mXrController[VRHand.RIGHT].modelParent;

        mHandAnchor[VRHand.LEFT] = mWeaponAnchor[VRHand.LEFT].parent;
        mHandAnchor[VRHand.RIGHT] = mWeaponAnchor[VRHand.RIGHT].parent;

        mPlayerOrigin = gameObject.transform.position;

        if (!mMainMenuMode)
        {
            // Create instance of saveload to interface with the save system
            SaveLoadInterface saveInterface = new SaveLoadInterface();

            // Register saveable objects to the save manager. These will be saved at the end of the round.
            mSaveableEnhancements = new PersistentDataType<EnhancementManager>(mEnhancements, "enhancements");
            saveInterface.RegisterSaveableObject(mSaveableEnhancements);

            mSaveableLWeapon = new PersistentDataType<string>("", "leftgun");
            mSaveableLWeapon.RegisterOnLoadCallback(() => ChangeWeaponByID(VRHand.LEFT, mSaveableLWeapon));
            saveInterface.RegisterSaveableObject(mSaveableLWeapon);

            mSaveableRWeapon = new PersistentDataType<string>("", "rightgun");
            mSaveableRWeapon.RegisterOnLoadCallback(() => ChangeWeaponByID(VRHand.RIGHT, mSaveableRWeapon));
            saveInterface.RegisterSaveableObject(mSaveableRWeapon);

            mSaveableOrigin = new PersistentDataType<SerializableVector3>(mPlayerOrigin, "position");
            saveInterface.RegisterSaveableObject(mSaveableOrigin);

            mCash = new PersistentDataType<uint>(450, "cash");
            saveInterface.RegisterSaveableObject(mCash);
        }
    }

    // -----------------------------------------------------------
    // Override the Start() method.
    // -----------------------------------------------------------
    private void Start()
    {
        var settings = Settings.Instance();
        mWeaponAnchor[VRHand.LEFT].localPosition = settings.GetControllerOffset(VRHand.LEFT);
        mWeaponAnchor[VRHand.LEFT].localRotation = Quaternion.Euler(settings.GetControllerRotation(VRHand.LEFT));
        mWeaponAnchor[VRHand.RIGHT].localPosition = settings.GetControllerOffset(VRHand.RIGHT);
        mWeaponAnchor[VRHand.RIGHT].localRotation = Quaternion.Euler(settings.GetControllerRotation(VRHand.RIGHT));

        ShowOrHideControllers(VRHand.LEFT);
        ShowOrHideControllers(VRHand.RIGHT);
    }

    // -----------------------------------------------------------
    // Override the Update() method. 
    // -----------------------------------------------------------
    private void Update()
    {
        if (!mMainMenuMode)
        {
            mRayVisualL.enabled = LaserArbitrationTable(VRHand.LEFT);
            mRayVisualR.enabled = LaserArbitrationTable(VRHand.RIGHT);
        }
    }

    // -----------------------------------------------------------
    // Override the OnEnable() function.
    // -----------------------------------------------------------
    private void OnEnable()
    {
        Events.Instance.AddListener<EventControllerOnline>(HandleControllerOnline);
        Events.Instance.AddListener<EventControllerOffline>(HandleControllerOffline);
    }

    // -----------------------------------------------------------
    // Override the OnDisable() function.
    // -----------------------------------------------------------
    private void OnDisable()
    {
        Events.Instance.RemoveListener<EventControllerOnline>(HandleControllerOnline);
        Events.Instance.RemoveListener<EventControllerOffline>(HandleControllerOffline);
    }
}
