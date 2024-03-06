
#include <string.h>

#include <planetary/queries.h>
#include <planetary/dummy_callbacks.h>
#include <planetary/conditions.h>
#include <planetary/grouping.h>
#include <planetary/utils.h>

#include <planetary/proto/planetarymsg.pb.h>
#include <planetary/proto/resultset.pb.h>

void initQueryCore(QueryCore* core, NodeId nodeId, enum APPMODE mode)
{
	core->myId = nodeId;
    core->mode = mode;

    int i;
	for (i = 0; i < MAX_RUNNING_QUERIES; i++)
		core->queries[i].state = STATE_UNUSED;

    queueInit(&(core->packetQueue));    

    // assign dummy functions (to avoid null ptr exceptions)
    core->getSensorValue = getSensorValue_dummy;
    core->getStaticAttributeValue = getStaticAttributeValue_dummy;
    core->isStaticAttribute = isStaticAttribute_dummy;
    core->act = act_dummy;
}

// returns the number of running queries
int getActiveQueryCount(QueryCore* core)
{
  int i;
  int cnt = 0;
  for(i = 0; i < MAX_RUNNING_QUERIES; i++)
  {
    if (core->queries[i].state != STATE_UNUSED)
      cnt++;
  }

  return cnt;
}

int findRunningQuery(QueryCore* core, QueryId query_id)
{
    int i;
  for(i = 0; i < MAX_RUNNING_QUERIES; i++)
   if (core->queries[i].state != STATE_UNUSED && core->queries[i].id.shortId == query_id.shortId)
    return i;

  return -1;
}

bool findQuery(QueryCore* core, QueryId query_id, QuerySlot** outQuery)
{
   int idx = findRunningQuery(core, query_id);
   if (idx == -1)
    return false;

   *outQuery = &core->queries[idx];
   return true;
}

int findFreeQuerySlot(QueryCore* core)
{
    int i;
  for(i = 0; i < MAX_RUNNING_QUERIES; i++)
   if (core->queries[i].state == STATE_UNUSED)
    return i;

  return -1;
}

void informQueryParent(QueryCore* core, QuerySlot* query)
{
	PacketToSend* p = queuePut(&core->packetQueue);
	p->noOfReceivers = 1;
	p->receivers[0] = query->parentId;
	p->what = PlanetaryMessage_parentsel_tag;
	p->curPos = 0;
	p->querySlot = query;
}

bool advanceQueryCore(QueryCore* core, int ticks)
{
    unsigned char changes = 0;

    QuerySlot* queries = core->queries;

    // can some queries be finished?
    int r;
    for(r = 0; r < MAX_RUNNING_QUERIES; r++)
    {
		if (queries[r].state == STATE_PARENT_SELECTION)
		{
			queries[r].stateWaitTime -= ticks;
			if (queries[r].stateWaitTime <= 0)
			{
				// inform our to-be-parent the information that we chose them as parent node for this query
				if (core->mode == AM_NODE) { // the sink does not need a parent
					informQueryParent(core, &queries[r]);
				}

				// re-broadcast query
				sendQueryToChildren(core, &queries[r]);
				changes = true;

				// PARENT SELECTION PHASE has finished, we are now rebroadcasting the query to find children
				queries[r].stateWaitTime = BROADCAST_WAIT_TIME;
				queries[r].state = STATE_FIND_CHILDREN;
			}
		} else
		if (queries[r].state == STATE_FIND_CHILDREN)
		{
			queries[r].stateWaitTime -= ticks;
			if (queries[r].stateWaitTime <= 0)
			{
				// BROADCASTING PHASE has finished, we are now waiting for child results
				// or finish the query right away if there are no children				
				queries[r].state = STATE_WAIT_FOR_RESULTS;
			}
		}

        if (queries[r].state == STATE_WAIT_FOR_RESULTS)
        {
            if (queries[r].periodic && queries[r].queryChildren_count == 0 && queries[r].nextRoundIn > 0)
            {
                queries[r].nextRoundIn -= ticks;
                if (queries[r].nextRoundIn < 0)
                    queries[r].nextRoundIn = 0;
            }

			if (queries[r].remainingResults == 0 || (queries[r].periodic && queries[r].nextRoundIn == 0))
            {
                finishQuery(core, &queries[r]);
                changes = true;
            }
        }
    }

    return changes;
}

// adds the result of this node to the given query
void addMyQueryResult(QueryCore *core, QuerySlot* query)
{  
  // a single line can always be treated as SEL_SINGLE
  if (query->resultset.rows_count >= MAX_QUERY_RESULTS) // no free answering slots
   return;

  // are the query conditions satisfied?
  int i;  
  if (!evaluateConditions(core, query->queryData.conditionGroups, query->queryData.conditionGroups_count, query->queryData.conditionGroupLink))
    return; // the conditions are not satisfied

  // create result set
  query->resultset.rows[query->resultset.rows_count].numberOfNodes = 1; // the result set originates only from this node
  query->resultset.rows[query->resultset.rows_count].values_count = 0;

  for(i = 0; i < query->queryData.actions_count; i++)
  {
	  if (query->queryData.actions[i].which_content == ActionType_SELECTOR) {
		  query->resultset.rows[query->resultset.rows_count].values[i] = core->getSensorValue(core, query->queryData.actions[i].content.selector.sensorId);
		  query->resultset.rows[query->resultset.rows_count].values_count++;
	  }
  }

  query->resultset.rows_count++; // now we have one result set more

  // actuate because the conditions are satisfied
  for(i = 0; i < query->queryData.actions_count; i++)
  {
	  if (query->queryData.actions[i].which_content == ActionType_ACTOR)
		core->act(core, query->queryData.actions[i].content.actor.actorId, query->queryData.actions[i].content.actor.param);
  }
}

// sends the result to the parent node
// use combineResults(..) beforehand if grouping / aggregation is wanted
void sendQueryResult(QueryCore* core, QuerySlot* query)
{
    // enqueue sending
    PacketToSend* p = queuePut(&(core->packetQueue));
    p->querySlot = query;
    p->what = PlanetaryMessage_resultset_tag;
    p->curPos = 0;
    p->noOfReceivers = 1;
	p->receivers[0] = query->parentId;
}

// finishing a query
void finishQuery(QueryCore* core, QuerySlot* query)
{
    // add this node's result line
    addMyQueryResult(core, query);
    // combine the existing results
    #if ALLOW_GROUPING == 1
    combineResults(query);
    #endif

    query->state = STATE_DORMANT;

  if (core->mode == AM_NODE)
  {
    // if we are a node send all results to the parent node
    sendQueryResult(core, query);
  } else
  {
    sendQueryResult(core, query);

    if (query->periodic)
      awakeQuery(query);
    else
      // query finished
      query->state = STATE_WAIT_FOR_RESULTS;
  }
}

QuerySlot* scheduleQueryFromBroadcast(QueryCore* core, QueryBroadcast* queryBroadcast)
{
	int qidx = findRunningQuery(core, queryBroadcast->query.queryId);
	QuerySlot* slot;
	if (qidx == -1)
	{
		slot = scheduleQuery(core, &queryBroadcast->query);
		if (slot != NULL) {
			slot->parentId = queryBroadcast->parentId;
			slot->parentDistanceToSink = queryBroadcast->distanceToSink;
			slot->parentPartOfQuery = queryBroadcast->parentIsPartOfQuery;	

			slot->state = STATE_PARENT_SELECTION;

			if (core->mode == AM_SINK)
				slot->stateWaitTime = 0;
			else
				slot->stateWaitTime = PARENT_SELECTION_WAIT_TIME;
		}
	}
	else
	{
		slot = &core->queries[qidx];

		// can we still change our parent?
		if (slot->state == STATE_PARENT_SELECTION)
		{					
			// is the broadcasting node better than the node we currently have?
			if (!slot->parentPartOfQuery && queryBroadcast->parentIsPartOfQuery)
			{
				slot->parentId = queryBroadcast->parentId;
				slot->parentDistanceToSink = queryBroadcast->distanceToSink;
				slot->parentPartOfQuery = true;
			}
			else if (slot->parentPartOfQuery && queryBroadcast->parentIsPartOfQuery)
			{
				if (queryBroadcast->distanceToSink < slot->parentDistanceToSink)
				{
					slot->parentId = queryBroadcast->parentId;
					slot->parentDistanceToSink = queryBroadcast->distanceToSink;
				}
			}
		}
	}
}

QuerySlot* scheduleQuery(QueryCore* core, Query* queryData)
{
	// Query ID already known? -> reuse / restart
	int qidx = findRunningQuery(core, queryData->queryId);
	if (qidx == -1)	
		qidx = findFreeQuerySlot(core);	
	if (qidx == -1)
		return NULL; // no free space
	
	QuerySlot* slot = &core->queries[qidx];
	
	// copy query data to slo
	slot->queryData = *queryData;	

	slot->id = queryData->queryId;
	slot->resultset.queryId = slot->id;
	slot->resultset.rows_count = 0;

	slot->periodic = slot->queryData.periodInSec > 0;	  

	slot->queryChildren_count = 0;	
	slot->remainingResults = 0;
	
	slot->state = STATE_WAIT_FOR_RESULTS;
	slot->stateWaitTime = 0;

    return slot;
}

// sends the query packet to the available children if they pass the static filter conditions
// returns the number of children to which a packet was sent (only i not broadcasted)
uint8 sendQueryToChildren(QueryCore* core, QuerySlot* query)
{
	int i;
	bool doSend;
	PacketToSend* p = NULL;
	query->queryChildren_count = 0;

	// queries are broadcasted
	p = queuePut(&(core->packetQueue));
	p->noOfReceivers = SEND_BROADCAST;	
	p->querySlot = query;
	p->what = PlanetaryMessage_broadcast_tag;
	p->curPos = 0;

	return 0;

	/*
	RoutingTableEntry* routing_table = core->routingTable.entries;

	for (i = 0; i < ROUTING_TABLE_SIZE; i++)
	{
		// do not send the packet to the parent node
		// do not send it to a child node if it does not fulfill the conditions
		// unless one of the child's children fulfills the static conditions
		if (routing_table[i].active) // && routing_table[i].gateway != core->routingTable.parentAddress
		{
			// did we already send to this gateway?
			if (p != NULL && a_contains(p->receivers, p->noOfReceivers, routing_table[i].gatewayId))
				continue; // we do not need to check the conditions

			doSend = false;
			doSend = nodeFulfillsStaticConditions(core, routing_table[i].destinationId, query->queryData.conditionGroups, query->queryData.conditionGroups_count, query->queryData.conditionGroupLink);

			if (doSend)
			{
				// add node to receivers list
				if (p == NULL) {
					p = queuePut(&(core->packetQueue));
					p->querySlot = query;
					p->what = ST_QUERY;
					p->curPos = 0;
					p->noOfReceivers = 0;
				}

				p->receivers[p->noOfReceivers] = routing_table[i].gatewayId;
				p->noOfReceivers++;

				// save this gateway node
				query->sentToChildren[query->noSentChildren] = routing_table[i].gatewayId;
				query->noSentChildren++;
			}
		}
	}

	// packets will be sent at a later point in time
	return p == NULL ? 0 : p->noOfReceivers;
	*/
}

// reactivates a dormant query
void awakeQuery(QuerySlot* query)
{
  if (query->state = STATE_DORMANT && query->periodic)
  {
        query->remainingResults = query->queryChildren_count;
        if (query->queryChildren_count == 0)
          query->nextRoundIn = query->queryData.periodInSec;
        query->resultset.rows_count = 0; // reset results
        query->state = STATE_WAIT_FOR_RESULTS;
  }
}

void cancelQuery(QueryCore* core, QueryId queryId)
{
	QuerySlot* query = NULL;
    PacketToSend* p;
    int i;

    for(i = 0; i < MAX_RUNNING_QUERIES; i++)
    {
	if (core->queries[i].state == STATE_WAIT_FOR_RESULTS && core->queries[i].id.shortId == queryId.shortId)
	{
		core->queries[i].state = STATE_UNUSED;
        query = &(core->queries[i]);
        break;
	}
    }

    // send to all children, which have received the query in the propagation phase
    if (query != NULL)
    {
       if (query->queryChildren_count > 0)
       {
          p = queuePut(&(core->packetQueue));
          p->what = PlanetaryMessage_cancelquery_tag;
          p->noOfReceivers = query->queryChildren_count;
          p->argument = query->id;

          for(i = 0; i < query->queryChildren_count; i++)
          {
              p->receivers[i] = query->queryChildren[i];
          }
       }
    }
}
