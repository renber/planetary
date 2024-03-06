
#include <planetary/proto/condition.pb.h>

#include <planetary/conditions.h>
#include <planetary/querytypes.h>
#include <planetary/typedefs.h>

#include <planetary/proto/query.pb.h>


bool isConditionSatisfiedEx(Condition* c, float svalue)
{
  switch(c->op)
  {
    case ValueOperator_EQUAL: return svalue == c->value;
    case ValueOperator_GREATER: return svalue > c->value;
    case ValueOperator_GREATER_OR_EQUAL: return svalue >= c->value;
    case ValueOperator_LESS: return svalue < c->value;
    case ValueOperator_LESS_OR_EQUAL: return svalue <= c->value;
    case ValueOperator_NOT: return svalue != c->value;
    default:
     return false;
  }
}

// returns true if the condition is satisfied
bool isConditionSatisfied(QueryCore* core, Condition* c)
{
  return isConditionSatisfiedEx(c, core->getSensorValue(core, c->identifier));
}

// return true if the group contains only static attributes or is linked with AND and contains at least one static attribute
bool groupCanBeEvaluatedStatically(QueryCore* core, ConditionGroup* group)
{
  int i;
  for(i = 0; i < group->conditions_count; i++)
  {
     if (core->isStaticAttribute(core, group->conditions[i].identifier))
     {
       if (group->conditionLink == ConditionLink_AND)
        return 1;
     } else
     {
        if (group->conditionLink == ConditionLink_OR)
          return 0;
     }
  }

  return 1;
}

bool evaluateGroup(QueryCore* core, ConditionGroup* group)
{
   int i;
   unsigned char res;

   for(i = 0; i < group->conditions_count; i++)
   {
      res = isConditionSatisfied(core, &(group->conditions[i]));
      if (group->conditionLink == ConditionLink_AND && !res) // AND: the whole group is false if one term is false
        return false;
      else
        if (group->conditionLink == ConditionLink_OR && res) // OR: the whole group is true if one term is true
            return true;
   }

   return group->conditionLink == ConditionLink_AND;
}

bool evaluateConditions(QueryCore* core, ConditionGroup* groups, uint8 noGroups, uint8 link)
{
  int i;
  unsigned char res;

  if (noGroups == 0)
    return 1;

   for(i = 0; i < noGroups; i++)
   {
      res = evaluateGroup(core, &groups[i]);
      if (link == ConditionLink_AND && !res) // AND: the whole group is false if one term is false
        return false;
      else
        if (link == ConditionLink_OR && res) // OR: the whole group is true if one term is true
            return true;
   }

   return link == ConditionLink_AND;
}

bool evaluateGroupStatically(QueryCore* core, NodeId nodeId, ConditionGroup* group)
{
   int i;
   unsigned char res;
   float val;
   unsigned char allStatic = 1;

   for(i = 0; i < group->conditions_count; i++)
   {
      if (core->isStaticAttribute(core, group->conditions[i].identifier)) // can this condition be checked statically?
      {
        val = core->getStaticAttributeValue(core, nodeId, group->conditions[i].identifier);

        res = isConditionSatisfiedEx(&(group->conditions[i]), val);
        if (group->conditionLink == ConditionLink_AND && !res) // AND: the whole group is false if one term is false
          return false;
        else
          if (group->conditionLink == ConditionLink_OR && res) // OR: the whole group is true if one term is true
              return true;
       }
       else
        allStatic = 0;
   }

   if (allStatic)
    return group->conditionLink == ConditionLink_AND;
   else
    return 2;
}

bool nodeFulfillsStaticConditions(QueryCore* core, NodeId nodeId, ConditionGroup* groups, uint8 noGroups, uint8 link)
{
  int i;
  unsigned char res; // 0 = false, 1 = true; 2 = cannot be determined
  unsigned char allStatic = 1;

  if (noGroups == 0)
    return 1;

   for(i = 0; i < noGroups; i++)
   {
      res = evaluateGroupStatically(core, nodeId, &groups[i]);
      if (link == ConditionLink_AND && res == 0) // AND: the whole group is false if one term is false
        return false;
      else
        if (link == ConditionLink_OR && res == 1) // OR: the whole group is true if one term is true
            return true;
        else
         if (res == 2)
          allStatic = 0;
   }

   if (allStatic)
    return link == ConditionLink_AND;
   else
    return 1; // unsure
}
