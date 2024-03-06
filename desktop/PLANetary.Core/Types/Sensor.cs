using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLANetary.Core.Types
{
    public class Sensor
    {
        public String Name;
        public String FriendlyName;           
  
        public Sensor(String variableName, String friendlyName)
        {
            Name = variableName;
            FriendlyName = friendlyName;            
        }
    }
}
