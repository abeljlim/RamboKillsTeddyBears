using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerWeapons : MonoBehaviour {

    AudioSource ButtonSound;
    public Text gun_A, gun_B;

    public Text skill, text_skill;
    public static bool skill_on;
    public bool gun1, gun2, gun3, gun4;

    private int skillTimer;

    public static int weaponState;
    public AudioClip ButtonPress;

    // Use this for initialization
    void Start() {

        ButtonSound = GetComponent<AudioSource>();

        skillTimer = 0;

        gun1 = true;
        gun2 = true;
        gun3 = false;
        gun4 = false;

        weaponState = 1;
        skill_on = false;
    }

    // Update is called once per frame
    void Update() {

        SkillSelected();
        GunSelected();

        if(!gun1)
        {
            gun_A.enabled = false;
        }
    }

    private void SkillSelected()
    {
        if (Input.GetKeyUp("r") && skill_on == false)
        {
            skill_on = true;
        }

        if(skill_on)
        {
            skillTimer++;
            if (skillTimer > 150)
                skill_on = false;
        }

        if (skill_on)
        {
            text_skill.enabled = true;
            skill.color = Color.black;
        }else
        {
            text_skill.enabled = false;
            skill.color = Color.white;
        }
    }

    private void GunSelected()
    {
        if(Input.GetKeyUp("1"))
        {
            weaponState = 1;
            ButtonSound.PlayOneShot(ButtonPress);
        }
        if (Input.GetKeyUp("2"))
        {
            if (gun1)
            {
                weaponState = 2;
                ButtonSound.PlayOneShot(ButtonPress);
            }
        }
        if (Input.GetKeyUp("3"))
        {
            if (gun2)
            {
                weaponState = 3;
                ButtonSound.PlayOneShot(ButtonPress);
            }
        }

        if (weaponState == 1)
        {
            gun_A.color = Color.white;
            gun_B.color = Color.black;
        }
        else if (weaponState == 2)
        {
            gun_B.color = Color.white;
            gun_A.color = Color.black;
        }

    }

    public void EnableRifle()
    {
        gun1 = true;
        gun_B.enabled = true;
    }

    public void EnableSpread()
    {
        gun2 = true;
        gun_B.enabled = true;
    }

}
