using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerWeapons : MonoBehaviour {

    AudioSource ButtonSound;
    public Text gun_A, gun_B;
    public static int weaponState;
    public AudioClip ButtonPress;

    // Use this for initialization
    void Start() {

        ButtonSound = GetComponent<AudioSource>();

        weaponState = 1;
    }

    // Update is called once per frame
    void Update() {

        GunSelected();
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
            weaponState = 2;
            ButtonSound.PlayOneShot(ButtonPress);
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

}
