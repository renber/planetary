#pragma once
#include <planetary/stores/SpaceSavingCore.h>
#include <stdbool.h>

int comparator(const void * v1, const void * v2);

void writeToSpaceSaving(float value, char* structArray);

float readFromSpaceSaving(char* structArray, float value, int index, bool* out_isFinished);

void initSpaceSavingArray(char* array);

int findSlot(struct SpaceSavingSlot * array, int newElement);

void overrideAtIndex(struct SpaceSavingSlot * array, int newElement, int index);

bool compareToIndex(int ith, int index, struct SpaceSavingCore * core, bool errorIsZero);