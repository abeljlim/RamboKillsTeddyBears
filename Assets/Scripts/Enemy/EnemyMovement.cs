using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

    Transform player;
        
    NavMeshAgent nav;

	// Use this for initialization
	void Start () {

        player = GameObject.FindGameObjectWithTag("Player").transform;
        nav = GetComponent<NavMeshAgent>();

	}
	
	// Update is called once per frame
	void Update () {

        nav.SetDestination ( player.position );
        //Debug.Log ( "Playerpos: " + player.position );
        //Debug.Log ( "Path:" );
        /*foreach (Vector3 v3 in nav.path.corners) {
            Debug.Log ( v3 );
        }*/
	}
}
