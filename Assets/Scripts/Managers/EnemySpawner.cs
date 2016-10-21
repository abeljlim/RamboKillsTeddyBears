using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

    public GameObject[] enemy;
    public static bool spawning;
    public static int level {
        get {return WaveManager.level;}
    } //what level, or wave, this would be, which determines which enemy is to spawn.
    public static int spawnInterval; //'global' interval shared across all spawners; can separate them as well.

	// Use this for initialization
	void Start () {
        // Repeatedly call Spawn at intervals of spawnInterval.
        spawnInterval = 3;
        InvokeRepeating ( "Spawn", 0, spawnInterval );
	}

    void Spawn ()
    {
        if(spawning)
            Instantiate ( enemy[level-1], transform.position, transform.rotation );
    }

	// Update is called once per frame
	void Update () {
	
	}
}
