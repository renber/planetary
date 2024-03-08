#include <stddef.h>
#include <stdbool.h>
#include <stdlib.h>

#include <math.h>

#include "CuTest.h"
#include <planetary/stores/linearCounting.h>

#define StructLength 40

void Test_ChangingBitMap(CuTest* tc)
{
	char linearCoreArray[StructLength];
	initLinearCountingArray(linearCoreArray);
	
	/*Write to amount different indexes in the bit map*/
	int amount = 12;
	amount = amount >= SIZE - 1 ? SIZE - 1 : amount;
	for (int i = 0; i < amount; i++)
	{
		writeToLinearCounting(i, linearCoreArray);
	}
	writeToLinearCounting(0, linearCoreArray);

	/*Test if exactly amount bits have been set*/
	struct LinearCountingCore* core = (struct LinearCountingCore*)linearCoreArray;
	CuAssertIntEquals(tc, SIZE * 8 - amount, calculateZeros(core));

	/*Test if resetting a bit changes the result, which it should not*/
	writeToLinearCounting(0, linearCoreArray);
	CuAssertIntEquals(tc, SIZE * 8 - amount, calculateZeros(core));

	initLinearCountingArray(linearCoreArray);

	/*Test if the modulo operation is working*/
	int tooLargeValue = SIZE * 8;
	writeToLinearCounting(tooLargeValue, linearCoreArray);
	CuAssertIntEquals(tc, SIZE * 8 - 1, calculateZeros(core));
	CuAssertIntEquals(tc, 1, core->bitMap[0]);

}


void Test_EvaluatingBitMap(CuTest* tc)
{
	bool out_isFinished = true;
	char linearCoreArray[StructLength];
	initLinearCountingArray(linearCoreArray);

	/*Write to amount different indexes in the bit map*/
	int amount = 12;
	amount = amount >= SIZE - 1 ? SIZE - 1 : amount;
	for (int i = 0; i < amount; i++)
	{
		writeToLinearCounting(i, linearCoreArray);
	}

	/*Test if the evaluation matches the expected result*/
	struct LinearCountingCore* core = (struct LinearCountingCore*)linearCoreArray;
	int u = SIZE * 8 - amount;
	float v = (float)(u) / (float)(SIZE * 8);
	int result = (int)(-(float)(SIZE * 8) * log(v));
	CuAssertIntEquals(tc, result, readFromLinearCounting(linearCoreArray, 0, 0, &out_isFinished));

	/*Test the accuracy of linear Counting*/
	initLinearCountingArray(linearCoreArray);
	amount = 1000;
	for (int i = 0; i < amount; i++)
	{
		writeToLinearCounting((float)rand(), linearCoreArray);
	}
	bool isWithinError = true;
	int evaluation = readFromLinearCounting(linearCoreArray, 0, 0, &out_isFinished);
	if (evaluation > (int)((float)amount * 1.2f) || evaluation < (int)((float)amount * 0.8f))
	{
		isWithinError = false;
	}
	CuAssertTrue(tc, isWithinError);
}


CuSuite* LinearCountingGetSuite()
{
	CuSuite* suite = CuSuiteNew();

	SUITE_ADD_TEST(suite, Test_ChangingBitMap);
	SUITE_ADD_TEST(suite, Test_EvaluatingBitMap);

	return suite;
}