using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour {

    Text text;
    Image image;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        image = GetComponent<Image>();
    }
    public int ShotgunAmmo
    {
        get
        {
            return PlayerPrefs.GetInt("ShotgunAmmo");
        }
    }
    public int RifleAmmo
    {
        get
        {
            return PlayerPrefs.GetInt("RifleAmmo");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(PlayerWeapons.gun2);
        switch (transform.name) {
            case "RifleButton":
                if (PlayerWeapons.gun2 == 0)
                {
                    text.text = "Rifle";
                }
                else
                {
                    text.text = "Rifle Ammo";
                }
                break;
            case "MultiButton":
                if (PlayerWeapons.gun3 == 0)
                {
                    text.text = "Multishot";
                }
                else
                {
                    text.text = "Multishot Ammo";
                }            
                break;
            case "RifleAmmoPrice":
                if(PlayerWeapons.gun2 == 1)
                {
                    text.text = PlayerWeapons.RIFLEAMMO_PRICE.ToString();
                }
                else
                {
                    text.text = PlayerWeapons.RIFLE_PRICE.ToString();
                }
                break;
            case "MultiAmmoPrice":
                if (PlayerWeapons.gun3 == 1)
                {
                    text.text = PlayerWeapons.SHOTGUNAMMO_PRICE.ToString(); ;
                }
                else
                {
                    text.text = PlayerWeapons.SHOTGUN_PRICE.ToString(); ;
                }
                break;
            case "RifleAmmo":
                if (PlayerWeapons.gun2 == 1)
                {
                    text.text = RifleAmmo.ToString();
                }
                else
                {
                    text.text = "";
                }
                break;
            case "MultiAmmo":
                if (PlayerWeapons.gun3 == 1)
                {
                    text.text = ShotgunAmmo.ToString();
                }
                else
                {
                    text.text = "";
                }
                break;
            case "StimPackLabel":
            case "StimPackImage":
            case "StimPackPrice":
                if(PlayerWeapons.skillE == 1)
                {
                    GreyComponent();
                }
                break;
            case "BulletFrenzyLabel":
            case "BulletFrenzyImage":
            case "BulletFrenzyPrice":
                if (PlayerWeapons.skillR == 1)
                {
                    GreyComponent();
                }
                break;
            case "AutoTurretLabel":
            case "AutoTurretImage":
            case "AutoTurretPrice":
                if (PlayerWeapons.skillT == 1)
                {
                    GreyComponent();
                }
                break;
        }
    }
    void GreyComponent()
    {
        if(text != null)
        {
            text.color = Color.grey;
        }
        if (image != null)
        {
            image.color = Color.grey;
        }
    }
}
