using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyHealth : MonoBehaviour {

    public int maxHealth;
    public int currHealth;

    CapsuleCollider capsuleCollider;
    NavMeshAgent nav;

    public static GameObject MoneyText;

    Rigidbody rigidBody;
    public bool isDead;

    //hit effect
    public static float flashTime;
    public float currFlashTime = 0f;
    public bool hitFlash = false;
    public static List<Color[]> initColors; //initialized in WaveManager
    public int initColorID; //each object with a different material is supposed to have their own ID
    private static Color hitColor = Color.yellow;
    public bool isBoss;
    public static bool BossKilled = false;

	// Use this for initialization
	void Start () {
        flashTime = 0.2f;
        currHealth = maxHealth;
        capsuleCollider = GetComponent<CapsuleCollider> ();
        nav = GetComponent<NavMeshAgent> ();
        rigidBody = GetComponent<Rigidbody> ();
        isDead = false; //not dead by default


        //get original colors of the GameObject
        // grab all child objects
        if (initColors[initColorID] == null)
        {
            Renderer[] rendererObjects = GetComponentsInChildren<Renderer> ();
            //create a cache of colors if necessary
            initColors[initColorID] = new Color[rendererObjects.Length];

            // store the original colours for all child objects
            for (int i = 0; i < rendererObjects.Length; i++)
            {
                initColors[initColorID][i] = rendererObjects[i].material.color;
            }
        }
	}

	
	// Update is called once per frame
	void Update () {

        if (currFlashTime > 0f) //If the enemy got hit, and hitflash is occurring
        {
            currFlashTime -= Time.deltaTime;

            Renderer[] rendererObjects = GetComponentsInChildren<Renderer> ();
            //apply color proportional to the flash time %
            for (int i = 0; i < rendererObjects.Length; i++)
            {
                Color currColor = Color.Lerp ( hitColor, initColors[initColorID][i], 1 - (currFlashTime / flashTime) );
                rendererObjects[i].material.SetColor ( "_Color", currColor );
            }
        }
        else if (hitFlash) //when hitflash ends
        {
            hitFlash = false;

            //revert colors to their initial
            Renderer[] rendererObjects = GetComponentsInChildren<Renderer> ();
            //get colors from the initColors
            for (int i = 0; i < rendererObjects.Length; i++)
            {
                rendererObjects[i].material.SetColor ( "_Color", initColors[initColorID][i] );
            }
        }
	}

    void TakeDamage ( int hitPoints/*, Vector3 hitPoint */)
    {
        if (isDead)
            return;
        //Todo: Any damage effects
        
        //Removed: attempt to use contact points
        //Instantiate(Resources.Load("FlareCore"), hitPoint, Quaternion.identity); 


        //hitflash code - apply color to each renderer object
        currFlashTime = 0.2f; //flash to indicate damage
        hitFlash = true;
        Renderer[] rendererObjects = GetComponentsInChildren<Renderer> ();
        for (int i = 0; i < rendererObjects.Length; i++)
        {
            rendererObjects[i].material.SetColor ( "_Color", hitColor );
        }

        currHealth -= hitPoints;
        if (currHealth <= 0)
        {
            isDead = true;
            if(isBoss)
            {
                //could do some death animation
                BossKilled = true;
            }
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
            if (WaveManager.isDay == true)
                //Debug.Log ( "Collision" );
                TakeDamage(100);
            else
                TakeDamage(50);
        }
    }
}
