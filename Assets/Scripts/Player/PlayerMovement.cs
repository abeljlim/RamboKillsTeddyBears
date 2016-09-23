using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    // A layer mask of ground so that a ray can be cast just at gameobjects on the floor layer
    private int groundLayer;

    // The length of the ray from the camera into the scene
    float cameraRayLength = 100f;          

    private float playerWalkSpeed = 0.5f;
    private Vector3 movement;

    Rigidbody playerRigidbody;

    private Input currentKeyboardState;

    // Use this for initialization
    void Start () {

        groundLayer = LayerMask.GetMask("Ground");
        playerRigidbody = GetComponent<Rigidbody>();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate()
    {
        // Returns the value of the virtual axis identified by axisName with no smoothing filtering applied
        // The value will be in the range -1 and 1 for keyboard and joystick input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Movement(horizontal, 0f, vertical);

        Rotation();
    }

    void Movement(float x, float y, float z)
    {
        movement.Set(x, y, z);

        movement = movement.normalized * playerWalkSpeed;

        playerRigidbody.MovePosition(transform.position + movement);
    }

    void Rotation()
    {
        // Create a ray from camera to the mouse cursor and cast on the ground
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Create a RaycastHit variable to store coordinates about where was hit by the ray
        // If true is returned, hitInfo will contain more information about where the collider was hit 
        RaycastHit groundHit;

        // Public static bool Raycast(Vector3 origin, out RaycastHit hitInfo, float maxDistance = Mathf.Infinity, int layerMask = DefaultRaycastLayers); 
        // origin - the starting point of the ray in world coordinates
        // out RaycastHit hitInfo - If true is returned, hitInfo will contain more information about where the collider was hit
        // float maxDistance - The max distance the rayhit is allowed to be from the start of the ray.
        // int layerMask - A Layer mask that is used to selectively ignore colliders when casting a ray.
        if (Physics.Raycast(cameraRay, out groundHit, cameraRayLength, groundLayer))
        {
            // Create a vector from the player to the point on the floor the raycast from the mouse hit
            // Actually, the result is not the hitpoint, little farther but the direction is the same
            Vector3 playerToHitPoint = groundHit.point - transform.position;

            playerToHitPoint.y = 0f;
            // Create a quaternion (rotation) based on looking down the vector from the player to the mouse.
            Quaternion newRotation = Quaternion.LookRotation(playerToHitPoint);
            
            // Operate Player to Rotate
            playerRigidbody.MoveRotation(newRotation);
        }


    }

}
