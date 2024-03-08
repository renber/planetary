#include <stddef.h>
#include <stdbool.h>
#include <stdio.h>
#include <math.h>

#include <stdlib.h>

#include "CuTest.h"

#include <planetary/stores/frugal.h>
#include "frugalHelper.h"

#define StreamLength 3000
#define StructLength 40


void Test_Frugal(CuTest *tc)
{
	srand(0);
	char frugalCoreArray[StructLength];
	double stream[StreamLength];
	double quantil = 0.5f;
	
	for (int i = 0; i < 5; i++)
	{
		initFrugalArray(frugalCoreArray, quantil);
		initRandomDoubleArray(stream, StreamLength);

		for (int i = 0; i < StreamLength; i++)
		{
			writeToFrugal(stream[i], frugalCoreArray);
		}
		double frugalResult = ((struct FrugalCore*)frugalCoreArray)->m;
		double result = calcMedian(stream, StreamLength, 0.5);

		double distance = fabs(frugalResult - result);

		int inRange = distance < 2.0f ? 1 : 0;

		CuAssertTrue(tc, inRange);
	}

}


CuSuite* FrugalGetSuite()
{
	CuSuite* suite = CuSuiteNew();

	SUITE_ADD_TEST(suite, Test_Frugal);

	return suite;
}
