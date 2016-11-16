using UnityEngine;
using System.Collections;

//Credit goes to Byte56 in http://gamedev.stackexchange.com/questions/123938/unity-i-want-to-make-my-ui-text-fade-in-after-5-seconds for this code.
public class UITextFade : MonoBehaviour
{

    public float changeTimeSeconds = 1;
    public float startAlpha = 0;
    public float endAlpha = 1;

    float changeRate = 0;
    float timeSoFar = 0;
    bool fading = false;
    public bool faded = false;
    CanvasGroup canvasGroup;


    void Start ()
    {
        canvasGroup = this.GetComponent<CanvasGroup> ();
        if (canvasGroup == null)
        {
            Debug.Log ( "Must have canvas group attached!" );
            this.enabled = false;
        }
    }

    public void FadeIn ()
    {
        startAlpha = 0;
        endAlpha = 1;
        timeSoFar = 0;
        fading = true;
        faded = true;
        StartCoroutine ( FadeCoroutine () );
    }

    public void FadeOut ()
    {
        startAlpha = 1;
        endAlpha = 0;
        timeSoFar = 0;
        fading = true;
        faded = true;
        StartCoroutine ( FadeCoroutine () );
    }

    IEnumerator FadeCoroutine ()
    {
        changeRate = (endAlpha - startAlpha) / changeTimeSeconds;
        SetAlpha ( startAlpha );
        while (fading)
        {
            timeSoFar += PauseManager.deltaTime;

            if (timeSoFar > changeTimeSeconds)
            {
                fading = false;
                SetAlpha ( endAlpha );
                yield break;
            }
            else
            {
                SetAlpha ( canvasGroup.alpha + (changeRate * PauseManager.deltaTime) );
            }

            yield return null;
        }
    }



    public void SetAlpha ( float alpha )
    {
        canvasGroup.alpha = Mathf.Clamp ( alpha, 0, 1 );
    }
}