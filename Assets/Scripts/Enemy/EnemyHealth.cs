using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

    public int maxHealth = 100;
    public int currHealth;

    CapsuleCollider capsuleCollider;
    NavMeshAgent nav;

    public static GameObject MoneyText;

    Rigidbody rigidBody;
    public bool isDead;

	// Use this for initialization
	void Start () {
        currHealth = maxHealth;
        capsuleCollider = GetComponent<CapsuleCollider> ();
        nav = GetComponent<NavMeshAgent> ();
        rigidBody = GetComponent<Rigidbody> ();
        isDead = false; //not dead by default
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void TakeDamage ( int hitPoints )
    {
        if (isDead)
            return;
        //Todo: Any damage effects
        currHealth -= hitPoints;
        if (currHealth <= 0)
        {
            isDead = true;
            Die ();
        }
    }

    void Die ()
    {
        //Todo: play death animation; get fade to work properly (need to make material transparent, which requires getting a material itself)
        FadeObjectInOut fadeObj = gameObject.GetComponent<FadeObjectInOut> ();
        //this.FadeOut (); 
        fadeObj.FadeOut ( 0.75f );
        capsuleCollider.isTrigger = true; //make the enemy intangible
        Destroy ( rigidBody );
        nav.enabled = false;
        MoneyManager.money += 20;
        Destroy ( gameObject , 0.75f);
    }

    /// <summary>
    /// Damage code
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter ( Collider other )
    {
        if (other.gameObject.CompareTag ( "PlayerBullet" ))
        {
            //Debug.Log ( "Collision" );
            TakeDamage ( 40 ); //current
        }
    }
}
