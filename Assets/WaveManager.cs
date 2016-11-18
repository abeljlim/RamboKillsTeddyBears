using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour {

    //access level with EnemySpawner.level;
    public static int level;
    public GameObject[] SpawnPoint; //Not used
    public string[] levelName; //to correspond to the enemies spawned in the enemy gameobject in EnemySpawner
    public static float waveTime; //current wave time
    public int[] levelTime; //full time for the wave to finish
    public int waveStartTime = 5;
    public static bool isDay = true;

	// Use this for initialization
	void Start () {
        level = 1;
        waveTime = 0;
        EnemySpawner.spawning = true;
        PlanetOrbit.SecondsInDay = levelTime[level - 1]; //update time of the day with the wave time.
	}
	
	// Update is called once per frame
	void Update () {
        waveTime += Time.deltaTime;
        //advance to next level when time of wave elapses, as long as more levels exist
        if (waveTime >= levelTime[level-1] && level < levelName.Length)
        {
            //Time.timeScale = 0;

            LevelNameText.text = levelName[level]; //level-1 is the 0-based level
            EnemySpawner.spawning = false; //pause for the period of waveStartTime
            level++;
            PlanetOrbit.SecondsInDay = levelTime[level - 1];
            waveTime = 0;

            PlayerPrefs.SetInt("previousWave", Application.loadedLevel);

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
