// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuCenter : MonoBehaviour
{
    public RectTransform mCanvasRectTransform;
    public Camera mPlayerCamera;
    private Player mPlayer;
    private float mYGap;
    private const float MoveSpeedConst = 5.67f;
    private const float RotateSpeedConst = 65f;
    private const float radius = 5f;
    private float speedMultpilier;

    enum MenuState { InScreen, OutScreen, Moving };
    private MenuState mCurrentMenuState;

    // Start is called before the first frame update
    void Start()
    {
        mPlayer = Player.GetInstance();
        //mCanvasRectTransform = GetComponentInChildren<RectTransform>();
        // mPlayerCamera = Camera.main;
        mCurrentMenuState = MenuState.InScreen;
        mYGap = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        switch (mCurrentMenuState)
        {
            case MenuState.InScreen:
                if (shouldMove(3.5f))
                {
                    speedMultpilier = 0f;
                    mCurrentMenuState = MenuState.OutScreen;
                }
                break;
            case MenuState.OutScreen:
                mCurrentMenuState = MenuState.Moving;
                StartCoroutine(MoveToCenter());
                break;
            case MenuState.Moving:
                if (Vector3.Distance(transform.position, GetCameraForward()) < 1.2)
                {
                    speedMultpilier -= 0.1f;
                }
                else if (Vector3.Distance(transform.position, GetCameraForward()) > 1.2 && speedMultpilier < 1f)
                {
                    speedMultpilier += 0.015f;
                }
                break;
        }
    }

    private IEnumerator MoveToCenter()
    {
        //while (transform.position != GetCameraForward())
        while(speedMultpilier >= 0f)
        {
            // transform.eulerAngles = mPlayerCamera.transform.eulerAngles;
            float playerRotation = mPlayer.GetPlayerPosition().eulerAngles.y;
            float moveSpeed = MoveSpeedConst * Time.unscaledDeltaTime;
            float rotationSpeed = RotateSpeedConst * Time.unscaledDeltaTime;
            if(speedMultpilier < 1f)
            {
                moveSpeed *= speedMultpilier;
                rotationSpeed *= speedMultpilier;
            }
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, playerRotation, 0), rotationSpeed);
            transform.position = Vector3.MoveTowards(transform.position, getNextPointOnCircle(), moveSpeed);
            yield return null;
        }
        mCurrentMenuState = MenuState.InScreen;
    }

    private bool shouldMove(float distance)
    {
        Vector3 lookPosition = GetCameraForward();
        // lookPosition.y = transform.position.y;
        // checks if the xz distance between where the player is looking and where the menu is

        return Vector3.Distance(transform.position, lookPosition) > distance;
    }

    private Vector3 GetCameraForward()
    {
        Vector3 forward = mPlayerCamera.transform.forward * 5; // 5m in front
        // forward.y += mYGap;
        forward += mPlayerCamera.transform.position;
        // if (Mathf.Abs(forward.y - transform.position.y) < mYGap)
           // forward.y = transform.position.y; // don't want menu going up/down

        return forward;
    }

    private float getAngle()
    {
        return 1f;
    }

    private Vector3 getNextPointOnCircle()
    {
        float newY = (transform.position.y - mPlayer.GetPlayerPosition().position.y > 0.6f) ? transform.position.y : mPlayer.GetPlayerPosition().position.y + 1.0f;
        Vector3 newPoint = new Vector3(mPlayer.GetPlayerPosition().transform.position.x + (radius * Mathf.Sin(transform.eulerAngles.y *Mathf.PI/180f)), newY, mPlayer.GetPlayerPosition().transform.position.z + (radius * Mathf.Cos(transform.eulerAngles.y * Mathf.PI/180)));

        return newPoint;
    }
}
