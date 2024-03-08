#pragma once
#include <planetary/stores.h>

#define SIZE (StructArraySize - 2 * sizeof(int))

/*
*	Core for the linearCounting Sketch
*	count: Int -> The intermediate Result of the sketch
*	numberOfElements: Int -> How many elements have been added to the bitMap
*	bitMap: char[SIZE] -> The bitMap used in the calculation
*/
struct LinearCountingCore
{
	int count;
	int numberOfElements;
	char bitMap[SIZE];
};
