#pragma once
#include <planetary/querytypes.h>

enum
{
	INCREASE = 0,
	STAY = 1,
	DECREASE = 2
};

void initRandomDoubleArray(double* doubleArray, int length);
double calcMedian(double* doubleArray, int length, double quantil);
int classify(double rand);
int compareDoubles(const void* p1, const void* p2);