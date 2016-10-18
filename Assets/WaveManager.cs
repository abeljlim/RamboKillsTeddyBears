using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour {

    //access level with EnemySpawner.level;
    public static int level = 1;
    float gameTime = 0;
    float waveTime = 0;
    public GameObject[] SpawnPoint; //Not used
    public string[] levelName; //to correspond to the enemies spawned in the enemy gameobject in EnemySpawner
    public int[] levelTime;
    public int waveStartTime = 5;

	// Use this for initialization
	void Start () {
        EnemySpawner.spawning = true;
	}
	
	// Update is called once per frame
	void Update () {
        gameTime += Time.deltaTime;
        waveTime += Time.deltaTime;

        //advance to next level when time of wave elapses, as long as more levels exist
        if (waveTime >= levelTime[level-1] && level < levelName.Length)
        {
            LevelNameText.text = levelName[level]; //level-1 is the 0-based level
            EnemySpawner.spawning = false; //pause for the period of waveStartTime
            level++;
            waveTime = 0;
        }

        //start spawning of the current wave after waveStartTime
        if (waveTime >= waveStartTime && !EnemySpawner.spawning)
        {
            EnemySpawner.spawning = true;
        }
	}
}
