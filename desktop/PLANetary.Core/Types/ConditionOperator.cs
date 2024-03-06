using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLANetary.Core.Types
{
    /// <summary>
    /// Defines the operators which can be used in conditions
    /// </summary>
    public enum ConditionOperator
    {
        OP_EQUAL = 0,
        OP_GREATER = 1,
        OP_GREATER_OR_EQUAL = 2,
        OP_LESS = 3,
        OP_LESS_OR_EQUAL = 4,
        OP_NOT = 5
    }

    public static class ConditionOperatorExtensions
    {
        /// <summary>
        /// Get a human friendly version of the ConditionOperator value
        /// </summary>
        /// <param name="op"></param>
        /// <returns></returns>
        public static string ToFriendlyName(this ConditionOperator op)
        {
            switch (op)
            {
                case ConditionOperator.OP_EQUAL: return "=";
                case ConditionOperator.OP_GREATER: return ">";
                case ConditionOperator.OP_GREATER_OR_EQUAL: return ">=";
                case ConditionOperator.OP_LESS: return "<";
                case ConditionOperator.OP_LESS_OR_EQUAL: return "<=";
                case ConditionOperator.OP_NOT: return "<>";
                default:
                    return "Unknown";
            }
        }

        public static ConditionOperator FromMathSymbol(string symbol)
        {
            switch (symbol)
            {
                case "=": return ConditionOperator.OP_EQUAL;
                case ">": return ConditionOperator.OP_GREATER;
                case ">=": return ConditionOperator.OP_GREATER_OR_EQUAL;
                case "<": return ConditionOperator.OP_LESS;
                case "<=": return ConditionOperator.OP_LESS_OR_EQUAL;
                case "!=":
                case "<>":
                    return ConditionOperator.OP_NOT;
                default:
                    return (ConditionOperator)(-1);
            }
        }
    }
}

