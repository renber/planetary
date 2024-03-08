#pragma once
#include <stdbool.h>
struct FrugalCore
{
	float quantil;
	float m;
	float step;
	int sign;
	int c;
	int n;
	bool wasGreater;
	bool firstNumber;
};