
#include "NetSim.h"

#include <planetary/queries.h>
#include <planetary/packetqueue.h>
#include <planetary/packet_man.h>
#include <planetary/proto/planetarymsg.pb.h>
#include <planetary/proto/resultset.pb.h>

#include <nanopb/pb_encode.h>

void initSim(NetSim* sim)
{
	sim->nodes_count = 0;

	Resultset emptyResultset = Resultset_init_default;
	sim->lastResultset = emptyResultset;
}

QueryCore* getSimNodeById(NetSim* sim, NodeId nodeId)
{
	for (int i = 0; i < sim->nodes_count; i++)
	{
		if (sim->nodes[i].queryCore.myId.shortId == nodeId.shortId)
			return &sim->nodes[i].queryCore;
	}

	return NULL;
}

void querySim(NetSim* sim, Query* query)
{
	if (sim->nodes_count > 0)
	{
		PlanetaryMessage msg = PlanetaryMessage_init_default;		
		msg.which_payload = PlanetaryMessage_broadcast_tag;

		msg.payload.broadcast.distanceToSink = 0;
		msg.payload.broadcast.parentId.shortId = 0;
		msg.payload.broadcast.parentIsPartOfQuery = true;
		msg.payload.broadcast.query = *query;

		unsigned char packetData[MAX_PACKET_SIZE];		
		pb_ostream_t stream = pb_ostream_from_buffer(packetData, MAX_PACKET_SIZE);
		if (!pb_encode(&stream, PlanetaryMessage_fields, &msg))
			return false;
		
		handlePlanetaryPacket(&sim->nodes[0].queryCore, packetData, stream.bytes_written);
	}
}

void advanceSim(NetSim* sim, int ticks)
{
	// advance all query cores
	for (int i = 0; i < sim->nodes_count; i++) {
		advanceQueryCore(&sim->nodes[i].queryCore, ticks);
	}

	unsigned char buf[NET_SIM_MAX_PACKET_LEN];

	// send/receive packets
	for (int i = 0; i < sim->nodes_count; i++) {

		PacketQueue* curQueue = &sim->nodes[i].queryCore.packetQueue;

		while (!queueIsEmpty(curQueue))
		{
			// create packet
			PacketToSend* p = queuePeekHead(curQueue);			
			int plen = createPlanetaryPacket(&sim->nodes[i].queryCore, p, buf, NET_SIM_MAX_PACKET_LEN);
			// directly inject the packet to the target node
			if (p->noOfReceivers == SEND_BROADCAST)
			{
				for (int n = 0; n < sim->nodes[i].neighbors_count; n++) {					
					QueryCore* targetNode = getSimNodeById(sim, sim->nodes[i].neighbors[n]);
					// do not broadcast to the sender itself
					if (targetNode != NULL && targetNode != &sim->nodes[i].queryCore)
					{
						handlePlanetaryPacket(targetNode, buf, plen);
					}
				}				
			}
			else
			{
				if (p->what == PlanetaryMessage_resultset_tag && sim->nodes[i].queryCore.mode == AM_SINK)
				{
					// results arrived at the sink
					sim->lastResultset = p->querySlot->resultset;
				}
				else 
				{
					QueryCore* targetNode = getSimNodeById(sim, p->receivers[p->curPos]);
					if (targetNode != NULL)
					{
						handlePlanetaryPacket(targetNode, buf, plen);
					}
				}
			}

			queueNext(curQueue);
		}
	}
}

float getSensorValue_NodeId_test(QueryCore* core, int sensorId) {
	return (int)core->myId.shortId;
}

QueryCore* addNodeToSim(NetSim* sim, NodeId nodeId, int neighbors_count, ...)
{
	int nidx = sim->nodes_count;
	sim->nodes_count++;
	// the first node which is added is considered to be the sink
	initQueryCore(&sim->nodes[nidx].queryCore, nodeId, nidx == 0 ? AM_SINK : AM_NODE);

	sim->nodes[nidx].queryCore.getSensorValue = getSensorValue_NodeId_test;

	// init routing table
	sim->nodes[nidx].neighbors_count = neighbors_count;

	va_list ap;
	va_start(ap, neighbors_count);
	for (int i = 0; i < neighbors_count; i++)
	{
		int neighborId = va_arg(ap, int);
		sim->nodes[nidx].neighbors[i].shortId = neighborId;
	}	
	va_end(ap);

	return &sim->nodes[nidx].queryCore;
}