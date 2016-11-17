using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour {

    public static int money; //accessible from other classes and instances
    public Text text;

    void Start()
    {
        money = 0; //reset money
    }

	void Awake () {
        text = GetComponent<Text> (); //get the UI text
        Debug.Log ( text );
        money = 0;
	}
	
	void Update () {
        // money from previous round
        int newMoney = PlayerPrefs.GetInt("score");

        money = newMoney;

        text.text = money.ToString("000000");
	}
}
