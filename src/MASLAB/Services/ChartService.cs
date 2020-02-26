using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using OxyPlot.Series;

namespace MASLAB.Services
{
    /// <summary>
    /// Serviço de plotagem de gráficos
    /// </summary>
    public class ChartService
    {
        /// <summary>
        /// Cria uma nova instância de ChartService
        /// </summary>
        private ChartService() { }

        private static ChartService Service = new ChartService();
        private static PlotModel model;

        public static PlotModel Model
        {
            get
            {
                if (model == null)
                {
                    model = new PlotModel();
                    Model.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Minimum = -2, Maximum = 150 });
                }
                return model;
            }
            private set
            {
                model = value;
            }
        }

        /// <summary>
        /// Obtém a instância do serviço
        /// </summary>
        /// <returns>Instância de ChartService</returns>
        public static ChartService GetService() => Service;

        /// <summary>
        /// Inclui um novo valor na plotagem de gráfico
        /// </summary>
        /// <param name="serieName"></param>
        /// <param name="time"></param>
        /// <param name="value"></param>
        public void Plot(string serieName, TimeSpan time, double value)
        {
            LineSeries m = (LineSeries)Model.Series.FirstOrDefault(x => x.Title == serieName);
            if(m == null)
            {
                m = new LineSeries() { Title = serieName };
                Model.Series.Add(m);
            }

            m.Points.Add(new DataPoint(time.TotalSeconds, value));

            Model.InvalidatePlot(true);
        }

        public void Clear()
        {
            Model.Series.Clear();
            Model.InvalidatePlot(true);
        }
    }

    public class ChartDataEventArgs : EventArgs
    {

    }
}
