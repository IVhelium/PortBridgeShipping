using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using PortBridgeShipping.MVVM.ViewModels;

namespace PortBridgeShipping.MVVM.Views
{
    public partial class StatisticsView : UserControl
    {
        public StatisticsView()
        {
            InitializeComponent();

            this.Loaded += StatisticsView_Loaded;
        }

        private void StatisticsView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is StatisticsViewModel vm)
            {
                vm.LoadStatistics();

                var values = vm.Values.Select(v => (double)v).ToArray();
                var labels = vm.Labels;

                var series = new ColumnSeries
                {
                    Title = "Count",
                    Values = new ChartValues<double>(values)
                };

                chart.Series = new SeriesCollection { series };
                axisX.Labels = labels;
                axisY.LabelFormatter = vm.YFormatter;
            }
        }
    }
}
