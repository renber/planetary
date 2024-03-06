#ifndef DUMMY_CALLBACKS_H_INCLUDED
#define DUMMY_CALLBACKS_H_INCLUDED

// Provides dummy callback funtions for the QueryCore struct

#include <planetary/typedefs.h>
#include <planetary/querytypes.h>
#include <planetary/proto/identifier.pb.h>

float getSensorValue_dummy(QueryCore* core, Identifier sensorId);

bool isStaticAttribute_dummy(QueryCore* core, Identifier sensorId);

float getStaticAttributeValue_dummy(QueryCore* core, NodeId nodeId, Identifier sensorId);

void act_dummy(QueryCore* core, Identifier actorId, uint8 parameter);

#endif // DUMMY_CALLBACKS_H_INCLUDED
