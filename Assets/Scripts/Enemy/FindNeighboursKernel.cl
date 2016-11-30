// OpenCL Kernel Function  
__kernel void FindNeighboursKernel(__global const float *t1, __global const float *t2, __global float *result)
{
	//add to result buffer at this current id
	int iGID = get_global_id(0);

	// elements operation - perform t1 + t2 and get the sum, result, and store it into the respective buffer location in the result buffer at this level. Initialized buffer elements are -1, and so only perform this if both elements are initialized; otherwise, wait for the elements to be ready. Maybe use a mutex or semaphore to keep track of the elements. Or, could use barrier before continuing to the next layer ... where on each loop iteration, the ids would be halved, and the remaining half would continue execution on a new array ...
	//could make t2 the second half of the work group ... rather than alternating ... so indices would not have to change
	//noting iterating through half the size of the previous work group size each time, getting element 0 and workSize / 2
	//with workSize/2 - 1 being the last element of the first half
	//So, using one array to sum up all the elements, divided in half each time
	//can also make said array a power of 2, the smallest power of 2 that would fit the array, to avoid any data loss in division that would truncate the last element ...
	//Can get the ceiling of Log base 2 of the size of the amt. components to do this, and have all of the array elements initialized to 0 - 
		//int array[smallestPow2] = {0};
	result[iGID] = t1[iGID] + t2[iGID];	
	
	/*Host code: 
	int smallestPow2 = Mathf.ceil(Mathf.log2(enemies.Length));
	int array[smallestPow2] = {0};
	*/
}