#ifdef _MSC_VER
#define _CRT_SECURE_NO_WARNINGS
#endif
#include <tchar.h>  
#include <fstream>  
#include <stdio.h>  
#include <stdlib.h>  
#include <iostream>
#include <string.h>
#include <CL/cl.h>
#include "Test.h"


using namespace std;
__declspec(dllexport) int Test(int input)
{
	return 599;
}

float inPut_a[1] = { 2 };
float inPut_b[1] = { 4 };
//float result[3] = { 0, 0, 0 }; //x,y,z ...actually, just going to get from the input buffers

//OpenCL preserved globals that are set up and initialized only once
cl_int status;
cl_uint numPlatforms;
cl_platform_id *_platformIds;
cl_device_id *_deviceIds;
cl_context _context;
cl_program _program;
cl_kernel _kernel_GPU;
cl_command_queue _commandQueue_GPU;
cl_mem _memoryBuffer[6];
bool programCompiled = false;

char* _errorMsgCCode;
//int _objCountPow2;
//vector3* _objectPos;
//float* _objectPos_x, *_objectPos_y, *_objectPos_z;

#define arraySize 10

//create opencl platform
cl_platform_id *GeneratePlatform()
{
	cl_platform_id *platformIds;
	//obtain the list of platforms available 
	//cl_int clGetPlatformIDs(cl_uint num_entries, cl_platform_id *platforms, cl_uint *num_platforms)
	status = clGetPlatformIDs(0, NULL, &numPlatforms);

	//if not obtaining platforms, exit;
	//if (status != CL_SUCCESS || numPlatforms <= 0) {
	//	printf("Failed to find any OpenCL platforms.");
	//	strcpy(_errorMsgCCode, "Failed to find any OpenCL platforms.");
	//	return 0;
	//}

	//printf("Platform Numbers: %d\n", numPlatforms);

	//distribute memories to id
	platformIds = (cl_platform_id *)malloc(numPlatforms * sizeof(cl_platform_id));
	//get all platforms
	status = clGetPlatformIDs(numPlatforms, platformIds, NULL);

	//if (status != CL_SUCCESS) {
	//	printf("Failed to find any OpenCL platforms.");
	//	strcpy(_errorMsgCCode, "Failed to find any OpenCL platforms.");
	//	return 0;
	//}

	return platformIds;
}

void PlatformInfo(cl_platform_id *platformIds)
{
	//print platform information
	size_t ext_size = 0;

	for (int i = 0; i < numPlatforms; i++)
	{
		printf("--------------------------------- \n");

		clGetPlatformInfo(platformIds[i], CL_PLATFORM_NAME, 0, NULL, &ext_size);
		char *name = (char*)malloc(ext_size);
		clGetPlatformInfo(platformIds[i], CL_PLATFORM_NAME, ext_size, name, NULL);
		printf("Platform Name: %s\n", name);

		clGetPlatformInfo(platformIds[i], CL_PLATFORM_VENDOR, 0, NULL, &ext_size);
		char *vendor = (char*)malloc(ext_size);
		clGetPlatformInfo(platformIds[i], CL_PLATFORM_VENDOR, ext_size, vendor, NULL);
		printf("Platform Vendor: %s\n", vendor);

		clGetPlatformInfo(platformIds[i], CL_PLATFORM_VERSION, 0, NULL, &ext_size);
		char *version = (char*)malloc(ext_size);
		clGetPlatformInfo(platformIds[i], CL_PLATFORM_VERSION, ext_size, version, NULL);
		printf("Platform Version: %s\n", version);

		clGetPlatformInfo(platformIds[i], CL_PLATFORM_PROFILE, 0, NULL, &ext_size);
		char *profile = (char*)malloc(ext_size);
		clGetPlatformInfo(platformIds[i], CL_PLATFORM_PROFILE, ext_size, profile, NULL);
		printf("Platform Full Profile or Embeded Profile: %s\n", profile);

		delete name;
		delete vendor;
		delete version;
		delete profile;
	}
	printf("--------------------------------- \n");
}
cl_device_id *SetupCPUDevice(cl_platform_id platformIds)
{
	cl_device_id *deviceIds;
	cl_uint numDevices;

	//check the device and get the size of the device buffer.  
	status = clGetDeviceIDs(platformIds, CL_DEVICE_TYPE_CPU, 0, NULL, &numDevices);

	//if (status != CL_SUCCESS) {
	//	printf("Failed to get CPU Device");
	//	exit(1);
	//}

	//printf("CPU Device Numbers: %d\n", numDevices);

	deviceIds = (cl_device_id *)malloc(numDevices * sizeof(cl_device_id));
	clGetDeviceIDs(platformIds, CL_DEVICE_TYPE_CPU, numDevices, deviceIds, NULL);

	return deviceIds;
}

cl_device_id *SetupGPUDevice(cl_platform_id platformIds)
{
	cl_device_id *deviceIds;
	cl_uint numDevices;

	//check the device and get the size of the device buffer.  
	status = clGetDeviceIDs(platformIds, CL_DEVICE_TYPE_GPU, 0, NULL, &numDevices);

	//if (status != CL_SUCCESS) {
	//	printf("Failed to get GPU Device");
	//	strcpy(_errorMsgCCode, "Failed to get GPU Device");
	//}

	//printf("GPU Device Numbers: %d\n", numDevices);

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
	free(name);
	free(device);
}

cl_context GenerateContext(cl_device_id *deviceIds) //make context for the device with deviceIds as the device ID?
{
	//cl_int errcode_ret;
	if(_context == NULL)
		_context = clCreateContext(NULL, 1, deviceIds, NULL, NULL, NULL);

	if (_context == NULL) {
		printf("Failed to create context");
		strcpy(_errorMsgCCode, "Failed to create context - error <errorcode not to be here as of this writing on 11/20/16, 2:44AM>");
		return _context;
	}

	return _context;
}

void ContextInfo(cl_device_id deviceIds)
{
	size_t ext_size = 0;

	clGetDeviceInfo(deviceIds, CL_DEVICE_NAME, 0, NULL, &ext_size);
	char *device = (char*)malloc(ext_size);
	clGetDeviceInfo(deviceIds, CL_DEVICE_NAME, ext_size, device, NULL);
	printf("Device for Current Context: %s\n", device);

	printf("--------------------------------- \n");
	free(device);
}
cl_command_queue GenerateCommandQueue(cl_context context, cl_device_id deviceIds)
{
	cl_command_queue commandQueue;
	commandQueue = clCreateCommandQueue(context, deviceIds, 0, NULL); //create command queue for GPU

	//if (commandQueue == NULL) {
	//	printf("Failed to create Command Queue");

	//	strcpy(_errorMsgCCode, "Failed to create Command Queue");
	//	//exit(1);
	//}

	return commandQueue;
}

char* oclLoadProgSource(const char* cFilename, const char* cPreamble, size_t* szFinalLength);

bool isErr(cl_mem *memoryBuffer, float *currPosArr, float *totalVecPos, int *neighbourCount, cl_platform_id *platformIds, cl_device_id* deviceIds) {
	if (_errorMsgCCode[0] != 'n') { //default of 'nothing'
		/*if (memoryBuffer != NULL) {
			for (int i=0; i<6; i++)
			{
				if (memoryBuffer[i] != NULL) {
					free(memoryBuffer[i]);
				}
			}
		}*/
		delete[] currPosArr;
		delete[] totalVecPos;
		delete[] neighbourCount;
		free(platformIds);
		if (deviceIds != NULL)
			free(deviceIds);
		return true;
	}
	return false;
}

cl_program ProgramObject(const char *filePath, cl_context context)
{
	cl_program program;
	char *source;
	size_t sourceLength;

	source = oclLoadProgSource(filePath, "", &sourceLength);
	//if (source == NULL) {
	//	printf("Error in oclLoadProgSource");

	//	strcpy(_errorMsgCCode, "Error in oclLoadProgSource");
	//	//exit(1);
	//	return 0;
	//}

	program = clCreateProgramWithSource(context, 1, (const	 char **)&source, &sourceLength, &status);

	//if (status != CL_SUCCESS) {
	//	printf("Error in CreateProgramWithSource");
	//	strcpy(_errorMsgCCode, "Error in CreateProgramWithSource");
	//	return 0;
	//}
	free(source);

	return program;
}

cl_int Compiler(cl_program program)
{
	status = clBuildProgram(program, 0, NULL, NULL, NULL, NULL);

	if (status != CL_SUCCESS) {
		printf("Compiler is failed");
		strcpy(_errorMsgCCode, "Compiler is failed");
	}
	return status;
}

cl_kernel KernelMemory(cl_context context, cl_program program)
{

	cl_kernel kernel;

	kernel = clCreateKernel(program, "TestOpenCLAdd", NULL);
	//if (kernel == NULL) {
	//	printf("Error: Can not create kernel \n");
	//	strcpy(_errorMsgCCode, "Error: Can not create kernel TestOpenCL\n");
	//}

	return kernel;
}
cl_kernel KernelMemory_Init(cl_context context, cl_program program)
{

	cl_kernel kernel;

	kernel = clCreateKernel(program, "TestOpenCL2", NULL);
	if (kernel == NULL) {
		printf("Error: Can not create kernel \n");
		strcpy(_errorMsgCCode, "Error: Can not create kernel TestOpenCL_Init \n");
	}

	return kernel;
}
/*
void KernelCompiler(cl_context context, cl_kernel kernel, float* objectPos_x, float* objectPos_y, float* objectPos_z, int objCountPow2) //Compile kernel?
{
	int i = 0;
	int totalSize = pow(2, objCountPow2);

	//Make R\W buffers? For the current kernel for the current context? For what device
	memoryBuffer[0] = clCreateBuffer(context, CL_MEM_READ_ONLY | CL_MEM_COPY_HOST_PTR, totalSize * sizeof(float), objectPos_x, NULL); //could try CL_MEM_COPY_USE_HOST_PTR as well
	memoryBuffer[1] = clCreateBuffer(context, CL_MEM_READ_ONLY | CL_MEM_COPY_HOST_PTR, totalSize * sizeof(float), objectPos_y, NULL);
	memoryBuffer[2] = clCreateBuffer(context, CL_MEM_READ_ONLY | CL_MEM_COPY_HOST_PTR, totalSize * sizeof(float), objectPos_z, NULL);
	//memoryBuffer[3] = clCreateBuffer(context, CL_MEM_WRITE_ONLY | CL_MEM_COPY_HOST_PTR, arraySize * sizeof(float), result, NULL); //result as actually buffer with result[0], result[1], and result[2] containing x,y,z respectively ... or _objectPos_x, _objectPos_y, and objectPos_z respectively

	//Handle if a memoryBuffer element would be null - print an error...
	for each (cl_mem memObjects in memoryBuffer)
	{
		if (memObjects == NULL) {
			printf("Error %d : creating memory objects \n", i);
			strcpy(_errorMsgCCode, "Error %d : creating memory objects \n");
		}
		i++;
	}

	//set the kernel arguments to be *arg_value <- in this case, with memoryBuffer[...] being the *arg_value>
	//kernel arguments being for 
	status = clSetKernelArg(kernel, 0, sizeof(cl_mem), &memoryBuffer[0]);
	status |= clSetKernelArg(kernel, 1, sizeof(cl_mem), &memoryBuffer[1]);
	status |= clSetKernelArg(kernel, 2, sizeof(cl_mem), &memoryBuffer[2]);
	//status |= clSetKernelArg(kernel, 3, sizeof(cl_mem), &memoryBuffer[3]);

	if (status != CL_SUCCESS) {
		printf("Error in clSetKernelArg \n");
		strcpy(_errorMsgCCode, "Error in clSetKernelArg \n");
	}
}
*/
void Debuging(cl_command_queue commandQueue, cl_kernel kernel)
{
}


	//////////////////////////////////////////////////////////////////////////////
	//! Loads a Program file and prepends the cPreamble to the code.
	//!
	//! @return the source string if succeeded, 0 otherwise
	//! @param cFilename        program filename
	//! @param cPreamble        code that is prepended to the loaded file, typically a set of #defines or a header
	//! @param szFinalLength    returned length of the code string
	//////////////////////////////////////////////////////////////////////////////

	char* oclLoadProgSource(const char* cFilename, const char* cPreamble, size_t* szFinalLength)
	{
		// locals 
		FILE* pFileStream = NULL;
		size_t szSourceLength;

		// open the OpenCL source code file
#ifdef _WIN64   // Windows version
		if (fopen_s(&pFileStream, cFilename, "rb") != 0)
		{
			return NULL;
		}
#else           // Linux version

		pFileStream = fopen(cFilename, "rb");
		if (pFileStream == 0)
		{
			return NULL;
		}
#endif

		size_t szPreambleLength = strlen(cPreamble);

		// get the length of the source code
		fseek(pFileStream, 0, SEEK_END);
		szSourceLength = ftell(pFileStream);
		fseek(pFileStream, 0, SEEK_SET);

		// allocate a buffer for the source code string and read it in
		char* cSourceString = (char *)malloc(szSourceLength + szPreambleLength + 1);
		memcpy(cSourceString, cPreamble, szPreambleLength);
		if (fread((cSourceString)+szPreambleLength, szSourceLength, 1, pFileStream) != 1)
		{
			fclose(pFileStream);
			free(cSourceString);
			return 0;
		}

		// close the file and return the total length of the combined (preamble + source) string
		fclose(pFileStream);
		if (szFinalLength != 0)
		{
			*szFinalLength = szSourceLength + szPreambleLength;
		}
		cSourceString[szSourceLength + szPreambleLength] = '\0';

		return cSourceString;
	}


	void CleanUp(cl_mem *memoryBuffer, cl_device_id *deviceIds, cl_context context, cl_command_queue commandQueue, cl_program program, cl_kernel kernel)
	{
		
		for (int i = 0; i < 6; i++)
		{
			if (memoryBuffer[i] != 0) {
				clReleaseMemObject(memoryBuffer[i]);
			}
		}
		//if (kernel != 0) { clReleaseKernel(kernel); }
		//if (program != 0) { clReleaseProgram(program); }
		//if (deviceIds[0] != 0) { clReleaseDevice(deviceIds[0]); }

		/*if (commandQueue != NULL) {
			clFinish(commandQueue);
			clReleaseCommandQueue(commandQueue);
		}*/
		//if (context != 0) { clReleaseContext(context); }
	}

//gets total mass and count, and converts that into an un-normalized avoidance or separation vector
__declspec(dllexport) vector3 GetUnNormalizedSeparationVector(int objCount, float* objectPos_x, float* objectPos_y, float* objectPos_z, vector3 currPos, char* errorMsgCCode)
{
	//code done in parallel
	//pass the currPos vector to an array
	float* currPosArr = new float[3];
	currPosArr[0] = currPos.x;
	currPosArr[1] = currPos.y;
	currPosArr[2] = currPos.z;

	int* neighbourCount = new int[1];
	neighbourCount[0] = 0;
	float* totalVecPos = new float[3];
	totalVecPos[0] = 0;
	totalVecPos[1] = 0;
	totalVecPos[2] = 0;

	const char* filePath = "Assets/DLLs/DLLTEST/DLLTEST/TestK.cl";

	if(_platformIds == NULL) {
		_platformIds = GeneratePlatform();

	//set up OpenCL code on GPU
		_deviceIds = SetupCPUDevice(_platformIds[0]); //includes a malloc of a cl_device_id to deviceIds, and some printing of the number of devices in the platform ...
		_context = GenerateContext(_deviceIds); //generate context for device?
		_commandQueue_GPU = GenerateCommandQueue(_context, _deviceIds[0]); //noting commandQueue ... calls clCreateCommandQueue <with error handling> //being used to enqueue commands? For this current device at index 0
		_program = ProgramObject(filePath, _context); //set up kernel - Test.cl - for this current device
		programCompiled = true;
		Compiler(_program); //calls clBuildProgram, which builds a program executable?
		_kernel_GPU = KernelMemory(_context, _program); //calls  clCreateKernel for this - creating a kernel, ie. a function <removed here: called>declared within a program
	}
	//start of KernelCompiler code
	int totalSize = objCount;

	//Make R\W buffers? For the current kernel for the current context? For what device
	_memoryBuffer[0] = clCreateBuffer(_context, CL_MEM_READ_WRITE | CL_MEM_USE_HOST_PTR, totalSize * sizeof(float), objectPos_x, NULL); //could try CL_MEM_COPY_USE_HOST_PTR as well
	_memoryBuffer[1] = clCreateBuffer(_context, CL_MEM_READ_WRITE | CL_MEM_USE_HOST_PTR, totalSize * sizeof(float), objectPos_y, NULL);
	_memoryBuffer[2] = clCreateBuffer(_context, CL_MEM_READ_WRITE | CL_MEM_USE_HOST_PTR, totalSize * sizeof(float), objectPos_z, NULL);
	_memoryBuffer[3] = clCreateBuffer(_context, CL_MEM_READ_WRITE | CL_MEM_USE_HOST_PTR, 3 * sizeof(float), currPosArr, NULL); //3 being vector size //result as actually buffer with result[0], result[1], and result[2] containing x,y,z respectively ... or _objectPos_x, _objectPos_y, and objectPos_z respectively
	_memoryBuffer[4] = clCreateBuffer(_context, CL_MEM_READ_WRITE | CL_MEM_USE_HOST_PTR, 3 * sizeof(float), totalVecPos, NULL);
	_memoryBuffer[5] = clCreateBuffer(_context, CL_MEM_READ_WRITE | CL_MEM_USE_HOST_PTR, 1 * sizeof(int), neighbourCount, NULL);

	//Removed from here: Handle if a memoryBuffer element would be null - print an error...

	//set the kernel arguments to be *arg_value <- in this case, with memoryBuffer[...] being the *arg_value>
	//kernel arguments being for 
	status = clSetKernelArg(_kernel_GPU, 0, sizeof(cl_mem), &_memoryBuffer[0]);
	status |= clSetKernelArg(_kernel_GPU, 1, sizeof(cl_mem), &_memoryBuffer[1]);
	status |= clSetKernelArg(_kernel_GPU, 2, sizeof(cl_mem), &_memoryBuffer[2]);
	status |= clSetKernelArg(_kernel_GPU, 3, sizeof(cl_mem), &_memoryBuffer[3]);
	status |= clSetKernelArg(_kernel_GPU, 4, sizeof(cl_mem), &_memoryBuffer[4]);
	status |= clSetKernelArg(_kernel_GPU, 5, sizeof(cl_mem), &_memoryBuffer[5]);

	//End of KernelCompiler code

	//run kernel stuff - the TestOpenCL function - on each 'processing element' in parallel?

	//totalSize = pow(2, objCountPow2); //total array size
	size_t globalWorkSize[1] = { totalSize };
	size_t localWorkSize[1] = { 1 }; //only work on the left side
	status = clEnqueueNDRangeKernel(_commandQueue_GPU, _kernel_GPU, 1, NULL, globalWorkSize, localWorkSize, 0, NULL, NULL);

	//read the memoryBuffers into the respective objectPos arrays again - get final totalVecPos and neighbourCount
	status = clEnqueueReadBuffer(_commandQueue_GPU, _memoryBuffer[4], CL_TRUE, 0, 3 * sizeof(float), totalVecPos, 0, NULL, NULL);
	status = clEnqueueReadBuffer(_commandQueue_GPU, _memoryBuffer[5], CL_TRUE, 0, 1 * sizeof(int), neighbourCount, 0, NULL, NULL);
	
	//handle neighbours, taking into account potential division by zero if zero neighbours
	int neighbourCountInt = neighbourCount[0];

	//(current pos - (center of mass coords)) providing the separation vector
	float normVecPos_x = (neighbourCountInt != 0) ? (currPosArr[0] - (totalVecPos[0] / neighbourCountInt)) : 0;
	float normVecPos_y = (neighbourCountInt != 0) ? (currPosArr[1] - (totalVecPos[1] / neighbourCountInt)) : 0;
	float normVecPos_z = (neighbourCountInt != 0) ? (currPosArr[2] - (totalVecPos[2] / neighbourCountInt)) : 0;

	//free all of the allocated memory
	delete [] currPosArr;
	delete [] totalVecPos;
	delete [] neighbourCount;
	//CleanUp(_memoryBuffer, _deviceIds, _context, _commandQueue_GPU, _program, _kernel_GPU);
	for (int i = 0; i < 6; i++)
	{
		clReleaseMemObject(_memoryBuffer[i]);
	}
	//return normalized weighted sum of separation (normalized in C# code)
	return vector3{ normVecPos_x, normVecPos_y, normVecPos_z };
}
