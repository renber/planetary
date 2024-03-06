using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetary.Types.Statistics
{
    /// <summary>
    /// Klasse, die Daten für die stat. Auswertung vorhält
    /// </summary>
    /// <typeparam name="TSeries"></typeparam>
    /// <typeparam name="TXVal"></typeparam>
    public class StatSeries<TSeries, TXVal, TYVal>
    {
        public TSeries SeriesDescriptor { get; private set; }

        public List<KeyValuePair<TXVal, TYVal>> Values { get; private set; }

        public StatSeries(TSeries descriptor)
        {
            SeriesDescriptor = descriptor;
            Values = new List<KeyValuePair<TXVal, TYVal>>();
        }
    }
}
