#ifndef _PACKET_MAN_H_
#define _PACKET_MAN_H_

#include <planetary/querytypes.h>
#include <planetary/proto/querybroadcast.pb.h>
#include <planetary/proto/queryparentsel.pb.h>
#include <planetary/proto/resultset.pb.h>
#include <planetary/proto/cancelquery.pb.h>

void handlePlanetaryPacket(QueryCore* core, unsigned char* packetData, int packetLen);

int createPlanetaryPacket(QueryCore* core, PacketToSend* packetInfo, unsigned char* buf, int bufLen);

bool handleQueryBroadcastPacket(QueryCore* core, QueryBroadcast* packet);

bool handleParentSelectionPacket(QueryCore* core, QueryParentSel* packet);

bool handleResultPacket(QueryCore* core, Resultset* packet);

bool handleCancelQueryPacket(QueryCore* core, CancelQueryMessage* packet);

#endif
