using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PLANetary.Types.Statistics
{
    /// <summary>
    /// Interface for classes which realize the binding between StatSeries model objects and a specific UI Charting control
    /// </summary>
    public interface IChartData
    {
        /// <summary>
        /// Indicates wether chart data is available
        /// </summary>
        bool DataAvailable { get; }

        /// <summary>
        /// Remove all series which have been added so far
        /// </summary>
        void ClearSeries();

        /// <summary>
        /// Remove all axes which have been added so far
        /// </summary>
        void ClearAxes();

        /// <summary>
        /// Remove all series and axes
        /// </summary>
        void Clear();

        /// <summary>
        /// Add an Axis
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="title"></param>
        void AddAxis(AxisOrientation orientation, string title);

        /// <summary>
        /// Add an Axis
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="title"></param>
        /// <param name="labelFormatter"></param>
        void AddAxis(AxisOrientation orientation, string title, Func<double, string> labelFormatter);

        /// <summary>
        /// Add an Axis
        /// </summary>
        /// <param name="orientation"></param>
        /// <param name="title"></param>
        /// <param name="labels"></param>
        void AddAxis(AxisOrientation orientation, string title, IList<string> labels);

        /// <summary>
        /// Add the given Series
        /// </summary>
        /// <typeparam name="TSeries">Type of the Series descriptor</typeparam>
        /// <typeparam name="TXVal">Type of the x values</typeparam>
        /// <typeparam name="TYVal">Type of the Y values</typeparam>
        /// <param name="modelSeries">The StatSeries object to display in a chart</param>
        /// <param name="seriesTitleFunc">The function to retrieve the title of the Chart Series from the Series descriptor</param>        
        void AddSeries<TSeries, TXVal, TYVal>(SeriesType seriesType, IEnumerable<StatSeries<TSeries, TXVal, TYVal>> series, Func<TSeries, String> seriesTitleFunc);

        /// <summary>
        /// Add the given Series
        /// </summary>
        /// <typeparam name="TSeries">Type of the Series descriptor</typeparam>
        /// <typeparam name="TXVal">Type of the x values</typeparam>
        /// <typeparam name="TYVal">Type of the Y values</typeparam>
        /// <param name="modelSeries">The StatSeries object to display in a chart</param>
        /// <param name="seriesTitleFunc">The function to retrieve the title of the Chart Series from the Series descriptor</param>
        /// <param name="seriesColorFunc">The function to retrieve the color of the Chart Series from the Series descriptor</param>
        void AddSeries<TSeries, TXVal, TYVal>(SeriesType seriesType, IEnumerable<StatSeries<TSeries, TXVal, TYVal>> series, Func<TSeries, String> seriesTitleFunc, Func<TSeries, RGBColor> seriesColorFunc);

        /// <summary>
        /// Add the given Series
        /// </summary>
        /// <typeparam name="TSeries">Type of the Series descriptor</typeparam>
        /// <typeparam name="TXVal">Type of the x values</typeparam>
        /// <typeparam name="TYVal">Type of the Y values</typeparam>
        /// <param name="modelSeries">The StatSeries object to display in a chart</param>
        /// <param name="seriesTitleFunc">The function to retrieve the title of the Chart Series from the Series descriptor</param>
        /// <param name="seriesColorFunc">The function to retrieve the color of the Chart Series from the Series descriptor</param>
        /// <param name="labelPointFunc">Function to retrieve a caption for a label point</param>
        void AddSeries<TSeries, TXVal, TYVal>(SeriesType seriesType, IEnumerable<StatSeries<TSeries, TXVal, TYVal>> series, Func<TSeries, String> seriesTitleFunc, Func<TSeries, RGBColor> seriesColorFunc, Func<DataPoint, string> labelPointFunc);


    }

    /// <summary>
    /// Represents a data point in a chart
    /// </summary>
    public class DataPoint
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        /// <summary>
        /// Percental influence of this point compared to all other points
        /// </summary>
        public double Participation { get; private set; }

        public DataPoint(double x, double y, double participation)
        {
            X = x;
            Y = y;
            Participation = participation;
        }
    }

    public enum AxisOrientation
    {
        X,
        Y
    }

    public enum SeriesType
    {
        Line,
        Bar,
        StackedBar,
        Pie
    }
}
