using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialArrow : MonoBehaviour
{
    private static Vector2 RIGHT = new Vector2(77f, 55f);
    private static Vector2 LEFT = new Vector2(-77f, 55f);
    private static Vector2 TOP = new Vector2(0f, 135f);
    private static Vector2 BOT = new Vector2(0f, -135f);

    public RectTransform mRect;
    public TextMeshProUGUI mMessage;

    public void Rotate(float angle)
    {
        mRect.localRotation = Quaternion.Euler(0f, 0f, angle);
        if ((angle >= 0 && angle < 45) || (angle >= 315 && angle < 360)) 
        {
            mMessage.transform.localPosition = RIGHT;
        }
        else if (angle >= 45 && angle < 135)
        {
            mMessage.transform.localPosition = TOP;
        }
        else if (angle >= 135 && angle < 225)
        {
            mMessage.transform.localPosition = LEFT;
        }
        else if (angle >= 225 && angle < 315)
        {
            mMessage.transform.localPosition = BOT;
        }
    }

    public void SetMessage(string message)
    {
        if (mMessage != null)
        {
            mMessage.text = message;
        } else {
            mMessage.text = "";
        }
    }

    public void SetMessagePosition(Vector2 p)
    {
        mMessage.transform.localPosition = p;
    }
}
