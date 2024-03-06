using PLANetary.Types.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetary.Services
{
    /// <summary>
    /// Factory interface to create ChartData objects
    /// </summary>
    public interface IChartDataFactory
    {

        IChartData CreateChartData();

    }
}
