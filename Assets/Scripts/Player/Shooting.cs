using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour {

    public int shootingFrequency = 40;
    public float attackRange = 100;

    private int timer;

    private ParticleSystem bulletParticles;
    private LineRenderer bulletLine;
    private AudioSource FireSoundEffect;
    private Light bulletLightEffects;

    private float effectDisplayTime = 0.2f;

    //public Transform PlayerBullet;

    // Use this for initialization
    void Start () {

        bulletParticles = GetComponent<ParticleSystem>();
        bulletLine = GetComponent<LineRenderer>();
        FireSoundEffect = GetComponent<AudioSource>();
        bulletLightEffects = GetComponent<Light>();
        timer = 0;
    }
	
	// Update is called once per frame
	void Update () {

        timer++;

        if (PlayerWeapons.weaponState == 1)
        {
            shootingFrequency = 40;
        }
        else if ((PlayerWeapons.weaponState == 2) || (PlayerWeapons.weaponState == 3))
        {
            shootingFrequency = 10;
        }

        if (Input.GetButton("Fire1") && timer >= shootingFrequency)
        {
            Shoot();
        }

        if (timer >= shootingFrequency * effectDisplayTime)
        {
            DisableEffect();
        }
	}

    private void DisableEffect()
    {
        bulletLightEffects.enabled = false;
        bulletLine.enabled = false;
    }

    private void Shoot()
    {
        timer = 0;

        bulletLightEffects.enabled = true;

        bulletParticles.Stop();
        bulletParticles.Play();

        //bulletLine.enabled = true;
        bulletLine.SetPosition(0, transform.position + transform.forward * 1);
        bulletLine.SetPosition(1, transform.position + transform.forward * attackRange);

        FireSoundEffect.Play();

        if ((PlayerWeapons.weaponState == 1) || (PlayerWeapons.weaponState == 2))
        {
            //Fire a bullet projectile
            GameObject CurrPlayerBullet = Instantiate(Resources.Load("PlayerBullet"), transform.position + transform.forward * 1, Quaternion.identity) as GameObject;
            if (!PlayerWeapons.skill_on)
            {
                CurrPlayerBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
            }else
            {
                CurrPlayerBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
            }
        }

        if (PlayerWeapons.weaponState == 3)
        {
            GameObject CurrPlayerBullet1 = Instantiate(Resources.Load("PlayerBullet"), transform.position + transform.right * 1, Quaternion.identity) as GameObject;
            

            GameObject CurrPlayerBullet2 = Instantiate(Resources.Load("PlayerBullet"), transform.position + transform.right * -1, Quaternion.identity) as GameObject;
            if (!PlayerWeapons.skill_on)
            {
                CurrPlayerBullet1.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
                CurrPlayerBullet2.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
            }
            else
            {
                CurrPlayerBullet1.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
                CurrPlayerBullet2.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
            }
            //Fire a bullet projectile
            //GameObject CurrPlayerBullet1 = Instantiate(Resources.Load("PlayerBullet"), new Vector3(transform.position.x -1, transform.position.y, transform.position.z-1) + transform.forward * 1, Quaternion.identity) as GameObject;
            //GameObject CurrPlayerBullet2 = Instantiate(Resources.Load("PlayerBullet"), new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 1) + transform.forward * 1, Quaternion.identity) as GameObject;
            //CurrPlayerBullet1.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
            //CurrPlayerBullet2.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
        }
    }
}
