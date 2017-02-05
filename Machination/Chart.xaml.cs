using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using SciChart.Charting.Model.DataSeries;
using SciChart.Charting.Visuals.RenderableSeries;
using SciChart.Data.Model;

namespace Machination
{
    /// <summary>
    /// Логика взаимодействия для Chart.xaml
    /// </summary>
    public partial class Chart : UserControl
    {

        public Chart()
        {
            InitializeComponent();
        }

        private void Chart_OnLoaded(object sender, RoutedEventArgs e)
        {

            var dataSeries = MainWindow.draw();

            var idealData = drawIdeal();
           
            idealSeries.DataSeries = idealData;
            lineRenderSeries.DataSeries = dataSeries;

            sciChart.ZoomExtents();
        }


        public XyDataSeries<double, double> drawIdeal()
        {
            var idealData = new XyDataSeries<double, double>();

            idealData.Append(0, 0);
            for (int i = 1; i <= 10; i++)
            {
                idealData.Append(i, i);
            }

            return idealData;
        }
    }
}
