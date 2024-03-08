#include <planetary/stores/memory.h>

void writeToMemory(float value, char* structArray)
{
	if ((int)value == 0x7FFF) return;
	struct MemoryCore* memoryCore = (struct MemoryCore*)structArray;

	memoryCore->memory[memoryCore->index] = value;
	memoryCore->lastWrittenIndex = memoryCore->index;
	memoryCore->index++;
	if (memoryCore->index >= MemorySize)
	{
		memoryCore->index = 0;
	}
}


float readFromMemory(char* structArray, float value, int index, bool* out_isFinished)
{
	struct MemoryCore* memoryCore = (struct MemoryCore*)structArray;

	if (index >= MemorySize)
	{
		*out_isFinished = true;
		return memoryCore->memory[0];
	}

	if (index + 1 >= MemorySize)
	{
		*out_isFinished = true;
	}
	return memoryCore->memory[index];
}


void initMemoryArray(char* array)
{
	struct MemoryCore* memoryCore = (struct MemoryCore*)array;
	for (int i = 0; i < MemorySize; i++)
	{
		memoryCore->memory[i] = 0;
	}
	memoryCore->index = 0;
}
