using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualBasic.ApplicationServices;

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
                Close();
            }
            else
            {
                return;
            }
        }
    }
}