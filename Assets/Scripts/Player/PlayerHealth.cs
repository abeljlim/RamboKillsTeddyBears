﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

    public int startHealth = 100;
    public int currentHealth;
    public Slider healthSlider;
    public Image damageScreenEffect;
    public AudioClip deathClip;
    int GOtimer = 0;

    public float flashSpeed = 5f;

    public Color flashColor = new Color(1f, 0f, 0f, 0.1f);
    public const float redScreenTime = 1f; //for death screen reddening
    public float currGameOverTime = 0f;
    public float currRedScreenTime = 0f; //for death screen reddening

    //public int waitUpdateCycle = 0; //bool used just to wait an update cycle before executing the next code
    AudioSource playerAudio;
    PlayerMovement playerMovement;
    public AudioClip HurtClip;

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

        // Debug for health regen
        if (Input.GetKeyUp("h"))
        {
            if (currentHealth < 100)
            {
                currentHealth += 10;
                healthSlider.value = currentHealth;
            }
        }

        CharacterState ();
        if (isDead)
        {
            GOtimer++;
            //Begin game over 'animation' with fading text
            if (currGameOverTime < 5f)
            {
                currGameOverTime += PauseManager.deltaTime;
                GameObject[] GameOverObjs = GameObject.FindGameObjectsWithTag ( "GameOverTxt" );

                //manually set
                GameObject GameTxt, OverTxt;
                GameTxt = GameOverObjs[1];
                OverTxt = GameOverObjs[0];

                //ensure that the Game Over text would be found
                foreach(GameObject g in GameOverObjs) {
                    if(g.transform.name == "GameOverPt1") {
                        GameTxt = g;
                    } else 
                    if (g.transform.name == "GameOverPt2")
                    {
                        OverTxt = g;
                    }
                }

                if (currGameOverTime >= 2f && !GameTxt.GetComponent<UITextFade> ().faded)
                {
                    GameTxt.GetComponent<UITextFade> ().FadeIn ();
                }
                if (currGameOverTime >= 3f && !OverTxt.GetComponent<UITextFade> ().faded)
                {
                    OverTxt.GetComponent<UITextFade> ().FadeIn ();
                }
            }
            if (currRedScreenTime < redScreenTime)
            {
                currRedScreenTime += PauseManager.deltaTime;
                damageScreenEffect.color = Color.Lerp ( Color.clear, Color.red, currRedScreenTime / redScreenTime * 0.6f );
                //Debug.Log ( currRedScreenTime );
            }

            if (GOtimer == 400)
            {
                PauseManager.Unpause ();
                Application.LoadLevel("MainMenu");
            }

            return;
        }
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
        if (PlayerWeapons.CurrSkill == PlayerWeapons.BULLETFRENZY) //invincible during Bullet Frenzy
            return;

        damaged = true;

        currentHealth -= damageAmout;


        healthSlider.value = currentHealth;

        if (currentHealth <= 0 && !isDead)
        {
            //code here for player dying
            isDead = true;
            PlayerDie ();
            return;
        }

        playerAudio.clip = HurtClip;
        playerAudio.Play();
    }

    public void PlayerDie ()
    {
        playerAudio.clip = deathClip;
        playerAudio.Play ();
        damageScreenEffect.color = flashColor;
        PauseManager.Pause ();
    }

    public void CharacterState()
    {
        if (currentHealth > 70)
        {
            //characterState.text = "d";
            //characterState.color = Color.green;
        }
        else if (currentHealth <= 70 && currentHealth >= 40)
        {
            //characterState.text = "h";
            //characterState.color = Color.yellow;
        }
        else if (currentHealth < 40 && currentHealth > 0)
        {
            //characterState.text = "x";
            //characterState.color = Color.red;
        }
        else
        {
            //characterState.text = "z";
            //characterState.color = Color.white;
        }
    }
    /// <summary>
    /// Player takes damage from his own bullets. No longer used; refer to PlayerBullet class instead.
    /// </summary>
    /// <param name="other"></param>
    /*void OnCollisionEnter ( Collision other )
    {
        if (other.gameObject.CompareTag ( "PlayerBullet" ))
        {
            //Debug.Log ( "Collision" );
            TakeDamage ( 20 ); //current
            justHitPlayer = true;
        }
    }*/
}
