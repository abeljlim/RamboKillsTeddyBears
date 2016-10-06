using UnityEngine;
using System.Collections;

public class EnemyAttack : MonoBehaviour {

    public float attackFrequency = .5f;
    public int attackDamage = 10;

    GameObject player;

    PlayerHealth playerHealth;

    bool playerInRange;
    float timer;

    // Use this for initialization
    void Start () {

        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
	
	}
	
	// Update is called once per frame
	void Update () {

        timer += Time.deltaTime;
        if (timer >= attackFrequency && playerInRange)
        {
            Attact();
        }
	
	}

    void Attact()
    {
        timer = 0;

        if (playerHealth.currentHealth > 0)
        {
            playerHealth.TakeDamage(attackDamage);
        }
    }

    void OnTriggerEnter(Collider other)
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
