// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using UnityEngine;
using System;


[CreateAssetMenu( menuName = "EnhancementGroup" )]
[Serializable]
public class EnhancementGroup : ScriptableObject
{
    public CommonTypes.WeaponClass weaponClass;
    public Enhancement[] top;
    public Enhancement[] mid;
    public Enhancement[] bot;
}
