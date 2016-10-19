using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour {

    public static int money; //accessible from other classes and instances
    public Text text;

	void Awake () {
        text = GetComponent<Text> (); //get the UI text
        Debug.Log ( text );
        money = 0; //reset money
	}
	
	void Update () {
        text.text = money.ToString("000000");
	}
}
