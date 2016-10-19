using UnityEngine;
using System.Collections;

public class PlanetOrbit : MonoBehaviour
{
    public float SecondsInDay = 30f;

    //Determine whether it's day or night based on whether the sun is above the position 0
    //Could also use fuzzy logic for this.
    public static bool isDay
    {
        get { 
            Transform groundTransform = GameObject.FindGameObjectWithTag ( "LevelArea" ).transform;
            float sunHeight = GameObject.FindGameObjectWithTag ( "Sun" ).transform.position.y;
            return sunHeight > groundTransform.position.y;
        }
    }
    //almost fuzzy logic isDay implementation; returns a value from -1 to 1, with -1 to 0 being night
    public static float isDayAmt
    {
        get {
            float sunHeight = GameObject.FindGameObjectWithTag ( "Sun" ).transform.position.y;
            return sunHeight / maxSunHeight;
        }
    }
    //fuzzy logic isDay implementation; returns a value from -1 to 1.
    /*public static float isDay
    {
    get {
        float sunHeight = GameObject.FindGameObjectWithTag ( "Sun" ).transform.position.y;
        //float minSunHeight = -maxSunHeight;
        float normalizedSunHeight = sunHeight + maxSunHeight; // or sunHeight - (-maxSunHeight)
        float normMaxSunHeight = maxSunHeight * 2;
        return normalizedSunHeight / normMaxSunHeight;
    }
}*/
    public static float maxSunHeight;

    // Use this for initialization
    void Start ()
    {
        if (tag == "Sun")
        {
            //set as the maximum sun height
            maxSunHeight = 500; //the maximum time of day
        }
    }

    // Update is called once per frame
    void Update ()
    {

    }
    void FixedUpdate ()
    {
        float rotAngle = 360f / SecondsInDay * Time.deltaTime; //angle to rotate in degrees during the time of the frame
        //Orbit around the x-axis, at 0,0,0, in the scene.
        transform.RotateAround ( Vector3.zero, Vector3.right, rotAngle );
        transform.LookAt ( Vector3.zero );
    }
}