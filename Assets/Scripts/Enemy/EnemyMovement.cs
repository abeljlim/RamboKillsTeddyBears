using UnityEngine;
using System.Collections;

public class EnemyMovement : MonoBehaviour {

    Transform player;

    NavMeshAgent nav;

    //Speed changes depending on the time of day
    //public float daySpeed = 3;
    //public float nightSpeed = 4.5f;
    public float daySpeed = 3.75f;
    public float daySpeedDeviation = 0.75f;

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

        //Set speed depending on time of day.
        //The later at night, the faster the enemies move.
        nav.speed = 3.75f + (-PlanetOrbit.isDayAmt) * daySpeedDeviation;


        //non-fuzzy logic approach to speed
        /*if (PlanetOrbit.isDayAmt && nav.speed != daySpeed)
        {
            nav.speed = daySpeed;
        }
        else if (!PlanetOrbit.isDay && nav.speed != nightSpeed)
        {
            nav.speed = nightSpeed;
        }
*/	}
}
