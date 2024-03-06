using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetary.Core.Types
{
    public class EventCondition : IQueryCondition
    {
        public Event Event { get; set; }
        public ConditionOperator Operator { get; set; }
        public int Value { get; set; }

        public EventCondition(Event evt, ConditionOperator op, int value)
        {
            Event = evt;
            Operator = op;
            Value = value;
        }

        public override string ToString()
        {
            return "evt'" + Event.Name + " " + Operator.ToFriendlyName() + " " + Value.ToString();
        }
    }
}
