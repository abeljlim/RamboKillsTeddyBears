using UnityEngine;
using System.Collections;

public class RestartPlayerData : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //delete all existing PlayerPrefs data
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("currentLevel", 5); //setting current level to 5 for testing
        PlayerPrefs.SetInt("score", 100000);
        EnemySpawner.spawning = true; //for enemies in demo
    }

    // Update is called once per frame
    void Update ()
    {
    }
}
