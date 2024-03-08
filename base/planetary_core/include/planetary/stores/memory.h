#pragma once
#include <planetary/stores/MemoryCore.h>
#include <stdbool.h>
void writeToMemory(float value, char* structArray);

float readFromMemory(char* structArray, float value, int index, bool* out_isFinished);

void initMemoryArray(char* array);