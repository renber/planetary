#ifndef QUERIES_H_INCLUDED
#define QUERIES_H_INCLUDED

#include <planetary/querytypes.h>
#include <planetary/typedefs.h>
#include <planetary/planetary_config.h>
#include <planetary/packetqueue.h>

#include <planetary/proto/identifier.pb.h>
#include <planetary/proto/query.pb.h>
#include <planetary/proto/querybroadcast.pb.h>

// Initializes a query core
// has to be called before the core can be used
void initQueryCore(QueryCore* core, NodeId nodeId, enum APPMODE mode);

int getActiveQueryCount(QueryCore* core);

// executes tasks of the query system
// returns true if some queries have been affected
bool advanceQueryCore(QueryCore* core, int ticks);

// finishes a query
void finishQuery(QueryCore* core, QuerySlot* query);

// cancel the given query
void cancelQuery(QueryCore* core, QueryId queryId);

// schedules the query which is part of the QueryBroadcast or if the query already is known to this node
// checks, if the query tree can be optimized by changing the parent
// when the query is new, state is set to STATE_PARENT_SELECTION
QuerySlot* scheduleQueryFromBroadcast(QueryCore* core, QueryBroadcast* queryBroadcast);

// schedules the given query to a free slot (or to an existing slot if a query with the given id already exists)
// and sets the state to STATE_WAIT_FOR_RESULTS
QuerySlot* scheduleQuery(QueryCore* core, Query* queryData);

// return the index of the query with the given id
int findRunningQuery(QueryCore* core, QueryId query_id);

// get a pointer to the running query with the given id
bool findQuery(QueryCore* core, QueryId query_id, QuerySlot** outQuery);

// return a free query slot
int findFreeQuerySlot(QueryCore* core);

// propagate the query
uint8 sendQueryToChildren(QueryCore* core, QuerySlot* query);

// reactivate a dormant periodic query
void awakeQuery(QuerySlot* query);

#endif // QUERIES_H_INCLUDED
