// OpenCL Kernel Function  
__kernel void TestOpenCL(__global const float *a, __global const float *b, __global float *result)
{
	// get index into global data array, iGID, corresponding to the current processing element? Among the processing elements executed in parallel?
	// could eg. do iGID * 3 - 3 + 1, 2 or 3 for eg. an offsetted value too ... eg. the 2nd in a pair, etc. ... or maybe something in two pairs ... or such ...
	// maybe t1 and t2 buffers, putting in those where if resultposition%2 == 0 then put in t1, and if resultposition%2 == 1 then put into t2
	// and add such results into a new input and output buffer, and set those as the args for this TestOpenCL function ... until the buffer size would become 1 for the final buffer ... at the final step ... and all other buffers would be empty or used ...
	// So, would make a linked list of buffers? With keeping in mind the ID ...
	int iGID = get_global_id(0);

	// elements operation  
	result[iGID] = a[iGID] * b[iGID];
}//Credit goes to http://simpleopencl.blogspot.ca/2013/05/atomic-operations-and-floats-in-opencl.html for this.
 //Could be 'slow' due to spending computation in doing a recheck each time that the atomic operation would have failed, but for our game, this would not really matter.
void atomic_add_global(volatile global float *source, const float operand) {
	union {
		unsigned int intVal;
		float floatVal;
	} newVal;
	union {
		unsigned int intVal;
		float floatVal;
	} prevVal;
	do {
		prevVal.floatVal = *source;
		newVal.floatVal = prevVal.floatVal + operand;
	} while (atomic_cmpxchg((volatile global unsigned int *)source, prevVal.intVal, newVal.intVal) != prevVal.intVal);
}

// OpenCL Kernel Function  
__kernel void TestOpenCLAdd(__global float *x, __global float *y, __global float *z, __global float *currVecPos, __global float *totalVecPos, __global int *neighbourCount) //add up neighbours if they are close enough to currVecPos
{

	int iGID = get_global_id(0); //global id of the current work item, or index of the array

	if ((x[iGID] - currVecPos[0]) * (x[iGID] - currVecPos[0]) + (z[iGID] - currVecPos[2]) * (z[iGID] - currVecPos[2]) < 12) //if distance from other enemy^2 < reqDistance^2, reqDistance hardcoded as sqrt(12)
	{
		//Add neighbours atomically
		//barrier(CLK_GLOBAL_MEM_FENCE); //wait for current step to finish before completing execution, or previous command before finishing ...though is this necessary?
		//add each of these atomically to the respective totalVecPos
		atomic_add_global(&totalVecPos[0], x[iGID]); //add to x total
		atomic_add_global(&totalVecPos[1], y[iGID]); //add to y total
		atomic_add_global(&totalVecPos[2], z[iGID]); //add to z total
													 //add atomically to neighbourCount
		atomic_inc(neighbourCount);
			//y[iGID] = y[iGID] + y[iGID];
			//z[iGID] = z[iGID] + z[iGID + get_global_size(0)];
	}
	//barrier(CLK_GLOBAL_MEM_FENCE); //wait for current step to finish before completing execution, or previous command before finishing ...though is this necessary?
}