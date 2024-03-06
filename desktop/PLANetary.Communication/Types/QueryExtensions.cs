using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using proto = PLANetary.Communication.Protobuf;
using pl = PLANetary.Core.Types;
using static PLANetary.Communication.Protobuf.Action;
using PLANetary.Communication.Protobuf;

namespace PLANetary.Communication.Types
{
    static class QueryExtensions
    {

        public static bool IsPeriodicQuery(this proto.Query query)
        {
            return query.PeriodInSec > 0;
        }

        public static bool HasSelections(this proto.Query query)
        {
            return query.Actions.Any(x => x.ContentCase == (ContentOneofCase)proto.ActionType.Selector);
        }

        public static bool HasActors(this proto.Query query)
        {
            return query.Actions.Any(x => x.ContentCase == (ContentOneofCase)proto.ActionType.Actor);
        }

        public static bool HasEvents(this proto.Query query)
        {
            return query.Actions.Any(x => x.ContentCase == (ContentOneofCase)proto.ActionType.Event);
        }

        /// <summary>
        /// Converts a received resultset to the model representation
        /// </summary>        
        public static pl.Resultset ToModel(this proto.Resultset resultset)
        {
            var mResultset = new pl.Resultset();
            mResultset.NumberOfNodes = 0; // todo
            foreach(var row in resultset.Rows)
            {
                var mRow = new pl.ResultRow();
                mRow.NumberOfNodes = (int)row.NumberOfNodes;

                foreach(var val in row.Values)
                {
                    mRow.Values.Add(new pl.SensorValue(val));
                }

                mResultset.Rows.Add(mRow);
            }

            return mResultset;
        }

        /// <summary>
        /// Converts a query representation to the corresponding communication message representation (protobuf)
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static proto.Query ToMessage(this pl.Query query)
        {
            proto.Query msg = new proto.Query();

            msg.QueryId = new proto.QueryId() { ShortId = (uint)query.QueryId };

            msg.PeriodInSec = (uint)(query.Periodic ? (int)(query.PeriodInMS / 1000) : 0);
            msg.ConditionGroupLink = query.Conditions.ConditionLink == pl.BooleanLink.AND ? proto.ConditionLink.And : proto.ConditionLink.Or;

            // conditions
            if (query.Conditions.Count > 0)
            {
                foreach (var c in query.Conditions.Conditions)
                {
                    msg.ConditionGroups.Add(c.ToProtoConditionGroup());
                }
            }

            // actions (i.e. selections, act, raise)
            query.Selections.ForEach(s => msg.Actions.Add(s.ToProtoAction()));
            query.Actuators.ForEach(a => msg.Actions.Add(a.ToProtoAction()));

            return msg;
        }

        private static proto.ConditionGroup ToProtoConditionGroup(this pl.IQueryCondition queryCondition)
        {
            if (queryCondition is pl.SensorCondition)
            {
                var cg = new proto.ConditionGroup();
                cg.ConditionLink = proto.ConditionLink.And; // does not matter
                cg.Conditions.Add(((pl.SensorCondition)queryCondition).ToProtoCondition());
                return cg;
            }

            if (queryCondition is pl.ConditionGroup)
            {
                return ToProtoConditionGroup(((pl.ConditionGroup)queryCondition));
            }

            throw new ArgumentException("Unsupported queryCondition type " + queryCondition.GetType().ToString());
        }

        private static proto.ConditionGroup ToProtoConditionGroup(pl.ConditionGroup cGroup)
        {
            proto.ConditionGroup cg = new proto.ConditionGroup();

            if (cGroup.Conditions.Any(x => x is pl.ConditionGroup))
                throw new NotSupportedException("Multiple nested conitions are not supported");

            foreach (var c in cGroup.Conditions.OfType<pl.SensorCondition>())
            {
                cg.Conditions.Add(c.ToProtoCondition());
            }

            return cg;
        }

        private static proto.Condition ToProtoCondition(this pl.SensorCondition queryCondition)
        {
            var c = new proto.Condition();

            c.Identifier = new Identifier() { Name = queryCondition.Sensor.Name };
            c.Op = (proto.ValueOperator)queryCondition.Operator;
            c.Value = queryCondition.Value;

            return c;
        }

        private static proto.Action ToProtoAction(this pl.ValueSelection sel)
        {
            var a = new proto.Action();
            a.Selector = new proto.Selector() { Type = (SelectorType)sel.SelFunction, SensorId = new Identifier() { Name = sel.Sensor.Name} };
            return a;
        }

        private static proto.Action ToProtoAction(this pl.ActuatorFunc act)
        {
            var a = new proto.Action();
            
            a.Actor = new proto.Actor() { ActorId = new Identifier() { Name = act.Actuator.Name }, Param = (uint)act.Parameters.FirstOrDefault() };            
            return a;
        }

    }
}
