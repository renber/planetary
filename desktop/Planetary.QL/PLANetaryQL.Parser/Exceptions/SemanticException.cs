using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetaryQL.Parser.Exceptions
{
    public class SemanticException : Exception
    {

        public SemanticException(String message)
            : base(message)
        {

        }

    }
}
