using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLANetary.Core.Types
{
    public class Query
    {
        public int QueryId = -1;

        /// <summary>
        /// Is the query periodic?
        /// </summary>
        public bool Periodic;

        /// <summary>
        /// Period of the query in milliseconds
        /// </summary>
        public int PeriodInMS;

        public readonly List<ValueSelection> Selections;

        public readonly List<ActuatorFunc> Actuators;

        public ConditionGroup Conditions { get; set; }        

        public string VirtualTableName { get; set; }

        public Query()
        {
            Selections = new List<ValueSelection>();
            Actuators = new List<ActuatorFunc>();
            Conditions = new ConditionGroup();           
        }

        public String GetTextRepresentation()
        {
            StringBuilder sb = new StringBuilder();

            if (Selections.Count > 0)
            {
                sb.Append("SENSE ");
                for (int i = 0; i < Selections.Count; i++)
                {
                    sb.Append(Selections[i].ToSqlString());

                    if (i < Selections.Count - 1)
                        sb.Append(", ");
                }
                sb.Append(Environment.NewLine);
            }

            if (Actuators.Count > 0)
            {
                sb.Append("ACT ");
                for (int i = 0; i < Actuators.Count; i++)
                {
                    sb.Append(Actuators[i].ToSqlString());

                    if (i < Actuators.Count - 1)
                        sb.Append(", ");
                }
                sb.Append(Environment.NewLine);
            }

            sb.Append("AT ").Append(VirtualTableName);

            if (Conditions.Count > 0)
            {                
                sb.Append(Environment.NewLine).Append("WHERE ");
                sb.Append(Conditions.ToString(false));
            }

            if (Selections.Any(s => s.SelFunction == SelectionFunction.GroupBy))
            {
                var groups = Selections.Where(s => s.SelFunction == SelectionFunction.GroupBy).ToList();

                sb.Append(Environment.NewLine).Append("GROUP BY ");
                for (int i = 0; i < groups.Count; i++)
                {
                    sb.Append(groups[i].ToSqlString());

                    if (i < groups.Count - 1)
                        sb.Append(", ");
                }
            }

            if (Periodic)
            {
                sb.Append(Environment.NewLine).Append("EVERY ").Append(PeriodInMS).Append(" milliseconds");
            }

            return sb.ToString();
        }
    }
}
