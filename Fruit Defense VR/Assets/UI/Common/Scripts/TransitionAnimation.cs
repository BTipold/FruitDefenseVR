using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionAnimation : MonoBehaviour
{
    private float mTime = 0.2f;

    IEnumerator ScaleOverTime(Vector3 initialScale, Vector3 finalScale, float duration, float delayTime, bool isActive, bool toDestroy = false)
    {
        yield return new WaitForSecondsRealtime(delayTime);
        for (float time = 0; time < duration; time += Time.unscaledDeltaTime)
        {
            float progress = 1 - ((duration - time) / duration);
            transform.localScale = Vector3.Lerp(initialScale, finalScale, Mathf.SmoothStep(0.0f, 1.0f, progress));
            yield return null;
        }
        gameObject.SetActive(isActive);

        if (toDestroy) 
            Destroy(this.gameObject);
    }

    public void ScaleIn(float delayTime = 0.5f)
    {
        transform.localScale = new Vector3(0, 0, 0);
        StartCoroutine(ScaleOverTime(new Vector3(0, 0, 0), new Vector3(1, 1, 1), mTime, mTime, true));
    }
    public void ScaleOut(float delayTime = 0f)
    {
        StartCoroutine(ScaleOverTime(new Vector3(1, 1, 1), new Vector3(0, 0, 0), mTime, delayTime, false));
    }

    public void ScaleOutMainMenu(float delayTime = 0f)
    {
        StartCoroutine(ScaleOverTime(new Vector3(1, 1, 1), new Vector3(0, 0, 0), mTime, delayTime, true, true));
    }
}
