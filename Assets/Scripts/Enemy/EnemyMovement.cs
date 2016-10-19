using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

    Transform player;

    NavMeshAgent nav;
    EnemyHealth enemyHealth;

	// Use this for initialization
	void Start () {

        player = GameObject.FindGameObjectWithTag("Player").transform;
        nav = GetComponent<NavMeshAgent> ();
        enemyHealth = GetComponent<EnemyHealth> ();

	}
	
	// Update is called once per frame
	void Update () {
        if (enemyHealth.isDead)
        {
            /*if (nav != null) //already done in the EnemyHealth.Die()
            {
                Destroy ( nav );
            }*/
            //cease movement, and do nothing else if dead
            /*if (nav.speed != 0)
            {
                nav.speed = 0;
                nav.acceleration = 0;
                nav.speed = 0;
            }*/
            return;
        }
        nav.SetDestination ( player.position );
        //Debug.Log ( "Playerpos: " + player.position );
        //Debug.Log ( "Path:" );
        /*foreach (Vector3 v3 in nav.path.corners) {
            Debug.Log ( v3 );
        }*/
	}
}
