#pragma once

//#include <string.h>
//#include <string>
using namespace std;

struct vector3 {
public:
	float x;
	float y;
	float z;
};

struct strng {
	int size;
	char *chars;
};

extern "C" __declspec(dllexport) vector3 GetUnNormalizedSeparationVector(int objCount, float* objectPos_x, float* objectPos_y, float* objectPos_z, vector3 currPos, char* errorMsgCCode);
extern "C" __declspec(dllexport) vector3 GetUnNormalizedSeparationVectorTest(int objCount, float* objectPos_x, float* objectPos_y, float* objectPos_z, vector3 currPos, char* errorMsgCCode);