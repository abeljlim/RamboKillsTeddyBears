using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public Transform target;
    public float smoothing = 5f;

    private Vector3 offset;
	// Use this for initialization
	void Start () {

        offset = transform.position - target.position;
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        Vector3 targetCameraPosition = target.position + offset;
        transform.position = targetCameraPosition;
    }

    //debug - testing targeting of sun
    /*void FixedUpdate ()
    {
        transform.LookAt ( new Vector3 ( target.position.x, target.position.y, target.position.z ) );
    }*/
}
