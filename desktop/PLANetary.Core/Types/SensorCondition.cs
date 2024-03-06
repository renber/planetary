using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLANetary.Core.Types
{
    public class SensorCondition : IQueryCondition
    {
        public Sensor Sensor;
        public ConditionOperator Operator;
        public int Value;

        public SensorCondition(Sensor sensor, ConditionOperator op, int value)
        {
            Sensor = sensor;
            Operator = op;
            Value = value;
        }

        public override string ToString()
        {
            return Sensor.Name + " " + Operator.ToFriendlyName() + " " + Value.ToString();
        }
    }
}
