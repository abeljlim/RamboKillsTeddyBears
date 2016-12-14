using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopButton : MonoBehaviour {

    Text text;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
	}

    // Update is called once per frame
    void Update () {
	    if(transform.name == "MultiButton") {
            if (PlayerWeapons.gun3 == 0)
            {
                text.text = "Multishot - 500";
            }
            else
            {
                text.text = "Multishot Ammo - 100";
            }
        }
        if (transform.name == "RifleButton")
        {
            if (PlayerWeapons.gun2 == 0)
            {
                text.text = "Rifle";
            }
            else
            {
                text.text = "Rifle Ammo - 200";
            }
        }
    }
}
