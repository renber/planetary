#ifndef QUERYTYPES_H_INCLUDED
#define QUERYTYPES_H_INCLUDED

#include <planetary/typedefs.h>
#include <planetary/planetary_config.h>

#include <planetary/proto/planetarymsg.pb.h>
#include <planetary/proto/identifier.pb.h>
#include <planetary/proto/query.pb.h>
#include <planetary/proto/resultset.pb.h>
#include <planetary/proto/store.pb.h>
#include <planetary/stores.h>

enum APPMODE {AM_SINK, AM_NODE};

enum QuerySlotState { STATE_UNUSED, STATE_PARENT_SELECTION, STATE_FIND_CHILDREN, STATE_WAIT_FOR_RESULTS, STATE_DORMANT };

// broadcast identifier
#define SEND_BROADCAST 255

typedef struct group_by_struct
{
  float values[MAX_QUERY_SELECTS];
} GroupBy;

/// ---------------------------------
/// The query object which holds data
/// about a query in the network
/// ---------------------------------
typedef struct QuerySlot_t {
	enum QuerySlotState state; // is the query slot used?	

    QueryId id; // query identifier (must be unique)

	NodeId parentId; // id of the parent node of this query (regarding the routing tree created for this query)    
	bool parentPartOfQuery;
	uint16 parentDistanceToSink;

	int stateWaitTime; // time left until the query parent is fixed    

    bool periodic; // is the query periodic?    
    int nextRoundIn; // remaining time to wait before this query is reactivated

    uint8 queryChildren_count; // number of children the query has been accepted by
    NodeId queryChildren[MAX_CHILDREN];	

    uint8 remainingResults; // the number of children which have yet to answer in this round

	Query queryData;
	Resultset resultset;
} QuerySlot;

// -----------
// Queue types
// -----------
typedef struct packettosend_t
{
	// one of PlanetaryMessage_*_tags
  int what;

  unsigned char frameID; // frameID for ack  
  unsigned char curPos;
  // number of receivers in receivers array or SEND_BROADCAST for broadcast
  unsigned char noOfReceivers;
  NodeId receivers[MAX_PACKET_RECEIVERS];  
  QuerySlot* querySlot; // pointer to corresponding query slot
  QueryId argument;

} PacketToSend;

typedef struct packetqueue_t
{
    uint8 q_Head;
    uint8 q_Tail;

    PacketToSend elements[MAX_QUEUE_ELEMENTS];
} PacketQueue;

// -----------
// Main object
// -----------

typedef struct QueryCore_t QueryCore;

/// Core object which manages all running queries
typedef struct QueryCore_t {

	NodeId myId;

    enum APPMODE mode; // AppMode

	QuerySlot queries[MAX_RUNNING_QUERIES];
    PacketQueue packetQueue;	
	struct StoreCollection storeCollection;

	// a user settable value, not used by planetary
	void* tag;

    // callback function pointers
    float (*getSensorValue)(QueryCore* core, Identifier sensorId);
    // returns if the attribute with the given id is a static attribute
    bool (*isStaticAttribute)(QueryCore* core, Identifier sensorId);
    // get the value of a static attribute for the sensor with id
    float (*getStaticAttributeValue)(QueryCore* core, NodeId nodeId, Identifier sensorId);

    // act an actuator
    void (*act)(QueryCore* core, Identifier actorId, uint8 parameter);

} QueryCore;

#endif // QUERYTYPES_H_INCLUDED
