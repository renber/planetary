#include <stdio.h>
#include <planetary/dummy_callbacks.h>
#include <planetary/proto/identifier.pb.h>

float getSensorValue_dummy(QueryCore* core, Identifier sensorId)
{
    return 0;
}

bool isStaticAttribute_dummy(QueryCore* core, Identifier sensorId)
{
    return false;
}

float getStaticAttributeValue_dummy(QueryCore* core, NodeId nodeId, Identifier sensorId)
{
    return 0;
}

void act_dummy(QueryCore* core, Identifier actorId, uint8 parameter)
{
    // nothing
    printf("dummy actor");
}
