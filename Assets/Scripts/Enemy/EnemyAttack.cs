using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour {

    public float attackFrequency = .5f;
    public int attackDamage = 10;

    GameObject player;

    PlayerHealth playerHealth;
    EnemyHealth enemyHealth;

    bool playerInRange;
    float timer;

    // Use this for initialization
    void Start () {

        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        enemyHealth = GetComponent<EnemyHealth> ();
	}
	
	// Update is called once per frame
	void Update () {

        timer += Time.deltaTime;
        if (timer >= attackFrequency && playerInRange && !enemyHealth.isDead)
        {
            Attack();
        }
	
	}

    void Attack()
    {
        timer = 0;

        if (playerHealth.currentHealth > 0)
        {
            if(WaveManager.isDay == true)
                playerHealth.TakeDamage(attackDamage);
            else
                playerHealth.TakeDamage(2 * attackDamage);
        }
    }
    
    void OnTriggerEnter ( Collider other )
    {
        if (other.gameObject == player)
        {
            playerInRange = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInRange = false;
        }
    }

}
