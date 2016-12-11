using UnityEngine;
using System.Collections;
using System.IO;
//using Cloo;
//using Cloo.Bindings;
//using ManOCL;
//using ManOCL.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Kernel
{
    public class EnemyMovement : MonoBehaviour
    {

        Transform player;

        NavMeshAgent nav;

        //Speed changes depending on the time of day
        //public float daySpeed = 3;
        //public float nightSpeed = 4.5f;
        public float daySpeed = 3.75f;
        public float daySpeedDeviation = 0.75f;

        EnemyHealth enemyHealth;


        public Vector3 velocity; //in pixels per 5 frames
        public Vector3 acceleration; //in velocity units per frame
        float maxAcceleration = 0.3f, maxVelocity = 1f;
        Vector3 steeringAccel = Vector3.zero; //equal to the acceleration in this assignment
        public float orientation //orientation - angle that the velocity vector would be
        {
            get { return (float)(Mathf.Atan2(velocity.z, velocity.x)); } //return the current rotation, based on the velocity
        }
        static bool notRun = true;

        // Use this for initialization
        void Start()
        {

            player = GameObject.FindGameObjectWithTag("Player").transform;
            nav = GetComponent<NavMeshAgent>();
            nav.Stop();
            enemyHealth = GetComponent<EnemyHealth>();

            //Test OpenCL on this
            if (notRun)
            {
                notRun = false;
                Debug.Log("Running once?");
                runOnce();
            }
        }

        // Update is called once per frame
        void Update()
        {
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
            Profiler.BeginSample("PathFollowStuff");
            nav.SetDestination ( player.position );
            //Debug.Log ( "Playerpos: " + player.position );
            //Debug.Log ( "Path:" );
            /*foreach (Vector3 v3 in nav.path.corners) {
                Debug.Log ( v3 );
            }*/

            //Set speed depending on time of day.
            //The later at night, the faster the enemies move.
            //nav.speed = 3.75f + (-PlanetOrbit.isDayAmt) * daySpeedDeviation;

            CrowdPathFollowing();


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
            Profiler.EndSample();
        }


        //Code for getting Morton codes for a LBVH.
        //From https://devblogs.nvidia.com/parallelforall/thinking-parallel-part-iii-tree-construction-gpu/

        // Expands a 10-bit integer into 30 bits
        // by inserting 2 zeros after each bit.
        uint expandBits(uint v)
        {
            v = (v * 0x00010001u) & 0xFF0000FFu;
            v = (v * 0x00000101u) & 0x0F00F00Fu;
            v = (v * 0x00000011u) & 0xC30C30C3u;
            v = (v * 0x00000005u) & 0x49249249u;
            return v;
        }

        // Calculates a 30-bit Morton code for the
        // given 3D point located within the unit cube [0,1] - ie. 'normalized' 3D point
        uint morton3D(float x, float y, float z)
        {
            x = Mathf.Min(Mathf.Max(x * 1024.0f, 0.0f), 1023.0f);
            y = Mathf.Min(Mathf.Max(y * 1024.0f, 0.0f), 1023.0f);
            z = Mathf.Min(Mathf.Max(z * 1024.0f, 0.0f), 1023.0f);
            uint xx = expandBits((uint)x);
            uint yy = expandBits((uint)y);
            uint zz = expandBits((uint)z);
            return xx * 4 + yy * 2 + zz;
        }


        //Can test path following with bounding spheres, implementing it that way, and use velocity matching along the path as done in my COMP8901 Assignment 1.
        //Crowd path following - http://www.red3d.com/cwr/steer/CrowdPath.html
        //Separation - from https://gamedevelopment.tutsplus.com/tutorials/3-simple-rules-of-flocking-behaviors-alignment-cohesion-and-separation--gamedev-3444

        //OpenCL functions
        /*
         cl_device_id *SetupGPUDevice(cl_platform_id platformIds)
        {
        cl_device_id *deviceIds;
        cl_uint numDevices;

        //check the device and get the size of the device buffer.  
        status = clGetDeviceIDs(platformIds, CL_DEVICE_TYPE_GPU, 0, NULL, &numDevices);

        if (status != CL_SUCCESS) {
            printf("Failed to get GPU Device");
            exit(1);
        }

        printf("GPU Device Numbers: %d\n", numDevices);

        deviceIds = (cl_device_id *)malloc(numDevices * sizeof(cl_device_id));
        clGetDeviceIDs(platformIds, CL_DEVICE_TYPE_GPU, numDevices, deviceIds, NULL);

        return deviceIds;
        }
         void DeviceInfo(cl_platform_id platformIds, cl_device_id deviceIds)
        {
            size_t ext_size = 0;
            //platform and device info for GPU
            clGetPlatformInfo(platformIds, CL_PLATFORM_NAME, 0, NULL, &ext_size);
            char *name = (char*)malloc(ext_size);
            clGetPlatformInfo(platformIds, CL_PLATFORM_NAME, ext_size, name, NULL);
            printf("Platform Name: %s\n", name);

            clGetDeviceInfo(deviceIds, CL_DEVICE_NAME, 0, NULL, &ext_size);
            char *device = (char*)malloc(ext_size);
            clGetDeviceInfo(deviceIds, CL_DEVICE_NAME, ext_size, device, NULL);
            printf("Device Name: %s\n", device);

            printf("--------------------------------- \n");
        }

        cl_context GenerateContext(cl_device_id *deviceIds) //make context for the device with deviceIds as the device ID?
        {
            cl_context context;

            context = clCreateContext(NULL, 1, deviceIds, NULL, NULL, NULL);

            if (context == NULL){
                printf("Failed to create context");
                exit(1);
            }

            return context;
        }
 
        void ContextInfo(cl_device_id deviceIds)
        {
            size_t ext_size = 0;

            clGetDeviceInfo(deviceIds, CL_DEVICE_NAME, 0, NULL, &ext_size);
            char *device = (char*)malloc(ext_size);
            clGetDeviceInfo(deviceIds, CL_DEVICE_NAME, ext_size, device, NULL);
            printf("Device for Current Context: %s\n", device);

            printf("--------------------------------- \n");
        }
        cl_command_queue GenerateCommandQueue(cl_context context, cl_device_id deviceIds)
        {
            cl_command_queue commandQueue;
            commandQueue = clCreateCommandQueue(context, deviceIds, 0, NULL); //create command queue for GPU

            if (commandQueue == NULL) {
                printf("Failed to create Command Queue");
                exit(1);
            }
	
            return commandQueue;
        }

        cl_program ProgramObject(const char *filePath, cl_context context)
        {
            cl_program program;
            char *source;
            size_t sourceLength;

            source = oclLoadProgSource(filePath,"",&sourceLength);
            if (source == NULL) {
                printf("Error in oclLoadProgSource");
                exit(1);
            }

            program = clCreateProgramWithSource(context, 1, (const char **)&source, &sourceLength, &status);

            if (status != CL_SUCCESS) {
                printf("Error in CreateProgramWithSource");
                exit(1);
            }

            return program;
        }

        void Compiler(cl_program program)
        {
            status = clBuildProgram(program, 0, NULL, NULL, NULL, NULL);

            if (status != CL_SUCCESS) {
                printf("Compiler is failed");
                exit(1);
            }
        }

        cl_kernel KernelMemory(cl_context context, cl_program program)
        {

            cl_kernel kernel;

            kernel = clCreateKernel(program, "TestOpenCL", NULL);
            if (kernel == NULL) {
                printf("Error: Can not create kernel \n");
                exit(1);
            }

            return kernel;
        }

        void KernelCompiler(cl_context context, cl_kernel kernel) //Compile kernel?
        {
            int i = 0;

            //Make R\W buffers? For the current kernel for the current context? For what device
            memoryBuffer[0] = clCreateBuffer(context, CL_MEM_READ_ONLY | CL_MEM_COPY_HOST_PTR, arraySize * sizeof(float), inPut_a, NULL);
            memoryBuffer[1] = clCreateBuffer(context, CL_MEM_READ_ONLY | CL_MEM_COPY_HOST_PTR, arraySize * sizeof(float), inPut_b, NULL);
            memoryBuffer[2] = clCreateBuffer(context, CL_MEM_WRITE_ONLY | CL_MEM_COPY_HOST_PTR, arraySize * sizeof(float), result, NULL);

            //Handle if a memoryBuffer element would be null - print an error...
            for each (cl_mem memObjects in memoryBuffer)
            {
                if (memObjects == NULL) {
                    printf("Error %d : creating memory objects \n", i);
                    exit(1);
                }

                i++;
            }

            //set the kernel arguments to be *arg_value <- in this case, with memoryBuffer[...] being the *arg_value>
            //kernel arguments being for 
            status = clSetKernelArg(kernel, 0, sizeof(cl_mem), &memoryBuffer[0]);  
            status |= clSetKernelArg(kernel, 1, sizeof(cl_mem), &memoryBuffer[1]);
            status |= clSetKernelArg(kernel, 2, sizeof(cl_mem), &memoryBuffer[2]);

            if (status != CL_SUCCESS) {
                printf("Error in clSetKernelArg \n");
                exit(1);
            }
        }
        void Debuging(cl_command_queue commandQueue, cl_kernel kernel)
        {
            size_t globalWorkSize[1] = { arraySize };
            size_t localWorkSize[1] = { 1 };

            //queue the kernel up for execution across the array  
            status = clEnqueueNDRangeKernel(commandQueue, kernel, 1, NULL, globalWorkSize, localWorkSize, 0, NULL, NULL);
            if (status != CL_SUCCESS) {
                printf("Error in clEnqueueNDRangeKernel \n");
                exit(1);
            }

            //read the output buffer back to the Host  
            status = clEnqueueReadBuffer(commandQueue, memoryBuffer[2], CL_TRUE, 0, arraySize * sizeof(float), result, 0, NULL, NULL);
            if (status != CL_SUCCESS) {
                printf("Error in clEnqueueReadBuffer \n");
                exit(1);
            }
        }
         */

        public void GetNeighbourMassAndCount(ref int neighbourCount, ref Vector3 totalMass)
        {

            //code done in parallel
            /*
     
            const char* filePath = "Test.cl";

            cl_platform_id *platformIds;
            cl_device_id *deviceIds;
            cl_context context;
            cl_command_queue commandQueue;
            cl_program program;
            cl_kernel kernel;

            //series
            platformIds = GeneratePlatform();
            PlatformInfo(platformIds);
     
            //openCL - GPU
            cl_command_queue commandQueue_GPU, commandQueue_CPU;
            cl_kernel kernel_GPU, kernel_CPU;

            //set up OpenCL code on GPU
            deviceIds = SetupGPUDevice(platformIds[1]); //includes a malloc of a cl_device_id to deviceIds, and some printing of the number of devices in the platform ...
            DeviceInfo(platformIds[1], deviceIds[0]); //assign device to platform?
            context = GenerateContext(deviceIds); //generate context for device?
            ContextInfo(deviceIds[0]); //retrieve and print out info of deviceIds[0] - ie. that device

            //
            commandQueue_GPU = GenerateCommandQueue(context, deviceIds[0]); //noting commandQueue ... calls clCreateCommandQueue <with error handling> //being used to enqueue commands? For this current device at index 0
            program = ProgramObject(filePath, context); //set up kernel - Test.cl - for this current device
            Compiler(program); //calls clBuildProgram, which builds a program executable?
            kernel_GPU = KernelMemory(context, program); //calls  clCreateKernel for this - creating a kernel, ie. a function <removed here: called>declared within a program
            KernelCompiler(context, kernel_GPU); //looking at ...
    
            //run kernel stuff - the TestOpenCL function - on each 'processing element' in parallel?
            Debuging(commandQueue_GPU, kernel_GPU); //call clEnqueueNDRangeKernel and clEnqueueReadBuffer to enqueue kernel commands to the command queue for the given buffer <<with the respective work size><YKWIM>>

            printf("Miracle !! No Error !! \n");
            printf("--------------------------------- \n");
            printf("\nTest: a * b = c \n\n");

            printf("%f * %f = %f \n\n", inPut_a[0], inPut_b[0], result[0]);
             */
            //CommandQueue d = new CommandQueue();
            //Context
        }


        static System.Random rand = new System.Random();

        static System.Single[] RandomArray(System.Int32 length)
        {
            System.Single[] result = new System.Single[length];

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (System.Single)rand.NextDouble();
            }

            return result;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct vector3
        {
            public float x;
            public float y;
            public float z;
        }
        [StructLayout(LayoutKind.Sequential)]
        struct strng
        {
            public int size;
            public char[] chars;
        }


        [DllImport("DLLTEST", CallingConvention = CallingConvention.Cdecl)]
        private static extern vector3 GetUnNormalizedSeparationVector(int objCount, float[] objectPos_x, float[] objectPos_y, float[] objectPos_z, vector3 currPos); //can't pass reference types? Including string?

        public static void runOnce()
        {
            /*
            string clSource = @"kernel void K(){}";
            ComputeContextPropertyList cpl = new ComputeContextPropertyList(ComputePlatform.Platforms[0]);
            ComputeContext context = new ComputeContext(ComputeDeviceTypes.Default, cpl, null, System.IntPtr.Zero);
            ComputeProgram program = new ComputeProgram(context, clSource);
            ComputeKernel kernel = program.CreateKernel("K");*/
            StringBuilder errorMsg = new StringBuilder(1024);
            //vector3 outVec = GetUnNormalizedSeparationVector(1, 2, 2, 2, errorMsg);
            //Debug.Log(outVec.x+", "+outVec.y+", "+outVec.z);
            //Debug.Log(errorMsg.ToString());

            /*//code for 

            DeviceGlobalMemory output = new System.Byte[4096];

            DeviceGlobalMemory indeces = RandomArray(102400);
            DeviceGlobalMemory ops = new System.Byte[3072];
            DeviceGlobalMemory data = RandomArray(1048576);

            Debug.Log("Creating kernel...");

            ManOCL.Kernel kernel = ManOCL.Kernel.Create("Kernel", System.IO.File.ReadAllText("Test.c"), data, indeces, ops, output);

            Debug.Log("Executing kernel...");

            ManOCL.Event e = kernel.Execute(256, 256);

            kernel.CommandQueue.Finish();

            Debug.Log("done, operation took " + ManOCL.Profiler.DurationSeconds(e));

            UnmanagedReader reader = new UnmanagedReader(new DeviceBufferStream(output));

            for (int i = 0; i < 256; i++)
            {
                if (i % 4 == 0) Debug.Log("");
                if (i % 16 == 0) Debug.Log("");

                Debug.Log(reader.Read<System.Single>()+"\t");
            }
            */
        }
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
            /*string s = "";
            for(int i2=0; i2<enemyPosBuf_x.Length; i2++)
            {
                s += enemyPosBuf_x[i2] + ", ";
            }*/
            //Debug.Log(s);
            //StringBuilder errorMsg = new StringBuilder(1024);
            //transform.position.x = 0;
            vector3 currPos = new vector3 {x = transform.position.x, y = transform.position.y, z = transform.position.z };
            Profiler.BeginSample("C++Code");
            vector3 SeparationVec = /*new vector3 { x = 0.01f, y = 0.01f, z = 0.01f };  */GetUnNormalizedSeparationVector(enemies.Length, enemyPosBuf_x, enemyPosBuf_y, enemyPosBuf_z, currPos);
            Profiler.EndSample();
            //vector3 SeparationVec = new Kernel.EnemyMovement.vector3 { x = 0, y = 0, z = 0};
            //Debug.Log(SeparationVec.x + ", " + SeparationVec.y + ", " + SeparationVec.z);
            //vector3 SeparationVecTest = GetUnNormalizedSeparationVector(enemies.Length, enemyPosBuf_x, enemyPosBuf_y, enemyPosBuf_z, currPos, errorMsg);
            //Debug.Log(errorMsg.ToString());
            //Debug.Log("SeparationVecTest: " + SeparationVecTest.x + ", " + SeparationVecTest.y + ", " + SeparationVecTest.z);
            Vector3 SeparationVecNorm = new Vector3(SeparationVec.x, SeparationVec.y, SeparationVec.z).normalized;
            //GetNeighbourMassAndCount()
            /*
            clCreateProgramWithSource()
            */

            Profiler.BeginSample("SerialTracking"); 
           foreach (GameObject otherEnemy in enemies) //this loop as being something that can be done in parallel, or basically tracking all of such that can be done in parallel - so, adding corresponding parallel code for this ... from OPAS2 and median_filterlab
           {
               Vector3 enemyPos = otherEnemy.transform.position;
               if ((enemyPos - transform.position).magnitude <= 2) //if otherEnemy is a neighbour
               {
                   totalMass += enemyPos;
                   neighbourCount++;
               }
           }

           //get center of mass - position to avoid or flee from
           Vector3 CenterOfMass = totalMass / neighbourCount;
           SeparationVecNorm = transform.position - CenterOfMass;
           SeparationVecNorm.Normalize();
            Profiler.EndSample();
           
            //add this as 1/3 the magnitude of path following weight
            //Profiler.BeginSample("PathFollowingVec()");


            //Vector3 pathFollowVec = PathFollowingVec();
            //Profiler.EndSample();

            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
            Vector3 SeekVec_BeforeSeparation = (navMeshAgent.destination - transform.position).normalized * (maxVelocity / 3);
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

        public Vector3 PathFollowingVec2()
        {

            //create path
            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
            NavMeshPath navMeshPath = new NavMeshPath();

            Profiler.BeginSample("Pathfinding");
            navMeshAgent.CalculatePath(player.position, navMeshPath);
            Profiler.EndSample();
            Vector3[] goalPath = navMeshPath.corners;
            Vector3 updatePosVec = Vector3.zero;
            if (goalPath.Length > 0)
            {
                Vector3 currTarget = new Vector3(goalPath[1].x, transform.position.y, goalPath[1].z);
                Vector3 desiredVelocity = (currTarget - transform.position).normalized * maxVelocity; //to be towards an object

                //velocity matching - go towards the difference to the desired velocity as soon as possible
                if (velocity != desiredVelocity)
                {

                    if ((desiredVelocity - velocity).magnitude <= maxAcceleration) //with velocity and to be updated to at most the difference between the velocity and desiredVelocity, not over-updating
                    {
                        steeringAccel = desiredVelocity - velocity;
                    }
                    else
                    { //update by a fraction of the amount to update, if unable to accelerate as fast as the difference between desiredVelocity and velocity
                        steeringAccel = (desiredVelocity - velocity) * (float)(Mathf.Min((desiredVelocity - velocity).magnitude, maxAcceleration));
                    }

                    //Debug.Log("works; steeringAccel: "+steeringAccel);
                }
                else
                {
                    steeringAccel = Vector3.zero;
                }
                acceleration = steeringAccel; //the only acceleration, in this case.


                //update code

                //Debug.Log("works; steeringAccel: " + steeringAccel);
                //path following update stuff
                updatePosVec = velocity / 5;
                transform.position += velocity / 5; //apply the velocity from the previous frame leading up to the current position before applying the acceleration (although this leaves out the velocities that would be in between the previous frame and in the current frame)
                velocity += acceleration;
                if (velocity.magnitude > 10)
                {
                    velocity.Normalize();
                    velocity *= 10;
                }
                //AIObj.Update ();

            }
            return Vector3.zero;
        }

        public Vector3 PathFollowingVec()
        {
            //create path
            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
            NavMeshPath navMeshPath = new NavMeshPath();

            //float currYpos = transform.position.y;
            //Debug.Log(currYpos);

            Profiler.BeginSample("Pathfinding");
            navMeshAgent.CalculatePath(player.position, navMeshPath);
            Profiler.EndSample();
            //transform.position = new Vector3(transform.position.x, currYpos, transform.position.z);
            //traverse navMeshPath vertices - at least their x,z coord values
            //keep going to the nearest or first corner found in the current calculated path, since there is a new path calculated on each frame
            Vector3[] goalPath = navMeshPath.corners;
            Vector3 updatePosVec = Vector3.zero;
            if (goalPath.Length > 0)
            {
                /*
                    Node currNode = goalPath[CurrPathPoint];
                    Vector2 currStartPosn = ToPixels ( currNode.row, currNode.col ) + new Vector2 ( TileWidth / 2, TileHeight / 2 );
                    if (!(currNode.row == goalRow && currNode.col == goalCol)) //If not yet at the goal
                    {
                        Node nextNode = goalPath[CurrPathPoint + 1]; //next point is the target



                        //See if past the point with respect to the direction intended. This is by using the vector from the last reached path node (currPathNode) to the current position, and projecting it onto the vector from the last reached node to the next path node (nextPathNode), and seeing if the projected vector would be greater than the difference between the last to next path node. If so, then this has reached the next path node (or in other words, the next point).
                        Node currPathNode = goalPath[CurrPathPoint];
                        Node nextPathNode = goalPath[CurrPathPoint + 1];
                        Vector2 CurrPathPos = ToPixels ( currPathNode.row, currPathNode.col ) + new Vector2 ( TileWidth / 2, TileHeight / 2 );
                        Vector2 NextPathPos = ToPixels ( nextPathNode.row, nextPathNode.col ) + new Vector2 ( TileWidth / 2, TileHeight / 2 );

                        Vector2 CurrToNextVect = NextPathPos - CurrPathPos;
                        Vector2 CurrPathPosToCurrPosVect = AIObj.position - CurrPathPos;
                        Vector2 NextVect = NextPathPos - CurrPathPos;

                        Vector2 DistanceAlongPath = Vector2.Dot ( CurrPathPosToCurrPosVect, CurrToNextVect ) / CurrToNextVect.Length () * (CurrToNextVect / CurrToNextVect.Length ()); //CurrPathPosToCurrPosVect vector projected onto CurrToNextVect. In other words, this is the vector from currPathNode to the closest matching interpolated point on the path, which would be a polyline connecting all of the nodes.


                    
                        Vector2 InterpolatedTargetAlongPath;
                        if (CurrPathPoint + 2 < goalPath.Count) //If there's a node 2 nodes ahead, then can determine this as the next target, and determine the progress towards that target (equal to the progress seen in DistanceAlongPath) to move towards.
                        {
                            Node PathNode2Next = goalPath[CurrPathPoint + 2];
                            Vector2 PathPos2Next = ToPixels ( PathNode2Next.row, PathNode2Next.col ) + new Vector2 ( TileWidth / 2, TileHeight / 2 );
                            Vector2 NextTo2Next = PathPos2Next - NextPathPos;

                        //Determining the target as a point along a polyline between the next node and the node after that by using the distance along the path and adding the vector from the current to next node.
                            InterpolatedTargetAlongPath = NextPathPos + NextTo2Next / NextTo2Next.Length () * DistanceAlongPath.Length (); //add the additional component with respect to the path
                        }
                        else
                        {
                            //If there is only one unreached node left, determining the target as a point along a polyline in the direction of from the last reached node and the next node.
                            InterpolatedTargetAlongPath = NextPathPos + DistanceAlongPath; //add the additional component with respect to the path
                        }
                        Vector2 currTarget = InterpolatedTargetAlongPath;


                        //if the component is larger than the current point, and it's not in the opposite direction (there are only 2 directions), then this has reached the next point.
                        bool isLarger = DistanceAlongPath.LengthSquared () > CurrToNextVect.LengthSquared ();
                        bool sameDirection = (DistanceAlongPath + CurrToNextVect).LengthSquared () > (DistanceAlongPath).LengthSquared ();
                        if (isLarger && sameDirection) //if the AIObject's position has reached the next point
                        {
                            CurrPathPoint++;
                        }
                        */
                //For project - testing: velocity matching towards the first corner as the target
                Vector3 currTarget = new Vector3(goalPath[1].x, transform.position.y, goalPath[1].z);
                //Vector2 currTarget2D = new Vector2(currTarget.x, currTarget.z) //noting y as z

                //seek code
                //AIObj.seek ( currTarget ); //steering towards matching velocity towards the target
                //put update and seek together        

                Vector3 desiredVelocity = Vector3.Normalize(currTarget - transform.position) * maxVelocity; //to be towards an object

                //velocity matching - go towards the difference to the desired velocity as soon as possible
                if (velocity != desiredVelocity)
                {
                    if ((desiredVelocity - velocity).magnitude <= maxAcceleration) //with velocity and to be updated to at most the difference between the velocity and desiredVelocity, not over-updating
                    {
                        steeringAccel = desiredVelocity - velocity;
                    }
                    else
                    { //update by a fraction of the amount to update, if unable to accelerate as fast as the difference between desiredVelocity and velocity
                        steeringAccel = Vector3.Normalize(desiredVelocity - velocity) * (float)(Mathf.Min((desiredVelocity - velocity).magnitude, maxAcceleration));
                    }

                }
                else
                {
                    steeringAccel = Vector3.zero;
                }
                acceleration = steeringAccel; //the only acceleration, in this case.


                //update code

                //path following update stuff
                updatePosVec = velocity / 5;
                transform.position += velocity / 5; //apply the velocity from the previous frame leading up to the current position before applying the acceleration (although this leaves out the velocities that would be in between the previous frame and in the current frame)
                velocity += acceleration;
                //Debug.Log(acceleration);
                if (velocity.magnitude > maxVelocity)
                {
                    velocity.Normalize();
                    velocity *= maxVelocity;
                }
                //AIObj.Update ();
            }
            return updatePosVec;
        }

        /* COMP8901 Asst1 path following code
         * could change the Update() function in this code to UpdatePhysics()
         * //Get AI to seek the current target via velocity matching.
         * 
         * below <<up to where I mention AIObject.cs is in Game1.cs><YKWIM>>
         * */
        /*
            
         * */
        /*
         * 
         * below is in AIObject.cs
     
            public Vector2 position;
            public Vector2 velocity; //in pixels per 5 frames
            public Vector2 acceleration; //in velocity units per frame
            float maxAcceleration = 1.5f, maxVelocity = 10;
            Vector2 steeringAccel; //equal to the acceleration in this assignment
            public float orientation //orientation - angle that the velocity vector would be
            {
                get { return (float)(Math.Atan2 ( velocity.Y, velocity.X )); } //return the current rotation, based on the velocity
            }

            public AIObject (Vector2 position)
            {
                this.position = position;
            }


            /// <summary>
            /// Following http://www.red3d.com/cwr/steer/SeekFlee.html regarding Seek.
            /// </summary>
            /// <param name="target"></param>
            public void seek (Vector2 target)
            {
                Vector2 desiredVelocity = Vector2.Normalize(target - position) * maxVelocity; //to be towards an object

                //velocity matching - go towards the difference to the desired velocity as soon as possible
                if (velocity != desiredVelocity) {
                    if ((desiredVelocity - velocity).Length() <= maxAcceleration) //with velocity and to be updated to at most the difference between the velocity and desiredVelocity, not over-updating
                    {
                        steeringAccel = desiredVelocity - velocity;
                    }
                    else { //update by a fraction of the amount to update, if unable to accelerate as fast as the difference between desiredVelocity and velocity
                        steeringAccel = Vector2.Normalize ( desiredVelocity - velocity ) *(float) (Math.Min((desiredVelocity - velocity).Length(), maxAcceleration));
                    }
                }
                else
                {
                    steeringAccel = Vector2.Zero;
                }
                acceleration = steeringAccel; //the only acceleration, in this case.
            }

            public void Update ()
            {
                position += velocity / 5; //apply the velocity from the previous frame leading up to the current position before applying the acceleration (although this leaves out the velocities that would be in between the previous frame and in the current frame)
                velocity += acceleration;
                if (velocity.Length() > 10)
                {
                    velocity.Normalize ();
                    velocity *= 10 ;
                }
            }
         */
    }
}