﻿using UnityEngine;
using System.Collections;

public class PlayerBullet : MonoBehaviour {

    // Use this for initialization
    GameObject ground;
    
    //int used to prevent immediate consecutive damage, by only allowing damage after 0.5sec. has elapsed before the next hit.
    public float timeSincePlayerDamage = 0;
    public const float timeBeforeDamagingPlayer = 0.5f; //
    public bool justHitPlayer = false;

	void Start () {
        //When the bullets go out of bounds, then destroy them.
	}
	
	// Update is called once per frame
	void Update () {
        ground = GameObject.FindGameObjectWithTag ( "LevelArea" );
        Vector3 levelMin = ground.GetComponent<Collider>().bounds.min;
        Vector3 levelMax = ground.GetComponent<Collider>().bounds.max;
        //if out of bounds (based on the ground area) by 15, then delete the bullet
        if (transform.position.x < levelMin.x-15 || transform.position.z < levelMin.z-15 || transform.position.x > levelMax.x+15 || transform.position.z > levelMax.z+15)
            Destroy ( gameObject );

        //allow temporary prevention of damage of player by this current bullet, to avoid accidental 'rapidfire' hits from what could be multiple collisions (eg. if the player re-contacts with the bullet after it is already going away
        if (justHitPlayer)
        {
            timeSincePlayerDamage += Time.deltaTime;
            if (timeSincePlayerDamage >= timeBeforeDamagingPlayer)
            {
                timeSincePlayerDamage = 0;
                justHitPlayer = false;
            }
        }
	}
    
    void OnCollisionEnter ( Collision other )
    {
        GameObject player = GameObject.FindGameObjectWithTag ( "Player" );
        if (other.gameObject == player)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth> ();
            playerHealth.TakeDamage ( 20 );
            justHitPlayer = true;
        }
    }
}
