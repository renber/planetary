using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLANetary.Core.Types
{
    public class SensorValue : IComparable<SensorValue>
    {
        public float Value;

        public SensorValue(float value)
        {
            Value = value;
        }

        public int CompareTo(SensorValue other)
        {
            return Value.CompareTo(other.Value);
        }
    }
}
