using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

    public int startHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Image damageScreenEffect;
    //public AudioClip deathClip;

    public float flashSpeed = 5f;

    public Color flashColor = new Color(1f, 0f, 0f, 0.1f);

    AudioSource playerAudio;
    PlayerMovement playerMovement;

    bool isDead;
    bool damaged;

    // Use this for initialization
    void Start () {

        playerAudio = GetComponent<AudioSource>();
        playerMovement = GetComponent<PlayerMovement>();

        currentHealth = startHealth;	
	}
	
	// Update is called once per frame
	void Update () {

        if (damaged)
        {
            damageScreenEffect.color = flashColor;
        }
        else
        {
            damageScreenEffect.color = Color.Lerp(damageScreenEffect.color, Color.clear, flashSpeed * Time.deltaTime);
        }

        damaged = false;	
	}

    public void TakeDamage(int damageAmout)
    {
        damaged = true;

        currentHealth -= damageAmout;

        healthSlider.value = currentHealth;

        playerAudio.Play();

        if (currentHealth <= 0 && !isDead)
        {

        }
    }
}
