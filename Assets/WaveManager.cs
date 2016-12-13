﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour {

    //access level with EnemySpawner.level;
    public static int level = 1;
    public GameObject[] SpawnPoint; //Not used
    public string[] levelName; //to correspond to the enemies spawned in the enemy gameobject in EnemySpawner
    public static float waveTime; //current wave time
    public int[] levelTime; //full time for the wave to finish
    public int waveStartTime = 5;
    public static bool isDay = true;

	// Use this for initialization
    // start before any of the SpawnPoint code runs
    void Awake ()
    {

        level = 1;
        if (PlayerPrefs.HasKey("currentLevel")) //new level
        {
            level = PlayerPrefs.GetInt("currentLevel");
            LevelNameText.text = levelName[level - 1]; //level-1 is the 0-based level
            //Debug.Log("Level: " + level);
        }
        waveTime = 0;
        EnemySpawner.spawning = true;
        if(EnemyHealth.initColors == null)
        {
            //Debug.Log(EnemyHealth.initColors);
            //instantiate initColors
            EnemyHealth.initColors = new List<Color[]>(10); //with 4 colors at the moment
            for (int i = 0; i < 10; i++)
                EnemyHealth.initColors.Add(null);
        }
    }

	void Start () {
        PlanetOrbit.SecondsInDay = levelTime[level - 1]; //update time of the day with the wave time.
	}
	
	// Update is called once per frame
	void Update () {
        waveTime += Time.deltaTime;

        //advance to next level when time of wave elapses, as long as more levels exist
        //or, advance when a boss is killed
        if (EnemyHealth.BossKilled || waveTime >= levelTime[level-1] && level < levelName.Length)
        {
            //Time.timeScale = 0;
            EnemyHealth.BossKilled = false;

            LevelNameText.text = levelName[level]; //level-1 is the 0-based level
            EnemySpawner.spawning = false; //pause for the period of waveStartTime

            level++;
            PlanetOrbit.SecondsInDay = levelTime[level - 1];
            waveTime = 0;

            //call the PerLevelUpdate method in each SpawnPoint in the children of WaveManager
            foreach (Transform child in transform)
            {
                child.gameObject.GetComponent<EnemySpawner>().PerLevelUpdate();
            }

            PlayerPrefs.SetInt("previousScene", Application.loadedLevel);
            PlayerPrefs.SetInt("currentLevel", level);

            PlayerPrefs.SetInt("score", MoneyManager.money);
            Application.LoadLevel("Shop");
        }

        //toggle the state between night and day
        if ((waveTime >= 0) && (waveTime < (levelTime[level - 1] / 2)))
            isDay = true;
        else
            isDay = false;

        //start spawning of the current wave after waveStartTime
        if (waveTime >= waveStartTime && !EnemySpawner.spawning)
        {
            EnemySpawner.spawning = true;
        }
	}
}
