#pragma once
#include <planetary/stores.h>

#define SLOTS (int)(StructArraySize / (3 * sizeof(int)))

/*
*	A Bucket used in the spaceSaving Sketch
*	element: Int -> The actual element from the dataStream
*	amount: Int -> How often the bucket has been written into
*	error: Int -> The error assumption made by the sketch
*/
struct SpaceSavingSlot
{
	int element;
	int amount;
	int error;
};


/*
*	The core for the spaceSaving Sketch
*	slots: SpaceSavingSlot[SLOTS] -> Array containing all the buckets for the spaceSaving Sketch
*/
struct SpaceSavingCore
{
	struct SpaceSavingSlot slots[SLOTS];
};
