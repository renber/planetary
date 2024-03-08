#include <stddef.h>
#include <stdbool.h>

#include <planetary/proto/store.pb.h>
#include <planetary/proto/query.pb.h>

#include <planetary/querytypes.h>
#include <planetary/queries.h>
#include <planetary/packet_man.h>

#include <stdlib.h>
#include <planetary/stores.h>

#include "CuTest.h"
#include "NetSim.h"
#include <planetary/stores/FrugalCore.h>
#include <planetary/stores/MemoryCore.h>
#include <planetary/stores/SpaceSavingCore.h>
#include <planetary/stores/LinearCountingCore.h>

#include "frugalHelper.h"
#include "NetSim.h"

#define StreamLength 3000
#define StructLength 40

void setupStoreCondition(Query * query)
{
	query->conditionGroupLink = ConditionLink_AND;
	query->conditionGroups_count = 1;
	query->conditionGroups[0].conditionLink = ConditionLink_AND;
	query->conditionGroups[0].conditions_count = 1;
	query->conditionGroups[0].conditions[0].op = ValueOperator_GREATER;
	query->conditionGroups[0].conditions[0].value = 0;
	strcpy_s(query->conditionGroups[0].conditions[0].identifier.name, 9, "id");
}


void initCreateQuery(Query* query, char* storeName, char* storeType, float param)
{
	query->queryId.shortId = 1;
	query->actions_count = 1;
	query->actions[0].which_content = ActionType_STORE;
	strcpy_s(query->actions[0].content.store.name, 10, storeName);
	query->actions[0].content.store.which_action = StoreType_CREATE;
	strcpy_s(query->actions[0].content.store.action.create.function, 10, storeType);
	query->actions[0].content.store.action.create.param = param;
	setupStoreCondition(query);
}


void initSendQuery(Query * query, char* storeName)
{
	query->queryId.shortId = 1;
	query->actions_count = 1;
	query->actions[0].which_content = ActionType_SELECTOR;
	query->resultTarget.which_target = TargetType_TARGETSTORE;
	strcpy_s(query->resultTarget.target.storeTarget.storeName, 10, storeName);
}


void initReadQuery(Query * query, char* storeName, float param)
{
	query->actions_count = 1;
	query->queryId.shortId = 1;
	query->actions[0].which_content = ActionType_SELECTOR;
	query->resultTarget.which_target = TargetType_TARGETSINK;
	query->actions[0].content.selector.which_source = SourceType_SOURCESTORE;
	strcpy_s(query->actions[0].content.selector.source.store.storeName, 10, storeName);
	query->actions[0].content.selector.source.store.param = param;
}


void sendTroughNetwork(NetSim * sim, Query* query)
{
	querySim(sim, query);

	advanceSim(sim, PARENT_SELECTION_WAIT_TIME);
	advanceSim(sim, BROADCAST_WAIT_TIME);
	advanceSim(sim, 1000);
}


float createSensorValue(QueryCore* core, Identifier sensorId)
{
	if (!strcmp("id", sensorId.name))
	{
		return (float)core->myId.shortId;
	}
	return (float)rand();
	//return (float)(sensorId.name[0]);
}


void initFrugalStore(Store* store)
{
	strcpy_s(store->name, _countof(store->name), "temp_med");
	strcpy_s(store->action.create.function, _countof(store->action.create.function), "frugal");
	store->which_action = StoreType_CREATE;
	store->action.create.param = 0.5;
}


void Test_Store(CuTest *tc) 
{
	//Create a frugal store message
	Store store = Store_init_default;

	initFrugalStore(&store);

	struct StoreCollection storeCollection;
	initDefaultStoreCoreCollection(&storeCollection);

	/*Create a store*/
	handleStoreMessage(&storeCollection, &store);
	int namesEqual = strcmp("temp_med", storeCollection.collection[0].name);
	
	/*Test if a store has been created*/
	CuAssertTrue(tc, !namesEqual);
	CuAssertIntEquals(tc, 1, storeCollection.inStore);

	/*Test if the store is beeing dropped*/
	store.which_action = StoreType_DROP;
	handleStoreMessage(&storeCollection, &store);
	CuAssertIntEquals(tc, 0, storeCollection.inStore);

	/*Test if a unknown store type is not created*/
	store.which_action = StoreType_CREATE;
	strcpy_s(store.action.create.function, _countof(store.action.create.function), "wrong");
	handleStoreMessage(&storeCollection, &store);
	CuAssertIntEquals(tc, 0, storeCollection.inStore);

}


void Test_StoreIntegration(CuTest *tc)
{
	NetSim sim;
	initSim(&sim);

	/*Init the network nodes*/
	QueryCore* sink = addNodeToSim(&sim, (NodeId) { 0 }, 1, 1); // node 0 with neighbors 1 and 2
	QueryCore* nodeOne = addNodeToSim(&sim, (NodeId) { 1 }, 1, 0); // node 1 with neighbor sink
	sink->getSensorValue = createSensorValue;
	nodeOne->getSensorValue = createSensorValue;

	/*Init the query for creating a store*/
	Query query = Query_init_default;
	initCreateQuery(&query, "temp", "memory", 0.5f);

	/*Send the query through the network*/
	sendTroughNetwork(&sim, &query);

	/*Test if the store has been initialised correctly*/
	//Test if a new store is created
	CuAssertIntEquals(tc, 1, nodeOne->storeCollection.inStore);
	CuAssertIntEquals(tc, 0, sink->storeCollection.inStore);

	//Test if the store has the right name
	int namesEqual = strcmp("temp", nodeOne->storeCollection.collection[0].name);
	CuAssertTrue(tc, !namesEqual);

	/*----------------------------------------------------------------*/
	/*Test if a wrongly configured store is not created*/
	strcpy_s(query.actions[0].content.store.action.create.function, 10, "wrong");

	/*Send the query through the network*/
	sendTroughNetwork(&sim, &query);

	CuAssertIntEquals(tc, 1, nodeOne->storeCollection.inStore);

	//Test if the store has the right name
	namesEqual = strcmp("temp", nodeOne->storeCollection.collection[0].name);
	CuAssertTrue(tc, !namesEqual);

	/*----------------------------------------------------------------*/
	/*Test if a store is dropped correctly*/
	query.actions[0].content.store.which_action = StoreType_DROP;
	query.conditionGroups_count = 0;

	/*Send the query through the network*/
	sendTroughNetwork(&sim, &query);

	CuAssertIntEquals(tc, 0, nodeOne->storeCollection.inStore);
}


void Test_FrugalStoreIntegration(CuTest *tc)
{
	NetSim sim;
	initSim(&sim);

	/*Init the network nodes*/
	QueryCore* sink = addNodeToSim(&sim, (NodeId) { 0 }, 2, 1, 2); // node 0 with neighbors 1 and 2
	QueryCore* nodeOne = addNodeToSim(&sim, (NodeId) { 1 }, 1, 0); // node 1 with neighbor sink
	QueryCore* nodeTwo = addNodeToSim(&sim, (NodeId) { 2 }, 1, 0); // node 1 with neighbor sink
	sink->getSensorValue = createSensorValue;
	nodeOne->getSensorValue = createSensorValue;
	nodeTwo->getSensorValue = createSensorValue;

	/*Init the query for creating a frugal store*/
	Query query = Query_init_default;
	initCreateQuery(&query, "temp_med", "frugal", 0.5f);

	/*Send the query through the network*/
	sendTroughNetwork(&sim, &query);

	//Test if the initial values are correct
	struct FrugalCore * frugalCore = (struct FrugalCore*)nodeOne->storeCollection.collection[0].structArray;
	CuAssertIntEquals(tc, 50, (int)(frugalCore->quantil * 100));
	CuAssertTrue(tc, frugalCore->firstNumber);
	CuAssertIntEquals(tc, 0, (int)(frugalCore->m));
	CuAssertIntEquals(tc, 0, (int)(frugalCore->c));
	CuAssertIntEquals(tc, 0, (int)(frugalCore->n));
	CuAssertTrue(tc, frugalCore->wasGreater);

	/*----------------------------------------------------------------*/
	/*Init the query for writing a value into the store*/
	Query query2 = Query_init_default;
	initSendQuery(&query2, "temp_med");

	/*Send the query through the network*/
	sendTroughNetwork(&sim, &query2);

	/*Test if the value reaches the store*/
	//Test if the values that can be known are correct
	CuAssertIntEquals(tc, 1, frugalCore->n);
	CuAssertTrue(tc, !frugalCore->firstNumber);

	/*----------------------------------------------------------------*/
	/*Init the query for reading a value from the store*/
	Query query3 = Query_init_default;
	initReadQuery(&query3, "temp_med", 0.0f);

	/*Send the query through the network*/
	sendTroughNetwork(&sim, &query3);

	/*Test if the return value is the same as the the value in the store*/
	CuAssertIntEquals(tc, (int)(frugalCore->m * 100), (int)(sim.lastResultset.rows[0].values[0] * 100));
}


void Test_SpaceSavingIntegration(CuTest *tc)
{
	NetSim sim;
	initSim(&sim);

	/*Init the network nodes*/
	QueryCore* sink = addNodeToSim(&sim, (NodeId) { 0 }, 2, 1, 2); // node 0 with neighbors 1 and 2
	QueryCore* nodeOne = addNodeToSim(&sim, (NodeId) { 1 }, 1, 0); // node 1 with neighbor sink
	QueryCore* nodeTwo = addNodeToSim(&sim, (NodeId) { 2 }, 1, 0); // node 1 with neighbor sink
	sink->getSensorValue = createSensorValue;
	nodeOne->getSensorValue = createSensorValue;
	nodeTwo->getSensorValue = createSensorValue;

	/*Init the query for creating a frugal store*/
	Query query = Query_init_default;
	initCreateQuery(&query, "topK", "spaceSave", 0.5f);

	/*Send the query through the network*/
	sendTroughNetwork(&sim, &query);

	/*Test if the initial values are correct*/
	struct SpaceSavingCore * core = (struct SpaceSavingCore*)nodeOne->storeCollection.collection[0].structArray;
	for (int i = 0; i < SLOTS; i++)
	{
		CuAssertIntEquals(tc, 0x7FFF, core->slots[i].element);
		CuAssertIntEquals(tc, 0, core->slots[i].amount);
		CuAssertIntEquals(tc, 0, core->slots[i].error);
	}

	/*----------------------------------------------------------------*/
	/*Init the query for writing a value into the store*/
	Query query2 = Query_init_default;
	initSendQuery(&query2, "topK");

	/*Send the query through the network*/
	sendTroughNetwork(&sim, &query2);

	/*Test if the counter is updated properly*/
	for (int i = 1; i < SLOTS; i++)
	{
		CuAssertIntEquals(tc, 0x7FFF, core->slots[i].element);
		CuAssertIntEquals(tc, 0, core->slots[i].amount);
		CuAssertIntEquals(tc, 0, core->slots[i].error);
	}
	CuAssertIntEquals(tc, 0, core->slots[0].error);
	CuAssertIntEquals(tc, 1, core->slots[0].amount);

	advanceSim(&sim, 1000);

	/*----------------------------------------------------------------*/
	/*Init the query for writing a value into the store*/
	Query query3 = Query_init_default;
	initSendQuery(&query3, "topK");

	/*Send the query through the network*/
	sendTroughNetwork(&sim, &query3);


	/*----------------------------------------------------------------*/
	/*Init the query for reading a value from the store*/
	Query query4 = Query_init_default;
	initReadQuery(&query4, "topK", -2.0f);

	/*Send the query through the network*/
	sendTroughNetwork(&sim, &query4);

	/*Test if the return value is the same as the the value in the store*/
	CuAssertIntEquals(tc, core->slots[0].element, sim.lastResultset.rows[0].values[0]);
	CuAssertIntEquals(tc, core->slots[1].element, sim.lastResultset.rows[1].values[0]);
}


void Test_MemoryStoreIntegration(CuTest *tc)
{
	NetSim sim;
	initSim(&sim);

	/*Init the network nodes*/
	QueryCore* sink = addNodeToSim(&sim, (NodeId) { 0 }, 2, 1, 2); // node 0 with neighbors 1 and 2
	QueryCore* nodeOne = addNodeToSim(&sim, (NodeId) { 1 }, 1, 0); // node 1 with neighbor sink
	QueryCore* nodeTwo = addNodeToSim(&sim, (NodeId) { 2 }, 1, 0); // node 1 with neighbor sink
	sink->getSensorValue = createSensorValue;
	nodeOne->getSensorValue = createSensorValue;
	nodeTwo->getSensorValue = createSensorValue;

	/*Init the query for creating a memory store*/
	Query query = Query_init_default;
	initCreateQuery(&query, "temp", "memory", 0.5f);

	/*Send the query through the network*/
	sendTroughNetwork(&sim, &query);

	//Test if the initial values are correct
	struct MemoryCore * memoryCore = (struct MemoryCore*)nodeOne->storeCollection.collection[0].structArray;
	CuAssertIntEquals(tc, 0, memoryCore->index);
	CuAssertIntEquals(tc, 0, memoryCore->lastWrittenIndex);
	for (int i = 0; i < MemorySize; i++)
	{
		CuAssertIntEquals(tc, 0, memoryCore->memory[i]);
	}

	/*----------------------------------------------------------------*/
	/*Init the query for writing a value into the store*/
	Query query2 = Query_init_default;
	initSendQuery(&query2, "temp");

	/*Send the query through the network*/
	sendTroughNetwork(&sim, &query2);

	/*Test if the value reaches the store*/
	//Test if the values that can be known are correct
	CuAssertIntEquals(tc, 1, memoryCore->index);
	CuAssertIntEquals(tc, 0, memoryCore->lastWrittenIndex);

	/*----------------------------------------------------------------*/
	/*Init the query for reading a value from the store*/
	Query query3 = Query_init_default;
	query3.actions_count = 1;
	query3.queryId.shortId = 3;
	query3.actions[0].which_content = ActionType_SELECTOR;
	query3.resultTarget.which_target = TargetType_TARGETSINK;
	query3.actions[0].content.selector.which_source = SourceType_SOURCESTORE;
	strcpy_s(query3.actions[0].content.selector.source.store.storeName, 10, "temp");

	/*Send the query through the network*/
	sendTroughNetwork(&sim, &query3);
	advanceSim(&sim, 1000);
	

	/*Test if the return value is the same as the the value in the store*/
	CuAssertIntEquals(tc, memoryCore->memory[0], sim.lastResultset.rows[0].values[0]);
}


void Test_LinearCountingIntegration(CuTest *tc)
{
	NetSim sim;
	initSim(&sim);

	/*Init the network nodes*/
	QueryCore* sink = addNodeToSim(&sim, (NodeId) { 0 }, 2, 1, 2); // node 0 with neighbors 1 and 2
	QueryCore* nodeOne = addNodeToSim(&sim, (NodeId) { 1 }, 1, 0); // node 1 with neighbor sink
	QueryCore* nodeTwo = addNodeToSim(&sim, (NodeId) { 2 }, 1, 0); // node 1 with neighbor sink
	sink->getSensorValue = createSensorValue;
	nodeOne->getSensorValue = createSensorValue;
	nodeTwo->getSensorValue = createSensorValue;

	/*Init the query for creating a linearCounting store*/
	Query query = Query_init_default;
	initCreateQuery(&query, "temp", "linCount", 0.5f);

	/*Send the query through the network*/
	sendTroughNetwork(&sim, &query);

	//Test if the initial values are correct
	struct LinearCountingCore * core = (struct LinearCountingCore*)nodeOne->storeCollection.collection[0].structArray;

	CuAssertIntEquals(tc, 0, core->count);
	CuAssertIntEquals(tc, 0, core->numberOfElements);
	for (int i = 0; i < SIZE; i++)
	{
		CuAssertIntEquals(tc, 0, core->bitMap[i]);
	}

	/*----------------------------------------------------------------*/
	/*Init the query for writing a value into the store*/
	Query query2 = Query_init_default;
	initSendQuery(&query2, "temp");

	/*Send the query through the network*/
	sendTroughNetwork(&sim, &query2);

	CuAssertIntEquals(tc, 0, core->count);
	CuAssertIntEquals(tc, 1, core->numberOfElements);

	/*----------------------------------------------------------------*/
	/*Init the query for reading a value from the store*/
	Query query3 = Query_init_default;
	initReadQuery(&query3, "temp", 0.0f);

	/*Send the query through the network*/
	sendTroughNetwork(&sim, &query3);

	CuAssertTrue(tc, sim.lastResultset.rows[0].values[0]);
}


void Test_RepeatedStoreIntegration(CuTest *tc)
{
	NetSim sim;
	initSim(&sim);

	/*Init the network nodes*/
	QueryCore* sink = addNodeToSim(&sim, (NodeId) { 0 }, 1, 1); // node 0 with neighbors 1 and 2
	QueryCore* nodeOne = addNodeToSim(&sim, (NodeId) { 1 }, 1, 0); // node 1 with neighbor sink
	sink->getSensorValue = createSensorValue;
	nodeOne->getSensorValue = createSensorValue;

	/*Init the query for creating a frugal store*/
	Query query = Query_init_default;
	initCreateQuery(&query, "temp_med", "frugal", 0.5f);

	/*Send the query through the network*/
	sendTroughNetwork(&sim, &query);

	/*Init the query for writing a value into the store*/
	Query query2 = Query_init_default;
	initSendQuery(&query2, "temp_med");
	query2.periodInSec = 1;

	/*Send the query through the network*/

	sendTroughNetwork(&sim, &query2);

	for (int i = 0; i < 10; i++)
	{
		advanceSim(&sim, 1000);
	}
	struct FrugalCore * frugalCore = (struct FrugalCore*)nodeOne->storeCollection.collection[0].structArray;
	CuAssertIntEquals(tc, 11, frugalCore->n);
	CuAssertFalse(tc, frugalCore->firstNumber);
	
}


CuSuite* StoreGetSuite() 
{
	CuSuite* suite = CuSuiteNew();

	
	SUITE_ADD_TEST(suite, Test_Store);
	SUITE_ADD_TEST(suite, Test_StoreIntegration);
	SUITE_ADD_TEST(suite, Test_FrugalStoreIntegration);
	SUITE_ADD_TEST(suite, Test_MemoryStoreIntegration);
	SUITE_ADD_TEST(suite, Test_SpaceSavingIntegration);
	SUITE_ADD_TEST(suite, Test_LinearCountingIntegration);
	SUITE_ADD_TEST(suite, Test_RepeatedStoreIntegration);

	return suite;
}