using System.Windows.Controls;

namespace PortBridgeShipping.MVVM.Views
{
    /// <summary>
    /// Логика взаимодействия для RoutesView.xaml
    /// </summary>
    public partial class RoutesView : UserControl
    {
        public RoutesView()
        {
            InitializeComponent();
        }

        private void ComboBox_ViewMode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ComboBox_ViewMode.SelectedIndex)
            {
                case 0:
                    StackPanel_Route.IsEnabled = true;
                    StackPanel_Segment.IsEnabled = false;
                    StackPanel_Route.Visibility = System.Windows.Visibility.Visible;
                    StackPanel_Segment.Visibility = System.Windows.Visibility.Hidden;
                    TextBox_Name.IsEnabled = true;
                    TextBox_From.IsEnabled = false;
                    TextBox_To.IsEnabled = false;
                    break;

                case 1:
                    StackPanel_Route.IsEnabled = false;
                    StackPanel_Segment.IsEnabled = true;
                    StackPanel_Route.Visibility = System.Windows.Visibility.Hidden;
                    StackPanel_Segment.Visibility = System.Windows.Visibility.Visible;
                    TextBox_Name.IsEnabled = false;
                    TextBox_From.IsEnabled = true;
                    TextBox_To.IsEnabled = true;
                    break;
            }
        }
    }
}
