using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class EnemyMovement : MonoBehaviour {

    Transform player;

    NavMeshAgent nav;

    //Speed changes depending on the time of day
    //public float daySpeed = 3;
    //public float nightSpeed = 4.5f;
    public float daySpeed;
    public float daySpeedDeviation;
    public bool IsDemo;

    public float maxVelocity
    {
        get
        {
            return usedVelocity * Time.deltaTime;
        }
    }
    public float usedVelocity
    {
        get
        {
            if (PlanetOrbit.isDay)
            {
                return daySpeed;
            }
            else
            {
                return daySpeed * 2;
            }
        }
    }

    EnemyHealth enemyHealth;

	// Use this for initialization
	void Start () {

        if (!IsDemo)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
        nav = GetComponent<NavMeshAgent> ();
        nav.Stop();
        enemyHealth = GetComponent<EnemyHealth> ();

    }

    // Update is called once per frame
    void Update () {
        if (enemyHealth.isDead)
        {
            /*if (nav != null) //already done in the EnemyHealth.Die()
            {
                Destroy ( nav );
            }*/
            //cease movement, and do nothing else if dead
            /*if (nav.speed != 0)
            {
                nav.speed = 0;
                nav.acceleration = 0;
                nav.speed = 0;
            }*/
            return;
        }
        if (IsDemo)
        {
            GameObject DemoDest = GameObject.FindGameObjectWithTag("DemoEnemyDest");
            nav.SetDestination(DemoDest.transform.position);
            //Debug.Log(DemoDest.transform.position);
            Vector3 dist = DemoDest.transform.position - transform.position;
            if (dist.sqrMagnitude < 9)
            {
                Destroy(gameObject);
            }
            nav.speed = daySpeed;
        }
        else
        {
            nav.SetDestination(player.position);
            nav.speed = usedVelocity;
            CrowdPathFollowing();
        }
        //Debug.Log ( "Playerpos: " + player.position );
        //Debug.Log ( "Path:" );
        /*foreach (Vector3 v3 in nav.path.corners) {
            Debug.Log ( v3 );
        }*/

        //Set speed depending on time of day.
        //The later at night, the faster the enemies move.
        //nav.speed = 3.75f + (-PlanetOrbit.isDayAmt) * daySpeedDeviation;

        //non-fuzzy logic approach to speed
        /*if (PlanetOrbit.isDayAmt && nav.speed != daySpeed)
        {
            nav.speed = daySpeed;
        }
        else if (!PlanetOrbit.isDay && nav.speed != nightSpeed)
        {
            nav.speed = nightSpeed;
        }

*/
        // Change speed depending on time of day

    }
    [StructLayout(LayoutKind.Sequential)]
    struct vector3
    {
        public float x;
        public float y;
        public float z;
    }
    [DllImport("DLLTEST", CallingConvention = CallingConvention.Cdecl)]
    private static extern vector3 GetUnNormalizedSeparationVector(int objCount, float[] objectPos_x, float[] objectPos_y, float[] objectPos_z, vector3 currPos); //can't pass reference types? Including string?

    //Non-quadtree method of collision detection for crowd path following:
    public void CrowdPathFollowing()
    {

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); //only avoids enemies as neighbours; could generalize to more obstacles though.
                                                                           //could make it include all non-player and non-self GameObjects, avoiding any only in terms of x and z though
                                                                           //find neighbours
        int neighbourCount = 0;
        Vector3 totalMass = Vector3.zero;

        //int enemyCountPow2 = (int)(Mathf.Ceil(Mathf.Log(enemies.Length, 2))); //smallest power of 2 that would fit enemies.Length
        //int totalSize = (int)(Mathf.Pow(2, enemyCountPow2));
        Profiler.BeginSample("AddToBuffersForGetUnNormalized...C++Code"); //the code - to avoid this <<copying><YKWIM>>, may have to somehow point to each vector and its respective x, y, and z values through the GameObject[] array, where the same order could be stored between these ... for the enemies ...
                                                                          //can figure out the memory locations of each ... well, variable ... for the class ... well, could get the difference in memory address for one relative to the beginning of the class memory allocation or such, dereference such, and then get the memory locations of each of the variables
                                                                          //and then use those respective offsets
                                                                          //however, these would still be necessary to copy to the buffer for OpenCL ...
                                                                          //so, to handle this ... maybe the tree for doing collision detection may be what would be intended in parallelizing collision detection - with noting how collision detection would not really involve much to be parallelized ... ie. each thing to be parallelized would take not that many instructions, with comparability to setting up a buffer ...
                                                                          //so, parallelizing may be for things that require a good amount of computation ... in parallelizing such relative to each position to compare to ...
                                                                          //maybe could update this buffer globally, rather than per enemy. So, would basically update each enemyPosBuf for the respective enemy, each having its own ID, on each frame, each enemy having its own ID
                                                                          //or, could use a List, which could be used ...well, would need to handle such contiguous memory ...
                                                                          //and would have to check to see that the enemy position would still be a valid one, in tracking such ... well, could always move a later enemy <or not move anything if there would not be anything in front of the current enemy> on the destroy() code of the buffer, and update the count of the buffer ...
                                                                          //and update the enemy buffer ID for the moved enemy as well, with keeping track of the ID of the enemy that would be at the end of <removed here: he bu>the buffer
                                                                          //So, it would be O(1) with such copying of another enemy to the current position, and O(n) <<across all enemies><YKWIM>>, rather than O(n^2)<<noting following " for updating" writing being written later, and this curr. "<<noting following...>...>" angled bracketed writing being written later: >< - note this is as I understand it to mean>> for updating
                                                                          //and alternatively, could construct a tree <removed here: in OpenCL >with said enemies' maint<removed here: a>enance of the tree
                                                                          //int[] neighbourCountBuf = new int[enemies.Length]; //<noted at 7:<removed here: 45>24-25PM<removed here: : never used>, as of 12/10/16 ...: never used>
        float[] enemyPosBuf_x = new float[enemies.Length];
        float[] enemyPosBuf_y = new float[enemies.Length];
        float[] enemyPosBuf_z = new float[enemies.Length];
        //vector3[] enemyVecBuf = new vector3[enemyCountPow2]; //all elements initialized to 0 by default
        int i = 0;
        foreach (GameObject enemy in enemies)
        {
            enemyPosBuf_x[i] = enemy.transform.position.x;
            enemyPosBuf_y[i] = enemy.transform.position.y;
            enemyPosBuf_z[i] = enemy.transform.position.z;
            i++;
        }
        Profiler.EndSample();

        vector3 currPos = new vector3 { x = transform.position.x, y = transform.position.y, z = transform.position.z };
        Profiler.BeginSample("C++Code");
        vector3 SeparationVec = /*new vector3 { x = 0.01f, y = 0.01f, z = 0.01f };  */GetUnNormalizedSeparationVector(enemies.Length, enemyPosBuf_x, enemyPosBuf_y, enemyPosBuf_z, currPos);
        Profiler.EndSample();

        Vector3 SeparationVecNorm = new Vector3(SeparationVec.x, SeparationVec.y, SeparationVec.z).normalized;
        //if (SeparationVecNorm.magnitude > 0)
        //    Debug.Log(SeparationVecNorm.x + ", " + SeparationVecNorm.y + ", " + SeparationVecNorm.z);

        Profiler.BeginSample("SerialTracking");
        //foreach (GameObject otherEnemy in enemies) //this loop as being something that can be done in parallel, or basically tracking all of such that can be done in parallel - so, adding corresponding parallel code for this ... from OPAS2 and median_filterlab
        //{
        //    Vector3 enemyPos = otherEnemy.transform.position;
        //    if ((enemyPos - transform.position).magnitude <= 2) //if otherEnemy is a neighbour
        //    {
        //        totalMass += enemyPos;
        //        neighbourCount++;
        //    }
        //}

        //get center of mass - position to avoid or flee from
        //Vector3 CenterOfMass = totalMass / neighbourCount;
        //SeparationVecNorm = transform.position - CenterOfMass;
        //SeparationVecNorm.Normalize();
        Profiler.EndSample();

        //add this as 1/3 the magnitude of path following weight
        //Profiler.BeginSample("PathFollowingVec()");


        //Vector3 pathFollowVec = PathFollowingVec();
        //Profiler.EndSample();

        NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
        Vector3 SeekVec_BeforeSeparation = (navMeshAgent.destination - transform.position).normalized * maxVelocity;
        Vector3 MovementWeightedSum = SeekVec_BeforeSeparation + SeparationVecNorm * SeekVec_BeforeSeparation.magnitude / 3/*pathfollowingvec + SeparationDir * pathfollowmagnitude / 3*/;

        //Debug.Log("orientation: " + transform.rotation.eulerAngles);
        //Debug.Log("position: " + transform.position);
        //transform.position += PathFollowingVec();
        //transform.rotation.SetEulerRotation(new Vector3(0,orientation,0));
        //transform.position.y = 3;
        //Vector3 rotatedForwardVec = velocity.normalized;
        //Vector3 YAxisVec = new Vector3(0, -1, 0);
        //float halfPI = Mathf.PI / 2;
        //Vector3 rotatedForwardVecAxis = /*rotatedForwardVec * Mathf.Cos(halfPI) +*/ Vector3.Cross(YAxisVec, rotatedForwardVec)/* * Mathf.Sin(halfPI)*//* + YAxisVec * (YAxisVec.magnitude * rotatedForwardVec.magnitude * Mathf.Cos(Mathf.PI / 2)) * (1 - Mathf.Cos(Mathf.PI / 2))*/; //rotatedForwardVec rotated by 90 degrees ... CCW? about the Y axis ...
        //Vector3 rotatedForwardVec2 = /*rotatedForwardVec * Mathf.Cos(halfPI) +*/ (Vector3.Cross(rotatedForwardVecAxis, rotatedForwardVec)/* * Mathf.Sin(halfPI)*/); //rotated about 90 degrees CCW? About axis ...
        //Debug.Log("rotatedForwardVec2: "+rotatedForwardVec2);
        transform.forward = MovementWeightedSum; //affects position as well
                                                 //transform.position = transform.position + new Vector3(0, -3, 0);
                                                 //transform.position = transform.position + new Vector3(0, 3, 0);
        transform.position += MovementWeightedSum;
        //Debug.Log("Crowd Path Following pos change: " +MovementWeightedSum);

    }
}
