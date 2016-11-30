// OpenCL Kernel Function  
__kernel void TestOpenCL(__global const float *a, __global const float *b, __global float *result)
{
	// get index into global data array, iGID, corresponding to the current processing element? Among the processing elements executed in parallel?
	// could eg. do iGID * 3 - 3 + 1, 2 or 3 for eg. an offsetted value too ... eg. the 2nd in a pair, etc. ... or maybe something in two pairs ... or such ...
	// maybe t1 and t2 buffers, putting in those where if resultposition%2 == 0 then put in t1, and if resultposition%2 == 1 then put into t2
	// and add such results into a new input and output buffer, and set those as the args for this TestOpenCL function ... until the buffer size would become 1 for the final buffer ... at the final step ... and all other buffers would be empty or used ...
	// So, would make a linked list of buffers? With keeping in mind the ID ...
	// And coul
	int iGID = get_global_id(0);

	// elements operation  
	result[iGID] = a[iGID] * b[iGID];
}