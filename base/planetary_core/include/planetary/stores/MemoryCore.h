#pragma once
#include <planetary/stores.h>

#define MemorySize (int)((StructArraySize - 2 * sizeof(int)) / sizeof(float))
struct MemoryCore
{
	float memory[MemorySize];
	int index;
	int lastWrittenIndex;
};
