using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Start a game
    public void BeginGame()
    {
        Application.LoadLevel("Survival Game");
    }

    public void BeginMulti()
    {
        Application.LoadLevel("Multiplayer");
    }

    // Quit the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
