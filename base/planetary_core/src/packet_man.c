#include <planetary/packet_man.h>
#include <planetary/querytypes.h>
#include <planetary/queries.h>
#include <planetary/grouping.h>
#include <planetary/conditions.h>

#include <planetary/proto/planetarymsg.pb.h>
#include <planetary/proto/query.pb.h>
#include <planetary/proto/querybroadcast.pb.h>
#include <planetary/proto/queryparentsel.pb.h>
#include <planetary/proto/resultset.pb.h>

#include <nanopb/pb.h>
#include <nanopb/pb_encode.h>
#include <nanopb/pb_decode.h>

void handlePlanetaryPacket(QueryCore* core, unsigned char* packetData, int packetLen)
{
	pb_istream_t stream = pb_istream_from_buffer(packetData, packetLen);
	PlanetaryMessage message;

	if (!pb_decode(&stream, PlanetaryMessage_fields, &message))
		return false;
	
    switch(message.which_payload)
    {
        case PlanetaryMessage_broadcast_tag:
			handleQueryBroadcastPacket(core, &message.payload.broadcast);
            break;
        case PlanetaryMessage_resultset_tag:
            handleResultPacket(core, &message.payload.resultset);
			break;
		case PlanetaryMessage_parentsel_tag:
			handleParentSelectionPacket(core, &message.payload.parentsel);
			break;
		case PlanetaryMessage_cancelquery_tag:
			handleCancelQueryPacket(core, &message.payload.cancelquery);			
			break;
    }
}

int createPlanetaryPacket(QueryCore* core, PacketToSend* packetInfo, unsigned char* buf, int bufLen)
{
	pb_ostream_t stream = pb_ostream_from_buffer(buf, MAX_PACKET_SIZE);

	PlanetaryMessage msg = PlanetaryMessage_init_default;

	// send query results to parent
	switch (packetInfo->what)
	{
	case PlanetaryMessage_resultset_tag:
		msg.which_payload = PlanetaryMessage_resultset_tag;
		msg.payload.resultset = packetInfo->querySlot->resultset;
		break;
	case PlanetaryMessage_cancelquery_tag:
		msg.which_payload = PlanetaryMessage_cancelquery_tag;
		msg.payload.cancelquery.queryId = packetInfo->querySlot->id;
		break;
	case PlanetaryMessage_parentsel_tag:
		msg.which_payload = PlanetaryMessage_parentsel_tag;
		msg.payload.parentsel.queryId = packetInfo->querySlot->id;
		msg.payload.parentsel.childId = core->myId;
		break;
	case PlanetaryMessage_broadcast_tag:
		msg.which_payload = PlanetaryMessage_broadcast_tag;
		msg.payload.broadcast.parentId = core->myId;
		msg.payload.broadcast.distanceToSink = packetInfo->querySlot->parentDistanceToSink + 1;
		msg.payload.broadcast.parentIsPartOfQuery = nodeFulfillsStaticConditions(core, core->myId, packetInfo->querySlot->queryData.conditionGroups, packetInfo->querySlot->queryData.conditionGroups_count, packetInfo->querySlot->queryData.conditionGroupLink);
		msg.payload.broadcast.query = packetInfo->querySlot->queryData;
		break;
	default:
		return false;
	}	

	if (!pb_encode(&stream, PlanetaryMessage_fields, &msg))
		return false;

	return stream.bytes_written;

}

bool handleQueryBroadcastPacket(QueryCore* core, QueryBroadcast* packet)
{	
	return scheduleQueryFromBroadcast(core, packet) != NULL;
}

bool handleParentSelectionPacket(QueryCore* core, QueryParentSel* packet)
{
	QueryId query_id = packet->queryId;
	QuerySlot* query;

	if (findQuery(core, query_id, &query))
	{
		if (query->queryChildren_count < MAX_CHILDREN)
		{
			query->queryChildren[query->queryChildren_count] = packet->childId;
			query->queryChildren_count++;

			query->remainingResults++;
			return true;
		}
	}	
	
	return false;	
}

bool handleResultPacket(QueryCore* core, Resultset* packet)
{
	// query id
	QueryId query_id = packet->queryId;
	QuerySlot* query;

	if (findQuery(core, query_id, &query)) {
		if (query->remainingResults > 0)
			query->remainingResults--;

		// todo: use define for limit		
		int maxResultRows = 20;		

		if (query->resultset.rows_count + packet->rows_count > maxResultRows)
		{
			// run combine results which might free some rows, then try again
			combineResults(query);
			if (query->resultset.rows_count + packet->rows_count > maxResultRows) {
				return false;
			}
		}
		
		// copy received results to query resultset
		for (int r = 0; r < packet->rows_count; r++) {
			query->resultset.rows[query->resultset.rows_count] = packet->rows[r];
			query->resultset.rows_count++;
		}

		return true;
	}

	return false;
}

bool handleCancelQueryPacket(QueryCore* core, CancelQueryMessage* packet)
{
	cancelQuery(core, packet->queryId);
	return true;
}