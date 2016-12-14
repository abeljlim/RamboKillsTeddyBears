using UnityEngine;
using System.Collections;

public class Shop : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //Don't need to use PlayerPrefs; can just reference the global of MoneyManager, whose value is preserved across scenes. However, for consistency and organization, may continue using score in PlayerPrefs.
        //int score = MoneyManager.money;
        //MoneyText.text = score.ToString("000000");
    }

    // Update is called once per frame
    void Update ()
    {
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

    // Exit the shop
    public void ExitShop()
    {
        int previousScene = PlayerPrefs.GetInt("previousScene");
        Debug.Log(previousScene);
        Application.LoadLevel(previousScene);
    }

}
