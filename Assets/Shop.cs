using UnityEngine;
using System.Collections;

public class Shop : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (PlayerPrefs.HasKey("score"))
        {
            int score = PlayerPrefs.GetInt("score");
            //Debug.Log(score);
            MoneyText.text = score.ToString("000000");
        }
        else
        {
            MoneyText.text = "000000";
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    // Exit the shop
    public void ExitShop()
    {
        int previousScene = PlayerPrefs.GetInt("previousScene");
        Application.LoadLevel(previousScene);
    }

}
