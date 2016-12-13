using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MoneyText : MonoBehaviour {


    public static string text; //accessible from other classes and instances
    public Text textObj;

    // Use this for initialization
    void Start () {
        textObj = GetComponent<Text>(); //get the UI text
    }
	
	// Update is called once per frame
	void Update ()
    {
        textObj.text = text;
    }
}

