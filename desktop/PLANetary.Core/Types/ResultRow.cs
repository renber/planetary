using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetary.Core.Types
{
    /// <summary>
    /// Represents a single row of a resultset
    /// </summary>
    public class ResultRow
    {        
        public int NumberOfNodes { get; set; }

        public List<SensorValue> Values { get; } = new List<SensorValue>();
    }
}
