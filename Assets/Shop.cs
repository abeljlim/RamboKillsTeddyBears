﻿using UnityEngine;
using System.Collections;

public class Shop : MonoBehaviour {

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // Exit the shop
    public void ExitShop()
    {
        int previousWave = PlayerPrefs.GetInt("previousWave");
        Application.LoadLevel(previousWave);
    }

}