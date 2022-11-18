// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------

using UnityEngine;

public class Calibrate : MonoBehaviour
{
    public void TriggerCalibration()
    {
        Player.GetInstance().StartControllerCalibration();
    }

    public void EndCalibration()
    {
        Player.GetInstance().EndControllerCalibration();
    }

    public void CancelCalibration()
    {
        Player.GetInstance().CancelControllerCalibration();
    }

    public void ResetCalibration()
    {
        Player.GetInstance().ResetControllerCalibration();
    }
}
