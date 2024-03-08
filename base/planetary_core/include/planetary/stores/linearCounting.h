#pragma once
#include <planetary/stores/LinearCountingCore.h>
#include <stdbool.h>

void writeToLinearCounting(float value, char* structArray);

float readFromLinearCounting(char* structArray, float value, int index, bool* out_isFinished);

void initLinearCountingArray(char* array);

int hashing(float value);

void initBitMap(char* bitMap);

int evaluate(struct LinearCountingCore* core);

int calculateZeros(struct LinearCountingCore* core);
