using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class WaveManager : MonoBehaviour {

    //access level with EnemySpawner.level;
    public static int level = 1;
    public GameObject[] SpawnPoint; //Not used
    public string[] levelName; //to correspond to the enemies spawned in the enemy gameobject in EnemySpawner
    public static float waveTime; //current wave time
    public int[] levelTime; //full time for the wave to finish
    public int waveStartTime = 5;
    public static bool isDay = true;
    //public static bool prevWasDay = true;

    public Slider bossHealthSlider;
    public EnemyHealth bossEnemyHealth;

    AudioSource BGMusic;
    public AudioClip NightClip, DayClip;
    //DoubleAudioSource doubleAudioSource;
    public AudioSource DaySource, NightSource;

// Use this for initialization
// start before any of the SpawnPoint code runs
void Awake ()
    {

        level = 1;
        if (PlayerPrefs.HasKey("currentLevel")) //new level
        {
            level = PlayerPrefs.GetInt("currentLevel");
            LevelNameText.text = levelName[level - 1]; //level-1 is the 0-based level
            //Debug.Log("Level: " + level);
        }
        waveTime = 0;
        EnemySpawner.spawning = true;
        if(EnemyHealth.initColors == null)
        {
            //Debug.Log(EnemyHealth.initColors);
            //instantiate initColors
            EnemyHealth.initColors = new List<Color[]>(10); //with 4 colors at the moment
            for (int i = 0; i < 10; i++)
                EnemyHealth.initColors.Add(null);
        }
        BGMusic = GetComponent<AudioSource>();
        //doubleAudioSource = GetComponent<DoubleAudioSource>();
        if(isDay)
        {
            BGMusic.clip = DayClip;
            BGMusic.Play();
        }
        else
        {
        }
    }

	void Start () {
        PlanetOrbit.SecondsInDay = levelTime[level - 1]; //update time of the day with the wave time.
        //bossHealthSlider = 
	}
	
	// Update is called once per frame
	void Update () {
        waveTime += Time.deltaTime;

        if (!EnemyHealth.bossExists)
        {
            //Debug.Log("Disable boss health");
            bossHealthSlider.enabled = false;
            bossHealthSlider.transform.localScale = new Vector3(0, 0, 0); //hide the slider - this being one way to do it; another would be to have a CanvasGroup component.
            //bossHealthSlider.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            //if (bossHealthSlider != null) //if this would have been set by a boss - update boss health accordingly
            //{
            //doing both here ...
            bossHealthSlider.enabled = true;
            bossHealthSlider.transform.localScale = new Vector3(1, 1, 1); //hide the slider - this being one way to do it; another would be to have a CanvasGroup component.
            bossHealthSlider.value = bossEnemyHealth.currHealth;
            //}
        }

        //advance to next level when time of wave elapses, as long as more levels exist
        //or, advance when a boss is killed
        if (EnemyHealth.BossKilled || waveTime >= levelTime[level-1] && level < levelName.Length)
        {
            //Time.timeScale = 0;
            EnemyHealth.BossKilled = false;

            LevelNameText.text = levelName[level]; //level-1 is the 0-based level
            EnemySpawner.spawning = false; //pause for the period of waveStartTime

            level++;
            PlanetOrbit.SecondsInDay = levelTime[level - 1];
            waveTime = 0;

            //call the PerLevelUpdate method in each SpawnPoint in the children of WaveManager
            foreach (Transform child in transform)
            {
                child.gameObject.GetComponent<EnemySpawner>().PerLevelUpdate();
            }

            PlayerPrefs.SetInt("previousScene", Application.loadedLevel);
            PlayerPrefs.SetInt("currentLevel", level);

            PlayerPrefs.SetInt("score", MoneyManager.money);

            //pass ammo of weapons
            //PlayerPrefs.SetInt("ShotgunAmmo", PlayerWeapons.ShotgunAmmo);
            //PlayerPrefs.SetInt("RifleAmmo", PlayerWeapons.RifleAmmo);

            EnemyHealth.bossExists = false; //no more boss at the end of the level
            Application.LoadLevel("Shop");
        }

        //toggle the state between night and day
        if ((waveTime >= 0) && (waveTime < (levelTime[level - 1] / 2)))
        {
            isDay = true;
        }
        else
        {
            isDay = false;
        }

        if(!PlanetOrbit.isDay && BGMusic.clip == DayClip)
        {
            BGMusic.clip = NightClip;
            //doubleAudioSource.CrossFade(NightClip, 100, 3);
            BGMusic.Stop();
            BGMusic.Play();
        }
        else if(PlanetOrbit.isDay && BGMusic.clip == NightClip)
        {
            //doubleAudioSource.CrossFade(DayClip, 100, 3);
            BGMusic.clip = DayClip;
            BGMusic.Stop();
            BGMusic.Play();
            //BGMusic.cross
        }

        //start spawning of the current wave after waveStartTime
        if (waveTime >= waveStartTime && !EnemySpawner.spawning)
        {
            EnemySpawner.spawning = true;
        }
	}
}
