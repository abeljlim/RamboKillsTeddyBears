using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelNameText : MonoBehaviour
{

    public static string text; //accessible from other classes and instances
    public Text textObj;

    void Awake ()
    {
        textObj = GetComponent<Text> (); //get the UI text
        text = "Level 1: Teddy target practice"; //reset text to level 1's
    }

    void Update ()
    {
        textObj.text = text;
    }
}
