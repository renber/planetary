using PLANetaryQL.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace planetaryql.test
{
    /// <summary>
    /// Base class for parser test caless
    /// </summary>
    public abstract class ParserTestBase
    {

        protected PQLParser PQLParser { get; } = new PQLParser();

    }
}
