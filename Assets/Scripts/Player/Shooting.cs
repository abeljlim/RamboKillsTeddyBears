using UnityEngine;
using System.Collections;

public class Shooting : MonoBehaviour {

    public float shootingFrequency = 0.15f;
    public float attackRange = 100;

    private float timer;

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
    }
	
	// Update is called once per frame
	void Update () {

        timer += Time.deltaTime;

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
        timer = 0f;

        bulletLightEffects.enabled = true;

        bulletParticles.Stop();
        bulletParticles.Play();

        bulletLine.enabled = true;
        bulletLine.SetPosition(0, transform.position + transform.forward * 1);
        bulletLine.SetPosition(1, transform.position + transform.forward * attackRange);

        FireSoundEffect.Play();

        //Fire a bullet projectile
        GameObject CurrPlayerBullet = Instantiate ( Resources.Load("PlayerBullet"), transform.position + transform.forward * 1, Quaternion.identity ) as GameObject;
        CurrPlayerBullet.GetComponent<Rigidbody>().AddForce(transform.forward * 1000);
    }
}
