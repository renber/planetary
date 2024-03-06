#ifndef PACKETQUEUE_H_INCLUDED
#define PACKETQUEUE_H_INCLUDED

#include <planetary/typedefs.h>
#include <planetary/queries.h>
#include <planetary/planetary_config.h>

void queueInit(PacketQueue* queue);

bool queueIsEmpty(PacketQueue* queue);

PacketToSend* queuePut(PacketQueue* queue);

// return the head of the queue without removing it
PacketToSend* queuePeekHead(PacketQueue* queue);

// remove the head of the queue
void queueRemoveHead(PacketQueue* queue);

// move to the next queue element
void queueNext(PacketQueue* queue);

#endif // PACKETQUEUE_H_INCLUDED
