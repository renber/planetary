#include <stddef.h>
#include <stdbool.h>

#include <stdlib.h>

#include "CuTest.h"
#include <planetary/stores/spaceSaving.h>

#define StructLength 40

void Test_SpaceSaving(CuTest* tc)
{
	char spaceSavingCoreArray[StructLength];
	initSpaceSavingArray(spaceSavingCoreArray);
	for (int i = 0; i < 10; i++)
	{
		if (i >= SLOTS - 1)
		{
			writeToSpaceSaving(10, spaceSavingCoreArray);
			continue;
		}
		writeToSpaceSaving(i, spaceSavingCoreArray);
	}
	struct SpaceSavingCore * core = (struct SpaceSavingCore *)spaceSavingCoreArray;

	/*Test if the values are correct*/
	for (int i = 0; i < SLOTS - 1; i++)
	{
		CuAssertIntEquals(tc, i, core->slots[i].element);
		CuAssertIntEquals(tc, 1, core->slots[i].amount);
		CuAssertIntEquals(tc, 0, core->slots[i].error);
	}
	CuAssertIntEquals(tc, 10, core->slots[2].element);
	CuAssertIntEquals(tc, 8, core->slots[2].amount);
	CuAssertIntEquals(tc, 0, core->slots[2].error);

	writeToSpaceSaving(9, spaceSavingCoreArray);

	CuAssertIntEquals(tc, 9, core->slots[0].element);
	CuAssertIntEquals(tc, 2, core->slots[0].amount);
	CuAssertIntEquals(tc, 1, core->slots[0].error);
}


CuSuite* SpaceSavingGetSuite()
{
	CuSuite* suite = CuSuiteNew();

	SUITE_ADD_TEST(suite, Test_SpaceSaving);

	return suite;
}