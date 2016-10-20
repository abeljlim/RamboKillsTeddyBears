using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class Timer : MonoBehaviour {

    public Text timerText;
    public static float elapsedTime;
    TimeSpan t;

	// Use this for initialization
	void Start () {
        timerText = GetComponent<Text> ();
	}
	
	// Update is called once per frame
	void Update () {
        elapsedTime += Time.deltaTime;
        t = TimeSpan.FromSeconds(elapsedTime);
        if (t.Hours < 1)
        {
            timerText.text = string.Format ( "{0}:{1:D2}",
                    t.Minutes,
                    t.Seconds );

        }
        else
        {
            timerText.text = string.Format ( "{0}:{1:D2}:{2:D2}",
                    t.Hours,
                    t.Minutes,
                    t.Seconds);
        }
	}
}
