using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowFPS : MonoBehaviour
{
    public TMPro.TextMeshProUGUI FpsText;

    float actualFpsLimit;

    // Update is called once per frame
    void Update()
    {
        FpsText.text = "FPS: " + ((int)(1f / Time.unscaledDeltaTime)).ToString() + " / " + actualFpsLimit.ToString();
    }

    private void Start()
    {
        if (!Application.isEditor)
        {
            OVRPlugin.systemDisplayFrequency = 120f;
        }

        //CoroutineManager scheduler = new CoroutineManager();
        //scheduler.DoAfter(() => { Unity.XR.Oculus.Performance.TrySetDisplayRefreshRate(90); Debug.Log("SETTING REFRESH RATE"); }, 3.0f);
    }
}
