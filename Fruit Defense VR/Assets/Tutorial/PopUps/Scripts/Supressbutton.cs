// ---------------------------------------------------------------
// Copyright (C) 2019-2022 TwinTip Studios - All Rights Reserved
// This software and all related information is the intellectual
// property of TwinTip Studios and may not be distributed,
// replicated or disclosed without the explicit prior written
// permission of TwinTip Studios. 
// ---------------------------------------------------------------


using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class SupressButton : MonoBehaviour
{
    private GameObject mCover;

    // -----------------------------------------------------------
    // @Summary: Search for any animators, buttons, images, and
    //   text and supress them.
    // @Return: void
    // -----------------------------------------------------------
    private void OnEnable()
    {
        Debug.Log("Supressing " + this.name);
        GetComponent<Button>().enabled = false;

        mCover = CreateBlackoutOverlay(this.gameObject);

        var img = mCover.GetComponent<Image>();
        if (img != null)
        {
            img.color = new Color32(0, 0, 0, 200);
        }

        var text = GetComponent<TextMeshProUGUI>();
        if (text != null)
        {
            text.color = new Color32(0, 0, 0, 200);
        }
    }

    // -----------------------------------------------------------
    // @Summary: Implement OnDestroy method. Search for any
    //   animators, buttons, images, and text and restore them.
    // @Return: void
    // -----------------------------------------------------------
    public void OnDisable()
    {
        Debug.Log("UnSupressing " + this.name);
        GetComponent<Button>().enabled = true;
        Destroy(mCover);
    }

    GameObject CreateBlackoutOverlay(GameObject org)
    {
        var copy = new GameObject();
        Debug.Log("Creating copy of: " + org.transform.name);
        CopyComponent<RectTransform>(org, copy);
        CopyComponent<CanvasRenderer>(org, copy);
        CopyComponent<Image>(org, copy);
        CopyComponent<TextMeshProUGUI>(org, copy);
        copy.transform.parent = org.transform;

        // Set on top (last sibling) and orientation aligned
        copy.transform.SetAsLastSibling();
        copy.transform.localPosition = new Vector3(0, 0, 0);
        copy.transform.localScale = new Vector3(1, 1, 1);
        copy.transform.localRotation = Quaternion.Euler(0, 0, 0);

        // Dont copy all the children, just the base image
        foreach (Transform child in copy.transform)
        {
            Destroy(child.gameObject);
        }
        return copy;
    }

    T CopyComponent<T>(GameObject original, GameObject destination) where T : Component
    {
        T ret = null;
        T component = original.GetComponent<T>();
        if (component != null)
        {
            System.Type type = component.GetType();
            var dst = destination.GetComponent(type) as T;
            if (!dst) dst = destination.AddComponent(type) as T;
            var fields = type.GetFields();
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(dst, field.GetValue(original));
            }
            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
                prop.SetValue(dst, prop.GetValue(component, null), null);
            }
            ret = dst;
        }
        
        return ret;
    }
}
