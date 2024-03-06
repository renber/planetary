using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetary.Core.Types
{
    /// <summary>
    /// Represents a result set of a query
    /// </summary>
    public class Resultset
    {

        public int NumberOfNodes { get; set; }

        public List<ResultRow> Rows { get; } = new List<ResultRow>();

    }
}
