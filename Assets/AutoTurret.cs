using UnityEngine;
using System.Collections;

public class AutoTurret : MonoBehaviour {

    public float shootingDelay = 0.66f;
    public float shootingDelayScale = 1;
    public float attackRange = 100;

    private float shotTimer;
    public float turretTimer = 2; //how long the turret lasts until it's done
    private bool isDead = false;

    private ParticleSystem bulletParticles;
    private LineRenderer bulletLine;
    private AudioSource FireSoundEffect;
    private Light bulletLightEffects;

    private float effectDisplayTime = 0.2f;

    Rigidbody turretRigidbody;

    // Use this for initialization
    void Start () {
        turretRigidbody = transform.parent.gameObject.GetComponent<Rigidbody>();

        bulletParticles = GetComponent<ParticleSystem>();
        bulletLine = GetComponent<LineRenderer>();
        FireSoundEffect = GetComponent<AudioSource>();
        bulletLightEffects = GetComponent<Light>();
        shotTimer = 0;
    }
	
	// Update is called once per frame
	void Update () {

        if (isDead)
            return;

        //target the nearest enemy among the list of enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        //doing basic detection, not necessarily doing quad or octree for detecting closest enemy - as it is just one turret
        //although could use the enemy's tree if that would be set up
        Vector3 closestEnemy = Vector3.zero;
        //Debug.Log(enemies.Length);
        if (enemies.Length >= 1)
        {
            closestEnemy = enemies[0].transform.position;
            float sqrClosestDistance = (enemies[0].transform.position - transform.position).sqrMagnitude;
            foreach (GameObject enemy in enemies) //redoing the first enemy with foreach, but for code simplicity
            {
                Vector3 enemyPos = enemy.transform.position;
                float sqrDistance = (enemyPos - transform.position).sqrMagnitude;
                if (sqrDistance < sqrClosestDistance)
                {
                    closestEnemy = enemyPos;
                    sqrClosestDistance = sqrDistance;
                }
            }
        }
        //LookRotation - code taken from PlayerMovement.cs to base on it

        // Create a vector from the player to the point on the floor the raycast from the mouse hit
        // Actually, the result is not the hitpoint, little farther but the direction is the same
        Vector3 turretToEnemyPos = closestEnemy - transform.position;

        turretToEnemyPos.y = 0f;
        // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
        Quaternion newRotation = Quaternion.LookRotation(turretToEnemyPos);
        //Debug.Log(turretToEnemyPos.x+", "+turretToEnemyPos.z);

        // Operate Player to Rotate
        turretRigidbody.MoveRotation(newRotation);

        //shooting periodically code
        shotTimer += Time.deltaTime;
        if(shotTimer >= shootingDelay)
            Shoot();

        turretTimer -= Time.deltaTime;
        if(turretTimer <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        //Todo: play death animation; get fade to work properly (need to make material transparent, which requires getting a material itself)
        isDead = true;
        FadeObjectInOut fadeObj = transform.parent.gameObject.GetComponent<FadeObjectInOut>();
        //this.FadeOut (); 
        fadeObj.FadeOut(0.75f);
        //capsuleCollider.isTrigger = true; //make the turret intangible
        //Destroy(rigidBody);
        //nav.enabled = false;
        Destroy(transform.parent.gameObject, 0.75f);
    }

    private void Shoot()
    {
        shotTimer = 0;

        bulletLightEffects.enabled = true;

        bulletParticles.Stop();
        bulletParticles.Play();

        //bulletLine.enabled = true;
        bulletLine.SetPosition(0, transform.position + transform.forward * 1);
        bulletLine.SetPosition(1, transform.position + transform.forward * attackRange);

        FireSoundEffect.Play();

        //if ((PlayerWeapons.weaponState == 1) || (PlayerWeapons.weaponState == 2))
        //{
            //Fire a bullet projectile
        GameObject CurrPlayerBullet = Instantiate(Resources.Load("PlayerBullet"), transform.position + transform.forward * 1, Quaternion.identity) as GameObject;
        //Debug.Log(CurrPlayerBullet.transform.position.x);
        //if (!PlayerWeapons.CurrSkill)
        //{
        CurrPlayerBullet.GetComponent<Rigidbody>().AddForce(-transform.up * 1000);
        //    }
        //    else
        //    {
        //        CurrPlayerBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
        //    }
        ////}

        //if (PlayerWeapons.weaponState == 3)
        //{
        //    GameObject CurrPlayerBullet1 = Instantiate(Resources.Load("PlayerBullet"), transform.position + transform.right * 1, Quaternion.identity) as GameObject;


        //    GameObject CurrPlayerBullet2 = Instantiate(Resources.Load("PlayerBullet"), transform.position + transform.right * -1, Quaternion.identity) as GameObject;
        //    if (!PlayerWeapons.CurrSkill)
        //    {
        //        CurrPlayerBullet1.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
        //        CurrPlayerBullet2.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
        //    }
        //    else
        //    {
        //        CurrPlayerBullet1.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
        //        CurrPlayerBullet2.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
        //    }
        //Fire a bullet projectile
        //GameObject CurrPlayerBullet1 = Instantiate(Resources.Load("PlayerBullet"), new Vector3(transform.position.x -1, transform.position.y, transform.position.z-1) + transform.forward * 1, Quaternion.identity) as GameObject;
        //GameObject CurrPlayerBullet2 = Instantiate(Resources.Load("PlayerBullet"), new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 1) + transform.forward * 1, Quaternion.identity) as GameObject;
        //CurrPlayerBullet1.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
        //CurrPlayerBullet2.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
        //}
    }
}
