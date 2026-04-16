using System.Windows;
using System.Windows.Input;

namespace PortBridgeShipping
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void lblClose_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show("Are you sure want to exit", "MESSAGE", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
            else
            {
                return;
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();    // Возможность передвигать окно
            }
        }
    }
}