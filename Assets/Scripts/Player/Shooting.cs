using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour
{

    public float shootingDelay = 0.66f;
    public float shootingDelayScale = 1;
    public float attackRange = 100;

    private float timer;

    private ParticleSystem bulletParticles;
    private LineRenderer bulletLine;
    private AudioSource FireSoundEffect;
    private Light bulletLightEffects;

    private float effectDisplayTime = 0.2f;

    //public Transform PlayerBullet;

    // Use this for initialization
    void Start()
    {

        bulletParticles = GetComponent<ParticleSystem>();
        bulletLine = GetComponent<LineRenderer>();
        FireSoundEffect = GetComponent<AudioSource>();
        bulletLightEffects = GetComponent<Light>();
        timer = 0;
    }

    // Update is called once per frame
    void Update()
    {

        timer += Time.deltaTime;

        if (PlayerWeapons.weaponState == 1)
        {
            shootingDelay = (0.66f * shootingDelayScale);
        }
        else if (PlayerWeapons.weaponState == 2)
        {
            shootingDelay = (0.166f * shootingDelayScale);
        }
        else if (PlayerWeapons.weaponState == 3)
        {
            shootingDelay = (0.7f * shootingDelayScale);
        }

        if ((PlayerWeapons.CurrSkill == PlayerWeapons.BULLETFRENZY || Input.GetButton("Fire1")) && timer >= shootingDelay)
        {
            Shoot();
        }

        if (timer >= shootingDelay * effectDisplayTime)
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
            if (PlayerWeapons.CurrSkill == PlayerWeapons.NONE)
            {
                CurrPlayerBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
            }
            else
            {
                CurrPlayerBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
            }

            //Add behind bullet if during bullet frenzy
            if (PlayerWeapons.CurrSkill == PlayerWeapons.BULLETFRENZY)
            {
                GameObject CurrPlayerBullet2 = Instantiate(Resources.Load("PlayerBullet"), transform.position - transform.forward * 1, Quaternion.identity) as GameObject;
                CurrPlayerBullet2.GetComponent<Rigidbody>().AddForce(-transform.forward * 1000);
            }
        }

        if (PlayerWeapons.weaponState == 3)
        {
            if (PlayerWeapons.CurrSkill == PlayerWeapons.NONE)
            {
                GameObject CurrPlayerBullet1 = Instantiate(Resources.Load("PlayerBullet"), transform.position + transform.forward * 1, Quaternion.identity) as GameObject;
                GameObject CurrPlayerBullet2 = Instantiate(Resources.Load("PlayerBullet"), transform.position + transform.forward * 1, Quaternion.identity) as GameObject;
                GameObject CurrPlayerBullet3 = Instantiate(Resources.Load("PlayerBullet"), transform.position + transform.forward * 1, Quaternion.identity) as GameObject;

                CurrPlayerBullet1.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
                CurrPlayerBullet1.GetComponent<Rigidbody>().AddForce(transform.right * -600);
                CurrPlayerBullet2.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
                CurrPlayerBullet2.GetComponent<Rigidbody>().AddForce(transform.right * 600);
                CurrPlayerBullet3.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
            }

            //Fire a bullet projectile
            //GameObject CurrPlayerBullet1 = Instantiate(Resources.Load("PlayerBullet"), new Vector3(transform.position.x -1, transform.position.y, transform.position.z-1) + transform.forward * 1, Quaternion.identity) as GameObject;
            //GameObject CurrPlayerBullet2 = Instantiate(Resources.Load("PlayerBullet"), new Vector3(transform.position.x + 1, transform.position.y, transform.position.z + 1) + transform.forward * 1, Quaternion.identity) as GameObject;
            //CurrPlayerBullet1.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
            //CurrPlayerBullet2.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
            //Add behind bullet if during bullet frenzy
            else if (PlayerWeapons.CurrSkill == PlayerWeapons.BULLETFRENZY)
            {
                GameObject CurrPlayerBullet1 = Instantiate(Resources.Load("PlayerBullet"), transform.position + transform.forward * 1, Quaternion.identity) as GameObject;
                GameObject CurrPlayerBullet2 = Instantiate(Resources.Load("PlayerBullet"), transform.position + transform.forward * 1, Quaternion.identity) as GameObject;
                GameObject CurrPlayerBullet3 = Instantiate(Resources.Load("PlayerBullet"), transform.position + transform.forward * 1, Quaternion.identity) as GameObject;

                CurrPlayerBullet1.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
                CurrPlayerBullet1.GetComponent<Rigidbody>().AddForce(transform.right * -600);
                CurrPlayerBullet2.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
                CurrPlayerBullet2.GetComponent<Rigidbody>().AddForce(transform.right * 600);
                CurrPlayerBullet3.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);

                GameObject CurrPlayerBullet4 = Instantiate(Resources.Load("PlayerBullet"), transform.position - transform.right * 1, Quaternion.identity) as GameObject;
                CurrPlayerBullet4.GetComponent<Rigidbody>().AddForce(-transform.forward * 1000);
                GameObject CurrPlayerBullet5 = Instantiate(Resources.Load("PlayerBullet"), transform.position - transform.right * -1, Quaternion.identity) as GameObject;
                CurrPlayerBullet5.GetComponent<Rigidbody>().AddForce(-transform.forward * 1000);
            }
        }
    }
}
