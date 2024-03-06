using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLANetary.Core.Types
{
    public class Actuator
    {        
        public string Name;
        public string FriendlyName;

        public Actuator(String variableName, String friendlyName)
        {
            Name = variableName;
            FriendlyName = friendlyName;            
        }
    }
}
