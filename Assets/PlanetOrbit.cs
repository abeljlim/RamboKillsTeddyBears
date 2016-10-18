using UnityEngine;
using System.Collections;

public class PlanetOrbit : MonoBehaviour
{
    public float SecondsInDay = 30f;


    // Use this for initialization
    void Start ()
    {

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