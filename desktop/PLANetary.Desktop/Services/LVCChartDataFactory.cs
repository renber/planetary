using PLANetary.Types.Statistics;
using PLANetary.ViewModels.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetary.Services
{
    /// <summary>
    /// IChartDataFactory implementation for the LiveCharts library
    /// </summary>
    public class LVCChartDataFactory : IChartDataFactory
    {
        public IChartData CreateChartData()
        {
            return new LVCChartData();
        }
    }
}
