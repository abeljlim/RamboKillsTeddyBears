using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerWeapons : MonoBehaviour {

    AudioSource ButtonSound;
    public Text gun_A, gun_B;

    public Text skillTextImg, text_skill;
    public Image StimPackImg, BulletFrenzyImg, AutoTurretImg;
    public const int NONE = 0, STIMPACK = 1, BULLETFRENZY = 2, AUTOTURRET = 3;
    public static int CurrSkill = NONE;
    public bool gun1, gun2, gun3, gun4;
    
    private float skillTimer;

    public static int weaponState;
    public AudioClip ButtonPress;
    public Shooting playerShooting;
    public Slider skillPtsSlider;
    public float SkillPts = 100;
    public float skillPtsRegenRate = 2f; //regen per second
    public int StimPackCost = 25;
    public int BulletFrenzyCost = 25;
    public int AutoTurretCost = 25;

    // Use this for initialization
    void Start() {

        ButtonSound = GetComponent<AudioSource>();
        playerShooting = transform.GetChild(0).GetComponent<Shooting>(); //gets the BulletEffect's Shooting script

        skillTimer = 0;

        gun1 = true;
        gun2 = true;
        gun3 = false;
        gun4 = false;

        weaponState = 1;
        CurrSkill = NONE;
    }

    // Update is called once per frame
    void Update() {

        //Skill regen code
        if(SkillPts < 100) //hardcoded as 100
        {
            SkillPts = Mathf.Min(SkillPts + skillPtsRegenRate * Time.deltaTime, 100);
            Debug.Log(SkillPts);
            skillPtsSlider.value = SkillPts;
        }

        if (SkillPts < StimPackCost && CurrSkill != STIMPACK)
            StimPackImg.color = Color.grey;
        else
            StimPackImg.color = Color.white;
        if (SkillPts < BulletFrenzyCost && CurrSkill != BULLETFRENZY)
            BulletFrenzyImg.color = Color.grey;
        else
            BulletFrenzyImg.color = Color.white;
        if (SkillPts < AutoTurretCost && CurrSkill != AUTOTURRET)
            AutoTurretImg.color = Color.grey;
        else
            AutoTurretImg.color = Color.white;

        SkillSelected();
        GunSelected();

        if(!gun1)
        {
            gun_A.enabled = false;
        }
    }

    private void SkillSelected()
    {
        //Stim Pack input
        if (Input.GetKeyUp("e") && CurrSkill == NONE)
        {
            if (SkillPts >= StimPackCost)
            {
                CurrSkill = STIMPACK;
                SkillPts -= StimPackCost;
                skillPtsSlider.value = SkillPts;
            }
            else
            {
                //some kind of indication that there would not be enough skill pts at the moment.
            }
        }

        //Bullet frenzy input
        if (Input.GetKeyUp("r") && CurrSkill == NONE)
        {
            if (SkillPts >= BulletFrenzyCost)
            {
                CurrSkill = BULLETFRENZY;
                SkillPts -= BulletFrenzyCost;
                skillPtsSlider.value = SkillPts; //update slider
            }
            else
            {
                //some kind of indication that there would not be enough skill pts at the moment.
            }
        }

        //Auto turret input
        if (Input.GetKeyUp("t") && CurrSkill == NONE)
        {
            if (SkillPts >= AutoTurretCost)
            {
                CurrSkill = AUTOTURRET;
                SkillPts -= AutoTurretCost;
                skillPtsSlider.value = SkillPts; //update slider
            }
            else
            {
                //some kind of indication that there would not be enough skill pts at the moment.
            }
        }

        //active skill
        if (CurrSkill != NONE)
        {
            skillTimer += Time.deltaTime;
            //Stimpack skill
            if (CurrSkill == STIMPACK)
            {
                if (skillTimer >= 5) //duration of stimpack is hardcoded as 5 here
                {
                    playerShooting.shootingDelayScale = 1.0f;
                    CurrSkill = NONE;
                    skillTimer = 0;
                    skillTextImg.color = Color.white;
                    StimPackImg.color = Color.white;
                    text_skill.enabled = false;
                }
            }

            if (CurrSkill == STIMPACK)
            {
                playerShooting.shootingDelayScale = 0.5f; //set to half
                text_skill.enabled = true;
                skillTextImg.color = Color.black;
                StimPackImg.color = Color.red;
            }

            //Bullet frenzy skill
            if (CurrSkill == BULLETFRENZY)
            {
                if (skillTimer >= 2) //hardcoded as 2 here
                {
                    playerShooting.shootingDelayScale = 1.0f; //end
                    BulletFrenzyImg.color = Color.white;
                    CurrSkill = NONE;
                    skillTimer = 0;
                }
            }

            if (CurrSkill == BULLETFRENZY)
            {
                BulletFrenzyImg.color = Color.red;
                playerShooting.shootingDelayScale = 0.1f; //set to 5x speed
                transform.RotateAround(transform.position, transform.up, Time.deltaTime * 720f);
                //handle color is TBD
                //text_skill.enabled = true;
                //skill.color = Color.black;
            }
            //else
            //{
            //    //text_skill.enabled = false;
            //    //skill.color = Color.white;
            //}


            if (CurrSkill == AUTOTURRET)
            {
                GameObject NewAutoTurret = Instantiate(Resources.Load("AutoTurretv2"), transform.position + transform.forward * 2, Quaternion.identity) as GameObject;
                CurrSkill = NONE;
                skillTimer = 0;
            }
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
