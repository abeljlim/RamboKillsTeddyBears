using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FadingText : MonoBehaviour {
    CanvasGroup cg;
    public bool faded = false;
    public const float fadeTime = 1f; //for death screen reddening
    public float currfadeTime = 0f; //for death screen reddening
    public bool playWhenPaused = true;

    /*
     
        
        if (currFadeTime < fadeTime) {
         if(playWhenPaused) {
            currFadeTime += PauseManager.deltaTime;
        }
        else
        {
            currFadeTime += Time.deltaTime;
        }//second param is the time
     */
    /*
    public float textAlpha
    {
        get
        {
            return GetComponent<Text>()
        }
    }*/
	// Use this for initialization
	void Start () {
        //cr = GetComponent<CanvasRenderer> ();
        //cr.SetAlpha ( 0f );
	}
	
	// Update is called once per frame
	void Update () {

    }
}
public static class GraphicExtensions
{
    /// <summary>
    /// Fade methods forUI elements;
    /// </summary>
    /// <param name="g"></param>
    public static void FadeIn ( this Graphic g )
    {
        g.GetComponent<CanvasRenderer> ().SetAlpha ( 0f );
        g.CrossFadeAlpha ( 0f, .15f, false );
    }
    public static void FadeOut ( this Graphic g )
    {
        g.GetComponent<CanvasRenderer> ().SetAlpha ( 1f );
        g.CrossFadeAlpha ( 0f, .15f, false );
    }
}