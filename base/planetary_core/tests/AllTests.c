#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include "CuTest.h"

/**
 when run, executes all unit tests
 and reports results
**/

CuSuite* EncodingGetSuite();

void RunAllTests(void) {
    srand(time(NULL)); // needed by some tests

    CuString *output = CuStringNew();
    CuSuite* suite = CuSuiteNew();

    CuSuiteAddSuite(suite, EncodingGetSuite());  	
	CuSuiteAddSuite(suite, GroupingGetSuite());	
	CuSuiteAddSuite(suite, QueryCoreGetSuite());	

    CuSuiteRun(suite);
    CuSuiteSummary(suite, output);
    CuSuiteDetails(suite, output);
    printf("%s\n", output->buffer);


#if _DEBUG
	getchar();
#endif
}

int main(void) {
	printf("Running test cases\n");
    RunAllTests();
}
