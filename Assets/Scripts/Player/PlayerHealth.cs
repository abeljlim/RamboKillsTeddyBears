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

    public Text characterState;

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

        CharacterState();

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

    public void CharacterState()
    {
        if (currentHealth > 70)
        {
            characterState.text = "d";
            characterState.color = Color.green;
        }
        else if (currentHealth <= 70 && currentHealth >= 40)
        {
            characterState.text = "h";
            characterState.color = Color.yellow;
        }
        else if (currentHealth < 40 && currentHealth > 0)
        {
            characterState.text = "x";
            characterState.color = Color.red;
        }
        else
        {
            characterState.text = "z";
            characterState.color = Color.white;
        }
    }
}
