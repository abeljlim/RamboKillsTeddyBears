using UnityEngine;
using System.Collections;

public class WaveManager : MonoBehaviour {

    //access level with EnemySpawner.level;
    public static int level = 1;
    float gameTime = 0;
    float waveTime = 0;
    public GameObject[] SpawnPoint; //Not used

	// Use this for initialization
	void Start () {
        EnemySpawner.spawning = true;
	}
	
	// Update is called once per frame
	void Update () {
        gameTime += Time.deltaTime;
        waveTime += Time.deltaTime;
        if (waveTime >= 10 && level == 1)
        {
            LevelNameText.text = "Level 2: Killing cute unicorns";
            EnemySpawner.spawning = false;
            level = 2;
            waveTime = 0;
        }
        if (waveTime >= 5 && level == 2 && !EnemySpawner.spawning)
        {
            //could make this code only run once
            EnemySpawner.spawning = true;
        }
	}
}
