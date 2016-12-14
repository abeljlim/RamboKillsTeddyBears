using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AmmoTracker : MonoBehaviour {
    Text text;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
	}

    // Update is called once per frame
    void Update() {
        switch (PlayerWeapons.weaponState) {
            case 1:
                text.text = "";
                break;
            case 2:
                text.text = PlayerPrefs.GetInt("RifleAmmo").ToString();
                break;
            case 3:
                text.text = PlayerPrefs.GetInt("ShotgunAmmo").ToString();
                break;
            default:
                text.text = "";
                break;
        }
	}
}
