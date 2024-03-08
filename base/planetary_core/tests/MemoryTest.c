#include <stddef.h>
#include <stdbool.h>

#include <stdlib.h>

#include "CuTest.h"
#include <planetary/stores/memory.h>

#define StructLength 40

void Test_Memory(CuTest* tc)
{
	char memoryCoreArray[StructLength];
	initMemoryArray(memoryCoreArray);
	for (int i = 0; i < 10; i++)
	{
		writeToMemory(i, memoryCoreArray);
	}
	struct MemoryCore* memoryCore = (struct MemoryCore*)memoryCoreArray;

	/*Test if the value at index x is what the function returns*/
	bool out_isFinished = false;
	for (int i = 0; i < 8; i++)
	{
		CuAssertFalse(tc, out_isFinished);
		CuAssertIntEquals(tc, memoryCore->memory[i], readFromMemory(memoryCoreArray, 0, i, &out_isFinished));
	}
	CuAssertTrue(tc, out_isFinished);

	/*Test if the values are correct*/
	CuAssertIntEquals(tc, 8, memoryCore->memory[0]);
	CuAssertIntEquals(tc, 9, memoryCore->memory[1]);
	CuAssertIntEquals(tc, 2, memoryCore->memory[2]);
	CuAssertIntEquals(tc, 3, memoryCore->memory[3]);
}


CuSuite* MemoryGetSuite()
{
	CuSuite* suite = CuSuiteNew();

	SUITE_ADD_TEST(suite, Test_Memory);

	return suite;
}