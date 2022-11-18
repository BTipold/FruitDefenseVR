// ---------------------------------------------------------------
// Copyright (C) 2019-2020 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using UnityEngine;
using System;


[CreateAssetMenu( menuName = "Enhancement" )]
[Serializable]
public class Enhancement : ScriptableObject
{
    public string title;
    public string description;
    public Sprite icon;
    public uint cost;
}
