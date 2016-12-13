using UnityEngine;
using System.Collections;

public class RestartPlayerData : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //delete all existing PlayerPrefs data
        PlayerPrefs.DeleteAll();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
