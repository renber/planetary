using PLANetary.Core.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetary.Types
{
    class QueryCreatedEventArgs : EventArgs
    {

        public bool Handled { get; set; }

        public Query Query { get; }

        public QueryCreatedEventArgs(Query query)
        {
            Query = query;
        }

    }
}
