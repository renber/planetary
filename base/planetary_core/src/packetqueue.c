
#include <planetary/proto/planetarymsg.pb.h>
#include <planetary/querytypes.h>
#include <planetary/packetqueue.h>

void queueInit(PacketQueue* queue)
{
    queue->q_Head = 0;
    queue->q_Tail = 0;
}

bool queueIsEmpty(PacketQueue* queue)
{
    return queue->q_Tail == queue->q_Head;
}

PacketToSend* queuePut(PacketQueue* queue)
{
   unsigned char t = queue->q_Tail;
   queue->q_Tail++;
   if (queue->q_Tail == MAX_QUEUE_ELEMENTS)
     queue->q_Tail = 0;

   // init/reset element
   queue->elements[t].frameID = 0;
   queue->elements[t].curPos = 0;
   queue->elements[t].noOfReceivers = 0;
   queue->elements[t].argument.shortId = 0;
   queue->elements[t].what = PlanetaryMessage_broadcast_tag;

   return &(queue->elements[t]);
}

PacketToSend* queuePeekHead(PacketQueue* queue)
{
    return &queue->elements[queue->q_Head];
}

void queueRemoveHead(PacketQueue* queue)
{
    queue->q_Head++;
    if (queue->q_Head == MAX_QUEUE_ELEMENTS)
      queue->q_Head = 0;

    //xSemaphoreTake(pendingPacketsEvent, 0);
}

void queueNext(PacketQueue* queue)
{
   if (queueIsEmpty(queue)) return;

    PacketToSend* p = queuePeekHead(queue);
    p->curPos = p->curPos + 1;
    if (p->noOfReceivers == SEND_BROADCAST || p->curPos >= p->noOfReceivers)
    {
      if(p->what == PlanetaryMessage_resultset_tag)
      {
          if (p->querySlot->state == STATE_DORMANT && p->querySlot->periodic)
            awakeQuery(p->querySlot);
          else
            p->querySlot->state = STATE_UNUSED; // deactivate query as results have been sent
      }

      queueRemoveHead(queue);
    }
}
