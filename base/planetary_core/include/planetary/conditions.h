
#ifndef CONDTIONS_H
#define CONDTIONS_H

#include <planetary/querytypes.h>

#include <planetary/proto/identifier.pb.h>
#include <planetary/proto/query.pb.h>
#include <planetary/proto/condition.pb.h>

bool evaluateConditions(QueryCore* core, ConditionGroup* groups, uint8 noGroups, uint8 link);
bool nodeFulfillsStaticConditions(QueryCore* core, NodeId nodeId, ConditionGroup* groups, uint8 noGroups, uint8 link);

#endif
