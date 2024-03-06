
#ifndef NETSIM_H_INCLUDED
#define NETSIM_H_INCLUDED

#include <stdarg.h>

#include <planetary/proto/identifier.pb.h>
#include <planetary/querytypes.h>

#define NET_SIM_MAX_NODES 10
#define NET_SIM_MAX_PACKET_LEN 255

#define NET_SIM_MAX_NEIGHBOR_COUNT 10

typedef struct SimNode_t{
	int neighbors_count;
	NodeId neighbors[NET_SIM_MAX_NEIGHBOR_COUNT];

	QueryCore queryCore;	
} SimNode;

// holds information about a simulated network of PLANetary nodes
typedef struct NetSim_t {
	int nodes_count;
	SimNode nodes[NET_SIM_MAX_NODES];

	Resultset lastResultset;
} NetSim;

void initSim(NetSim* sim);

// sends a query to the sink nod eof the simulation
void querySim(NetSim* sim, Query* query);

void advanceSim(NetSim* sim, int ticks);

QueryCore* getSimNodeById(NetSim* sim, NodeId nodeId);

QueryCore* addNodeToSim(NetSim* sim, NodeId nodeId, int neighbors_count, ...);

#endif