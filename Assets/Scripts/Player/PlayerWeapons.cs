using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEditor;

public class PlayerWeapons : MonoBehaviour {

    AudioSource ButtonSound;

    public Text skillTextImg, text_skill;
    public Image StimPackImg, BulletFrenzyImg, AutoTurretImg, PistolImg, RifleImg, ShotgunImg;
    public Text StimPackText, BulletFrenzyText, AutoTurretText, RifleText, ShotgunText; //text for the label for the hotkey

    public const int NONE = 0, STIMPACK = 1, BULLETFRENZY = 2, AUTOTURRET = 3;
    public static int CurrSkill = NONE;

    public const int RIFLE_PRICE = 500;
    public const int RIFLEAMMO_PRICE = 100;
    public const int SHOTGUN_PRICE = 300;
    public const int SHOTGUNAMMO_PRICE = 100;

    public const int STIMPACK_BASEPRICE = 800;
    public const int BULLETFRENZY_BASEPRICE = 3000;
    public const int AUTOTURRET_BASEPRICE = 8000;
    public static int STIMPACK_PRICE
    {
        get
        {
            return STIMPACK_BASEPRICE + (skillE) * 1000; //price increment
        }
    }
    public static int BULLETFRENZY_PRICE
    {
        get
        {
            return BULLETFRENZY_BASEPRICE + (skillR) * 1000; //price increment
        }
    }
    public static int AUTOTURRET_PRICE
    {
        get
        {
            return AUTOTURRET_BASEPRICE + (skillE) * 1000; //price increment
        }
    }

    public static int ShotgunAmmo, RifleAmmo;
    public bool gun1;
    public static int gun2
    {
        get
        {
            return PlayerPrefs.GetInt("gun2");
        }
        set
        {
            PlayerPrefs.SetInt("gun2", value);
        }
    }
    public static int gun3
    {
        get
        {
            return PlayerPrefs.GetInt("gun3");
        }
        set
        {
            PlayerPrefs.SetInt("gun3", value);
        }
    }
    public static int gun4
    {
        get
        {
            if(PlayerPrefs.HasKey("gun4"))
                return PlayerPrefs.GetInt("gun4");
            else
            {
                return 0;
            }
        }
        set
        {
            PlayerPrefs.SetInt("gun4", value);
        }
    }//default is false
    public static int skillE //stimpack
    {
        get
        {
            if (PlayerPrefs.HasKey("skillE"))
                return PlayerPrefs.GetInt("skillE");
            else
            {
                return 0;
            }
        }
        set
        {
            PlayerPrefs.SetInt("skillE", value);
        }
    }
    public static int skillR //bullet frenzy
    {
        get
        {
            if (PlayerPrefs.HasKey("skillR"))
                return PlayerPrefs.GetInt("skillR");
            else
            {
                return 0;
            }
        }
        set
        {
            PlayerPrefs.SetInt("skillR", value);
        }
    }

    public static int skillT //auto-turret
    {
        get
        {
            if (PlayerPrefs.HasKey("skillT"))
                return PlayerPrefs.GetInt("skillT");
            else
            {
                return 0;
            }
        }
        set
        {
            PlayerPrefs.SetInt("skillT", value);
        }
    }

    public static int GlobalScore
    {
        get
        {
            if (PlayerPrefs.HasKey("score"))
                return PlayerPrefs.GetInt("score");
            else
            {
                return 0;
            }
        }
        set
        {
            PlayerPrefs.SetInt("score", value);
        }
    }
    
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

    public AudioClip StimPackSound, BulletFrenzySound, AutoTurretSound;
    public AudioClip SkillErrorSound;
    public AudioSource SkillSoundSource;

    // Use this for initialization
    void Start() {

        if (!(Application.loadedLevelName == "Survival Game"))
            return;

        ButtonSound = GetComponent<AudioSource>();
        Debug.Log(transform.name);
        playerShooting = transform.GetChild(0).GetComponent<Shooting>(); //gets the BulletEffect's Shooting script

        //hide anything that isn't obtained
        if(skillE == 0)
        {
            StimPackImg.transform.localScale = Vector3.zero;
            StimPackText.text = "";
        }
        if (skillR == 0)
        {
            BulletFrenzyImg.transform.localScale = Vector3.zero;
            BulletFrenzyText.text = "";
        }
        if (skillT == 0)
        {
            AutoTurretImg.transform.localScale = Vector3.zero;
            AutoTurretText.text = "";
        }
        if (gun2 == 0)
        {
            RifleImg.transform.localScale = Vector3.zero;
            RifleText.text = "";
        }
        if (gun3 == 0)
        {
            ShotgunImg.transform.localScale = Vector3.zero;
            ShotgunText.text = "";
        }

        skillTimer = 0;
        SkillSoundSource = GetComponent<AudioSource>();

        PistolImg.color = Color.black;

        gun1 = true;
        if(gun2 == 1)
        {
            //gun_B.enabled = true;
        }
        //if (gun3 == 1)
        //{
        //    gun_C.enabled = true;
        //}
        //Get current shotgun ammo as of this time
        ShotgunAmmo = PlayerPrefs.GetInt("ShotgunAmmo");
        RifleAmmo = PlayerPrefs.GetInt("RifleAmmo");

        weaponState = 1;
        CurrSkill = NONE;
    }

    // Update is called once per frame
    void Update() {
        if (!(Application.loadedLevelName == "Survival Game"))
            return;

        //Skill regen code
        if(SkillPts < 100) //hardcoded as 100
        {
            SkillPts = Mathf.Min(SkillPts + skillPtsRegenRate * Time.deltaTime, 100);
            //Debug.Log(SkillPts);
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

        }
    }

    private void SkillSelected()
    {
        //Stim Pack input
        if (skillE != 0 && Input.GetKeyUp("e"))
        {
            if (SkillPts >= StimPackCost && CurrSkill == NONE)
            {
                CurrSkill = STIMPACK;
                SkillPts -= StimPackCost;
                skillPtsSlider.value = SkillPts;
                SkillSoundSource.volume = 60;
                SkillSoundSource.clip = StimPackSound;
                SkillSoundSource.Play();
            }
            else
            {
                //some kind of indication that there would not be enough skill pts at the moment.
                SkillSoundSource.clip = SkillErrorSound;
                SkillSoundSource.Play();
            }
        }

        //Bullet frenzy input
        if (skillR != 0 && Input.GetKeyUp("r"))
        {
            if (SkillPts >= BulletFrenzyCost && CurrSkill == NONE)
            {
                CurrSkill = BULLETFRENZY;
                SkillPts -= BulletFrenzyCost;
                skillPtsSlider.value = SkillPts; //update slider
                SkillSoundSource.clip = BulletFrenzySound;
                SkillSoundSource.Play();
            }
            else
            {
                //some kind of indication that there would not be enough skill pts at the moment.
                SkillSoundSource.clip = SkillErrorSound;
                SkillSoundSource.Play();
            }
        }

        //Auto turret input
        if (skillT != 0 && Input.GetKeyUp("t"))
        {
            if (SkillPts >= AutoTurretCost && CurrSkill == NONE)
            {
                CurrSkill = AUTOTURRET;
                SkillPts -= AutoTurretCost;
                skillPtsSlider.value = SkillPts; //update slider
            }
            else
            {
                //some kind of indication that there would not be enough skill pts at the moment.
                SkillSoundSource.clip = SkillErrorSound;
                SkillSoundSource.Play();
            }
        }

        //active skill
        if (CurrSkill != NONE)
        {
            skillTimer += Time.deltaTime;
            //Stimpack skill
            if (CurrSkill == STIMPACK)
            {
                if (skillTimer >= 5 + (skillE - 1) * 0.5) //duration of stimpack is hardcoded as 5 + (level of skill - 1) * 0.5 here
                {
                    playerShooting.shootingDelayScale = 1.0f;
                    CurrSkill = NONE;
                    skillTimer = 0;
                    StimPackImg.color = Color.white;
                }
            }

            if (CurrSkill == STIMPACK)
            {
                playerShooting.shootingDelayScale = 0.5f * Mathf.Pow(0.95f, skillE - 1); //set to half * 0.95 ^ (level of skill - 1)
                StimPackImg.color = Color.red;
            }

            //Bullet frenzy skill
            if (CurrSkill == BULLETFRENZY)
            {
                if (skillTimer >= 2 + ((skillR - 1) * 0.2)) //hardcoded as 2 + (level of skill - 1) here
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
                transform.RotateAround(transform.position, transform.up, Time.deltaTime * 360f);
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
            PistolImg.color = Color.black;
            RifleImg.color = Color.white;
            ShotgunImg.color = Color.white;
            ButtonSound.PlayOneShot(ButtonPress);
        }
        if (Input.GetKeyUp("2"))
        {
            if (gun2 == 1)
            {
                weaponState = 2;
                PistolImg.color = Color.white;
                RifleImg.color = Color.black;
                ShotgunImg.color = Color.white;
                ButtonSound.PlayOneShot(ButtonPress);
            }
        }
        if (Input.GetKeyUp("3"))
        {
            if (gun3 == 1)
            {
                weaponState = 3;
                PistolImg.color = Color.white;
                RifleImg.color = Color.white;
                ShotgunImg.color = Color.black;
                ButtonSound.PlayOneShot(ButtonPress);
            }
        }

        if (weaponState == 1)
        {

        }
        else if (weaponState == 2)
        {

        }

    }

    public void EnableRifle() //when Rifle button is clicked in the shop
    {
        if (gun2 == 0)
        {
            if (GlobalScore >= RIFLE_PRICE)
            {
                PlayerPrefs.SetInt("score", GlobalScore - RIFLE_PRICE);
                int CurrRifleAmmo = PlayerPrefs.GetInt("RifleAmmo");
                PlayerPrefs.SetInt("RifleAmmo", CurrRifleAmmo + 100);
                gun2 = 1;
            }
        }
        else //have rifle; buying ammo instead
        {
            if (GlobalScore >= RIFLEAMMO_PRICE)
                GlobalScore -= RIFLEAMMO_PRICE;
            int CurrRifleAmmo = PlayerPrefs.GetInt("RifleAmmo");
            PlayerPrefs.SetInt("RifleAmmo", CurrRifleAmmo + 100);
        }
    }

    public void EnableSpread() //when Multishot button is clicked in the shop
    {
        if (gun3 == 0)
        {
            if (GlobalScore >= SHOTGUN_PRICE)
            {
                GlobalScore -= SHOTGUN_PRICE;
                int CurrShotgunAmmo = PlayerPrefs.GetInt("ShotgunAmmo");
                PlayerPrefs.SetInt("ShotgunAmmo", CurrShotgunAmmo + 100);
                gun3 = 1;
                //gun_C.enabled = true; //do gun_C
            }
            //gun3 = 1;
            //gun_B.enabled = true;
        }
        else
        {
            if (GlobalScore >= SHOTGUNAMMO_PRICE)
                GlobalScore -= SHOTGUNAMMO_PRICE;
            int CurrShotgunAmmo = PlayerPrefs.GetInt("ShotgunAmmo");
            PlayerPrefs.SetInt("ShotgunAmmo", CurrShotgunAmmo + 100);
        }

    }
    public void EnableStimPack()
    {
        if(GlobalScore >= STIMPACK_PRICE)
        {
            GlobalScore -= STIMPACK_PRICE;
            skillE++; //level up the skill
        }
    }
    public void EnableBulletFrenzy()
    {
        if (GlobalScore >= BULLETFRENZY_PRICE)
        {
            GlobalScore -= BULLETFRENZY_PRICE;
            skillR++;
        }
    }
    public void EnableAutoTurret()
    {
        if (GlobalScore >= AUTOTURRET_PRICE)
        {
            GlobalScore -= AUTOTURRET_PRICE;
            skillT++;
        }
    }
}
