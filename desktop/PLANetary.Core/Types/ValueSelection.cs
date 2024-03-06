using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLANetary.Core.Types
{
    public class ValueSelection
    {
        public Sensor Sensor;

        public string Alias = "";
        public SelectionFunction SelFunction;
 
        public ValueSelection(Sensor sensor, SelectionFunction selFunction)
        {
            Sensor = sensor;
            SelFunction = selFunction;
        }

        public override string ToString()
        {
            if (SelFunction == SelectionFunction.Single)
                return Sensor.Name;
            else
                return SelFunction.ToFriendlyName() + "(" + Sensor.Name + ")";
        }

        public string ToSqlString()
        {
            if (SelFunction == SelectionFunction.Single || SelFunction == SelectionFunction.GroupBy)
                return Sensor.Name;
            else
                return SelFunction.ToSqlFuncName() + "(" + Sensor.Name + ")";
        }

        public override bool Equals(object obj)
        {
            if (obj is ValueSelection)
            {
                return (obj as ValueSelection).Sensor == Sensor && (obj as ValueSelection).SelFunction == SelFunction;
            }
            else
                return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            int mc = 397;
            return mc * Sensor.GetHashCode() * SelFunction.GetHashCode();
        }
    }
}
