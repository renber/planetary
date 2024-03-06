#include <stddef.h>
#include <stdbool.h>

#include <planetary/proto/query.pb.h>

#include <planetary/querytypes.h>
#include <planetary/queries.h>
#include <planetary/packet_man.h>

#include "CuTest.h"
#include "NetSim.h"

#define TEST_TICK_STEP 1000

void TestScheduleEmptyQuery(CuTest *tc) {
	QueryCore core;
	initQueryCore(&core, (NodeId) { 0 }, AM_SINK);

	// schedule a new query
	Query query = Query_init_default;
	query.queryId.shortId = 42;

	QuerySlot* slot = scheduleQuery(&core, &query);
	CuAssertTrue(tc, slot != NULL);
	CuAssertIntEquals(tc, 1, getActiveQueryCount(&core));
	
	// advance the core -> query should be finished right away
	CuAssertTrue(tc, advanceQueryCore(&core, TEST_TICK_STEP));	

	// check that all results (i.e. one) are present (there are no conditions, that means they are always fulfilled)
	CuAssertIntEquals(tc, 1, slot->resultset.rows_count);
	// But as there are no selections, the reultset contains no values
	CuAssertIntEquals(tc, 0, slot->resultset.rows[0].values_count);

	// check that a packet send request has been generated
	CuAssertFalse(tc, queueIsEmpty(&core.packetQueue));
	PacketToSend* pack = queuePeekHead(&core.packetQueue);
	CuAssertTrue(tc, pack->what == PlanetaryMessage_resultset_tag);
}

void TestThreeNodeNetwork(CuTest *tc) {

	// simulate a simple network consisting of 3 nodes in the form
	// nodeOne <--> sink <--> nodeTwo
	// a query is scheduled and Sink and sent to Node1 and Node2
	// which send their results back to sink
	// The query selects the maximum node ID (i.e. 2)

	NetSim sim;
	initSim(&sim);

	QueryCore* sink = addNodeToSim(&sim, (NodeId) { 0 }, 2, 1, 2); // node 0 with neighbors 1 and 2
	QueryCore* nodeOne = addNodeToSim(&sim, (NodeId) { 1 }, 1, 0); // node 1 with neighbor sink
	QueryCore* nodeTwo = addNodeToSim(&sim, (NodeId) { 2 }, 1, 0); // node 2 with neighbor sink

	// create & schedule the query at the sink
	Query query = Query_init_default;	
	query.queryId.shortId = 42;
	query.actions_count = 1;
	query.actions[0].which_content = ActionType_SELECTOR;
	query.actions[0].content.selector.type = SelectorType_MAX;
	strcpy_s(query.actions[0].content.selector.sensorId.name, 9, SENSOR_ID);	
	querySim(&sim, &query);

	CuAssertIntEquals(tc, 1, getActiveQueryCount(sink));
	CuAssertIntEquals(tc, 0, getActiveQueryCount(nodeOne));
	CuAssertIntEquals(tc, 0, getActiveQueryCount(nodeTwo));

	// advance the sim, the child nodes should now receive the query
	advanceSim(&sim, PARENT_SELECTION_WAIT_TIME);
	advanceSim(&sim, BROADCAST_WAIT_TIME);

	CuAssertIntEquals(tc, 1, getActiveQueryCount(sink));
	CuAssertIntEquals(tc, 1, getActiveQueryCount(nodeOne));
	CuAssertIntEquals(tc, 1, getActiveQueryCount(nodeTwo));

	// now the child nodes process the query and send the result back to the sink	
	advanceSim(&sim, PARENT_SELECTION_WAIT_TIME);
	advanceSim(&sim, BROADCAST_WAIT_TIME);
	advanceSim(&sim, TEST_TICK_STEP);

	CuAssertIntEquals(tc, 0, getActiveQueryCount(sink)); // query has finished
	CuAssertIntEquals(tc, 0, getActiveQueryCount(nodeOne));
	CuAssertIntEquals(tc, 0, getActiveQueryCount(nodeTwo));

	// check the query result		
	CuAssertIntEquals(tc, query.queryId.shortId, sim.lastResultset.queryId.shortId);
	CuAssertIntEquals(tc, 1, sim.lastResultset.rows_count);
	CuAssertIntEquals(tc, 2, sim.lastResultset.rows[0].values[0]);
}

void TestThreeNodeNetwork_SingleResult(CuTest* tc) {

	// simulate a simple network consisting of 3 nodes in the form
	// nodeOne <--> sink <--> nodeTwo
	// a query is scheduled and Sink and sent to Node1 and Node2
	// which send their results back to sink
	// The query selects the maximum node ID (i.e. 2)

	NetSim sim;
	initSim(&sim);

	QueryCore* sink = addNodeToSim(&sim, (NodeId) { 4 }, 2, 1, 2); // node 0 with neighbors 1 and 2
	QueryCore* nodeOne = addNodeToSim(&sim, (NodeId) { 1 }, 1, 4); // node 1 with neighbor sink
	QueryCore* nodeTwo = addNodeToSim(&sim, (NodeId) { 2 }, 1, 4); // node 2 with neighbor sink

	// create & schedule the query at the sink
	Query query = Query_init_default;
	query.queryId.shortId = 42;
	query.actions_count = 1;
	query.actions[0].which_content = ActionType_SELECTOR;
	query.actions[0].content.selector.type = SelectorType_SUM;
	strcpy_s(query.actions[0].content.selector.sensorId.name, 9, SENSOR_ID);

	query.conditionGroups_count = 1;
	query.conditionGroups[0].conditions_count = 1;
	strcpy_s(query.conditionGroups[0].conditions[0].identifier.name, 9, SENSOR_ID);	 
	query.conditionGroups[0].conditions[0].op = ValueOperator_LESS;
	query.conditionGroups[0].conditions[0].value = 4;
	querySim(&sim, &query);

	CuAssertIntEquals(tc, 1, getActiveQueryCount(sink));
	CuAssertIntEquals(tc, 0, getActiveQueryCount(nodeOne));
	CuAssertIntEquals(tc, 0, getActiveQueryCount(nodeTwo));

	// advance the sim, the child nodes should now receive the query
	advanceSim(&sim, PARENT_SELECTION_WAIT_TIME);
	advanceSim(&sim, BROADCAST_WAIT_TIME);

	CuAssertIntEquals(tc, 1, getActiveQueryCount(sink));
	CuAssertIntEquals(tc, 1, getActiveQueryCount(nodeOne));
	CuAssertIntEquals(tc, 1, getActiveQueryCount(nodeTwo));

	// now the child nodes process the query and send the result back to the sink	
	advanceSim(&sim, PARENT_SELECTION_WAIT_TIME);
	advanceSim(&sim, BROADCAST_WAIT_TIME);
	advanceSim(&sim, TEST_TICK_STEP);

	CuAssertIntEquals(tc, 0, getActiveQueryCount(sink)); // query has finished
	CuAssertIntEquals(tc, 0, getActiveQueryCount(nodeOne));
	CuAssertIntEquals(tc, 0, getActiveQueryCount(nodeTwo));

	// check the query result		
	CuAssertIntEquals(tc, query.queryId.shortId, sim.lastResultset.queryId.shortId);
	CuAssertIntEquals(tc, 1, sim.lastResultset.rows_count);
	CuAssertIntEquals(tc, 3, sim.lastResultset.rows[0].values[0]);
}

CuSuite* QueryCoreGetSuite() {
	CuSuite* suite = CuSuiteNew();

	SUITE_ADD_TEST(suite, TestScheduleEmptyQuery);
	SUITE_ADD_TEST(suite, TestThreeNodeNetwork);
	SUITE_ADD_TEST(suite, TestThreeNodeNetwork_SingleResult);

	return suite;
}
