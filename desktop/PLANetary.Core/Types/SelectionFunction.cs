using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLANetary.Core.Types
{
    public enum SelectionFunction
    {
        Single = 0,    // return all datasets
        Sum,        // return sum of values
        Max,       // return only maximum value
        Min,       // return minimum value
        Avg,       // return average value
        Count,     // return no of values
        GroupBy
    }

    public static class SelectionFunctionExtensions
    {
        public static string ToFriendlyName(this SelectionFunction selFunc)
        {
            switch (selFunc)
            {
                case SelectionFunction.Single: return "Single";
                case SelectionFunction.Sum: return "Sum";
                case SelectionFunction.Max: return "Max";
                case SelectionFunction.Min: return "Min";
                case SelectionFunction.Avg: return "Avg";
                case SelectionFunction.Count: return "Count";
                case SelectionFunction.GroupBy: return "Group by";
                default:
                    return "Unknown";
            }
        }

        public static string ToSqlFuncName(this SelectionFunction selFunc)
        {
            switch (selFunc)
            {                
                case SelectionFunction.Sum: return "SUM";
                case SelectionFunction.Max: return "MAX";
                case SelectionFunction.Min: return "MIN";
                case SelectionFunction.Avg: return "AVG";
                case SelectionFunction.Count: return "COUNT";                
                default:
                    return "";
            }
        }

        public static SelectionFunction FromSqlFuncName(string sqlName)
        {
            sqlName = sqlName.ToUpper();            

            switch (sqlName)
            {
                case "SUM": return SelectionFunction.Sum;
                case "MAX": return SelectionFunction.Max;
                case "MIN": return SelectionFunction.Min;
                case "AVG": return SelectionFunction.Avg;
                case "COUNT": return SelectionFunction.Count;
                default:
                    return (SelectionFunction)(-1);
            }
        }
    }   
}
