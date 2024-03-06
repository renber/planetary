using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLANetary.Core.Types
{
    public class ConditionGroup : IQueryCondition
    {
        public BooleanLink ConditionLink { get; set; }

        public List<IQueryCondition> Conditions { get; private set; }

        public ConditionGroup()
        {
            Conditions = new List<IQueryCondition>();
        }

        public int Count
        {
            get
            {
                return Conditions.Count;
            }
        }

        public IQueryCondition this[int i]
        {
            get
            {
                return Conditions[i];
            }
        }

        public override string ToString()
        {
            return ToString(true);
        }

        public string ToString(bool includeBraces)
        {
            StringBuilder sb = new StringBuilder();

            if (includeBraces)
                sb.Append("(");

            for (int i = 0; i < Conditions.Count; i++)
            {
                sb.Append(Conditions[i].ToString());

                if (i < Conditions.Count - 1)
                {
                    sb.Append(" ").Append(ConditionLink.ToFriendlyName()).Append(" ");
                }
            }

            if (includeBraces)
                sb.Append(")");

            return sb.ToString();
        }
    }
}
