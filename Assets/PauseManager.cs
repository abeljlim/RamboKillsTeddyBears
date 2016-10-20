using UnityEngine;
using System.Collections;

public class PauseManager : MonoBehaviour {

    public static float deltaTime; //delta time that persists in increasing, even while paused
    public static float LastrealtimeSinceStartup; //for the prev. Update

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        deltaTime = Time.realtimeSinceStartup - LastrealtimeSinceStartup;
        LastrealtimeSinceStartup = Time.realtimeSinceStartup;
	}

    /// <summary>
    /// Pauses by setting timeScale to 0
    /// </summary>
    public static void Pause ()
    {
        Time.timeScale = 0;
    }

    /// <summary>
    /// Unpauses by setting timeScale to 1
    /// </summary>
    public static void Unpause ()
    {
        Time.timeScale = 1;
    }
}
