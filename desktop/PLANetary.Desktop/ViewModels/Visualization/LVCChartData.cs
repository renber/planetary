using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using PLANetary.Types;
using PLANetary.Types.Statistics;

namespace PLANetary.ViewModels.Visualization
{
    /// <summary>
    /// A ViewModel which wraps a StatSeries Model and exhibits all properties needed
    /// by a LiveCharts View
    /// </summary>
    class LVCChartData : ViewModelBase, IChartData
    {
        public SeriesCollection Series { get; private set; }

        public AxesCollection AxesX { get; private set; }

        public AxesCollection AxesY { get; private set; }

        public bool DataAvailable
        {
            get
            {
                return Series.Count > 0;
            }
        }

        public LVCChartData()
        {
            Series = new SeriesCollection();
            AxesX = new AxesCollection();
            AxesY = new AxesCollection();
        }

        public void AddAxis(Types.Statistics.AxisOrientation orientation, string title)
        {
            AddAxis(orientation, title, null, null);
        }

        public void AddAxis(Types.Statistics.AxisOrientation orientation, string title, Func<double, string> labelFormatter)
        {
            AddAxis(orientation, title, labelFormatter, null);
        }

        public void AddAxis(Types.Statistics.AxisOrientation orientation, string title, IList<string> labels)
        {
            AddAxis(orientation, title, null, labels);
        }

        private void AddAxis(Types.Statistics.AxisOrientation orientation, string title, Func<double, string> labelFormatter, IList<string> labels)
        {
            AxesCollection axesColl = orientation == Types.Statistics.AxisOrientation.X ? AxesX : AxesY;
            Axis axis = new Axis();
            axis.Title = title;
            if (labelFormatter != null)
                axis.LabelFormatter = labelFormatter;
            if (labels != null)
                axis.Labels = labels;

            axesColl.Add(axis);
        }

        public void AddSeries<TSeries, TXVal, TYVal>(SeriesType seriesType, IEnumerable<StatSeries<TSeries, TXVal, TYVal>> series, Func<TSeries, String> seriesTitleFunc)
        {
            AddSeries(seriesType, series, seriesTitleFunc, null, null);
        }

        public void AddSeries<TSeries, TXVal, TYVal>(SeriesType seriesType, IEnumerable<StatSeries<TSeries, TXVal, TYVal>> series, Func<TSeries, String> seriesTitleFunc, Func<TSeries, RGBColor> seriesColorFunc)
        {
            AddSeries(seriesType, series, seriesTitleFunc, seriesColorFunc, null);
        }

        public void AddSeries<TSeries, TXVal, TYVal>(SeriesType seriesType, IEnumerable<StatSeries<TSeries, TXVal, TYVal>> sseries, Func<TSeries, String> seriesTitleFunc, Func<TSeries, RGBColor> seriesColorFunc, Func<DataPoint, string> labelPointFunc)
        {
            List<Series> createdSeries = new List<Series>();

            foreach (var singleSeries in sseries)
            {
                Series chartSeries;
                switch (seriesType)
                {
                    case SeriesType.Line:
                        // create a linear line series
                        chartSeries = new LineSeries() { LineSmoothness = 0 };
                        break;
                    case SeriesType.Bar:
                        chartSeries = new ColumnSeries();
                        break;
                    case SeriesType.StackedBar:
                        chartSeries = new StackedColumnSeries();
                        break;
                    case SeriesType.Pie:
                        chartSeries = new PieSeries() { DataLabels = true };
                        break;
                    default:
                        throw new ArgumentException("The given SeriesType '" + seriesType.ToString() + "' is not supported.");
                }

                chartSeries.Title = seriesTitleFunc(singleSeries.SeriesDescriptor);
                chartSeries.Values = new ChartValues<TXVal>(singleSeries.Values.Select(x => x.Value).Cast<TXVal>());

                // colors
                if (seriesColorFunc != null)
                {
                    RGBColor color = seriesColorFunc(singleSeries.SeriesDescriptor);

                    chartSeries.Stroke = new SolidColorBrush(color.ToWpfColor());
                    if (seriesType == SeriesType.Line)
                    {
                        // do not fill the area below the lines
                        chartSeries.Stroke = new SolidColorBrush(color.ToWpfColor());
                        chartSeries.Fill = Brushes.Transparent;
                    }
                    else
                        chartSeries.Fill = new SolidColorBrush(color.ToWpfColor());

                    if (labelPointFunc != null)
                    {
                        chartSeries.LabelPoint = (chartPoint) => labelPointFunc(new DataPoint(chartPoint.X, chartPoint.Y, chartPoint.Participation));
                    }
                }

                createdSeries.Add(chartSeries);
            }

            Series.AddRange(createdSeries);

            OnPropertyChanged("DataAvailable");
        }

        public void ClearSeries()
        {
            Series.Clear();
            OnPropertyChanged("DataAvailable");
        }

        public void ClearAxes()
        {
            AxesX.Clear();
            AxesY.Clear();
        }

        public void Clear()
        {
            ClearAxes();
            ClearSeries();
        }
    }
}
