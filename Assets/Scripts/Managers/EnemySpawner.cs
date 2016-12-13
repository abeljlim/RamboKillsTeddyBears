using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    public GameObject[] enemy;
    public static bool spawning; //global condition on whether to spawn
    public static int level {
        get {return WaveManager.level;}
    } //what level, or wave, this would be, which determines which enemy is to spawn.
    public int[] spawnInterval; //'global' interval shared across all spawners; can separate them as well.

	// Use this for initialization
	void Start () {
        // Repeatedly call Spawn at intervals of spawnInterval.
        if (enemy[level - 1] != null)
        {
            Debug.Log(enemy[level - 1]);
            Debug.Log("SpawnPoint started; interval: " + spawnInterval[level - 1]);
            InvokeRepeating("Spawn", 0, spawnInterval[level - 1]); //recreated each scene, so just refer to this ...
        }
    }

    void Spawn ()
    {
        //Debug.Log(level);
        //if (spawnInterval[level-1] == 2)
        //{
        //    Debug.Log("Is spawning ...");
        //}
        if(spawning && enemy[level-1] != null) //check global condition, in addition to if there would be an enemy
            Instantiate ( enemy[level-1], transform.position, transform.rotation );
    }

	// Update is called once per frame
	void Update () {
    }
    public void PerLevelUpdate()
    {
        /*Debug.Log("New Spawn for level "+level+", with interval "+spawnInterval[level-1]);
        if (enemy[level - 1] != null)
            CancelInvoke("Spawn");
        InvokeRepeating("Spawn", 0, spawnInterval[level - 1]);*/
    }
}
