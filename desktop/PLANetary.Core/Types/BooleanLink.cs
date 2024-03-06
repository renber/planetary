using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLANetary.Core.Types
{
    public enum BooleanLink
    {
        AND = 1,
        OR = 2
    }

    public static class BooleanLinkExtension
    {
        public static string ToFriendlyName(this BooleanLink link)
        {
            switch(link)
            {
                case BooleanLink.AND: return "AND";
                case BooleanLink.OR: return "OR";
                default: return "?";
            }
        }
    }
}

