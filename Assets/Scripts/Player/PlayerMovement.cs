using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    // A layer mask of ground so that a ray can be cast just at gameobjects on the floor layer
    private int groundLayer;

    // The length of the ray from the camera into the scene
    float cameraRayLength = 100f;          

    private float playerWalkSpeed = 0.25f;
    private Vector3 movement;

    Rigidbody playerRigidbody;

    private Input currentKeyboardState;
    GameObject ground;
    private static Vector3 levelMin, levelMax;
    
    // Use this for initialization
    void Start () {

        ground = GameObject.FindGameObjectWithTag("LevelArea");
        groundLayer = LayerMask.GetMask("Ground");
        levelMin = ground.GetComponent<Collider>().bounds.min;
        levelMax = ground.GetComponent<Collider>().bounds.max;
        playerRigidbody = GetComponent<Rigidbody>();

	}
	
	// Update is called once per frame
	void Update () {

        if (PlayerWeapons.CurrSkill == PlayerWeapons.STIMPACK)
        {
            playerWalkSpeed = 0.5f;
        }
        else
            playerWalkSpeed = 0.25f;
	}

    void FixedUpdate()
    {
        // Returns the value of the virtual axis identified by axisName with no smoothing filtering applied
        // The value will be in the range -1 and 1 for keyboard and joystick input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        if (PlayerWeapons.CurrSkill != PlayerWeapons.BULLETFRENZY)
        {
            //For isometric movement - rotates movement angle to match the camera angle.
            Transform CameraTransform = Camera.main.gameObject.transform;
            float cameraYangle = CameraTransform.rotation.eulerAngles.y / 180 * Mathf.PI; //camera angle in radians
            //Debug.Log ( cameraYangle );
            Vector2 movementVec = new Vector2 ( horizontal, vertical );
            //do rotation, except during bullet frenzy state in which the player would be spinning
            if (!(vertical == 0 && horizontal == 0))
            {
                float angle = Mathf.Atan2(vertical, horizontal);
                angle -= cameraYangle;
                float rotatedY = Mathf.Sin(angle) * movementVec.magnitude;
                float rotatedX = Mathf.Cos(angle) * movementVec.magnitude;
                /*
                Vector3 moveDir;
                if (vertical == 0)
                {
                   moveDir = new Vector3(rotatedX, 0, rotatedY);
                }
                else
                {
                   moveDir = new Vector3(rotatedX, 0, rotatedY);
                }
                transform.rotation = Quaternion.LookRotation(moveDir);*/
                Movement(rotatedX, 0f, rotatedY);
            }

            //Movement ( horizontal, 0f, vertical );
            Rotation();
        }
    }

    void Movement(float x, float y, float z)
    {
        movement.Set(x, y, z);

        movement = movement.normalized * playerWalkSpeed;

        Vector3 direction = movement;
        Vector3 potentialNewPos = transform.position + movement;

        if (potentialNewPos.x < levelMin.x || potentialNewPos.z < levelMin.z || potentialNewPos.x > levelMax.x || potentialNewPos.z > levelMax.z) //disallow doing the movement if it would lead to going out of what would be the ground - as covered by LevelArea
            return;

        //The following raycast is to prevent movement that would go through thin objects and walls (such as hollow walls), and such
        Ray ray = new Ray ( transform.position, direction );
        RaycastHit hit;
        if (!Physics.Raycast ( ray, out hit, direction.magnitude))
        {

            playerRigidbody.MovePosition ( transform.position + movement );
            // Do something if hit
        }
        else
        {
            playerRigidbody.MovePosition ( hit.point ); //go as close as possible to the place hit
        }
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
