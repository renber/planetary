#pragma once
#include <planetary/stores/FrugalCore.h>
#include <stdbool.h>

void writeToFrugal(float value, char* structArray);

float readFromFrugal(char* structArray, float value, int index, bool* out_isFinished);

void initFrugalArray(char* array, float param);

float capStep(float step);

float capGrowth(struct FrugalCore* frugalCore);

float f(float step);

float randomNumber();
