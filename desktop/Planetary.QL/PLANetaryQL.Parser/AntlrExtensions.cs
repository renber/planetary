using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetaryQL.Parser
{
    public static class AntlrExtensions
    {

        public static bool IsPresent(this ParserRuleContext context)
        {
            return context != null && context.ChildCount > 0;
        }

        public static bool IsPresent(this ITerminalNode terminalNode)
        {
            return terminalNode != null && !String.IsNullOrEmpty(terminalNode.GetText());
        }
    }
}
