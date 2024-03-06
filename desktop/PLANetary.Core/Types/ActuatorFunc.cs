using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLANetary.Core.Types
{
    public class ActuatorFunc
    {
        public Actuator Actuator { get; set; }
        public List<int> Parameters { get; private set; }

        public ActuatorFunc(Actuator actuator, params int[] parameters)
        {
            Actuator = actuator;
            Parameters = new List<int>(parameters);
        }

        public string ToSqlString()
        {
            if (Parameters.Count > 0)
                return Actuator.Name + "(" + String.Join(", ", Parameters.Select(x => x.ToString())) + ")";
            else
                return Actuator.Name;
        }
    }
}
