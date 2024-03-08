#include <stddef.h>
#include <stdbool.h>
#include <stdarg.h>

#include <planetary\querytypes.h>
#include <planetary/proto/query.pb.h>
#include <planetary\queries.h>
#include <planetary/grouping.h>

#include "CuTest.h"

#define TEST_TICK_STEP 1000
#define EPSILON 0.00001

// adds a row with the given float values to the resultset
void addResultRow(Resultset* resultset, int values_count, ...) {

	resultset->rows[resultset->rows_count].values_count = values_count;

	va_list ap;	
	va_start(ap, values_count);	
	for (int i = 0; i < values_count; i++) {
		double v = va_arg(ap, double);
		resultset->rows[resultset->rows_count].values[i] = (float)v;
	}
	va_end(ap);

	resultset->rows_count++;
}

void Test_addResultRow(CuTest *tc)
{
	Resultset resultset = Resultset_init_default;
	addResultRow(&resultset, 1, 1.0);
	addResultRow(&resultset, 1, 4.0);
	addResultRow(&resultset, 1, 3.0);

	CuAssertIntEquals(tc, 3, resultset.rows_count);
	CuAssertDblEquals(tc, 1, resultset.rows[0].values[0], EPSILON);
	CuAssertDblEquals(tc, 4, resultset.rows[1].values[0], EPSILON);
	CuAssertDblEquals(tc, 3, resultset.rows[2].values[0], EPSILON);
}

void Test_CombineResults_Max(CuTest *tc)
{
	Query emptyQuery = Query_init_default;
	Resultset emptyResultset = Resultset_init_default;

	QuerySlot slot;
	slot.state = STATE_WAIT_FOR_RESULTS;
	slot.queryData = emptyQuery;
	slot.queryData.actions_count++;
	slot.queryData.actions[0].which_content = ActionType_SELECTOR;
	slot.queryData.actions[0].content.selector.which_source = TargetType_TARGETSINK;
	slot.queryData.actions[0].content.selector.type = SelectorType_MAX;	
	strcpy_s(slot.queryData.actions[0].content.selector.source.sensor.sensorId.name, 9, SENSOR_ID);
	
	// add 3 result rows
	slot.resultset = emptyResultset;
	addResultRow(&slot.resultset, 1, 1.0);
	addResultRow(&slot.resultset, 1, 4.0);
	addResultRow(&slot.resultset, 1, 3.0);
	CuAssertIntEquals(tc, 3, slot.resultset.rows_count);

	// combine should now reduce the results to one row containing the max value
	combineResults(&slot);

	CuAssertIntEquals(tc, 1, slot.resultset.rows_count);
	CuAssertIntEquals(tc, 1, slot.resultset.rows[0].values_count);
	CuAssertDblEquals(tc, 4, slot.resultset.rows[0].values[0], EPSILON);
}

CuSuite* GroupingGetSuite() {
	CuSuite* suite = CuSuiteNew();

	SUITE_ADD_TEST(suite, Test_addResultRow);
	SUITE_ADD_TEST(suite, Test_CombineResults_Max);

	return suite;
}
