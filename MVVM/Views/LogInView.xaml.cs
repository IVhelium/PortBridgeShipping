using System.Windows.Controls;
using System.Windows.Input;
using PortBridgeShipping.MVVM.ViewModels;

namespace PortBridgeShipping.MVVM.Views
{
    /// <summary>
    /// Логика взаимодействия для LogInView.xaml
    /// </summary>
    public partial class LogInView : UserControl
    {
        public LogInView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is LogInViewModel vm && sender is PasswordBox pb)
            {
                vm.Password = pb.Password;
                CommandManager.InvalidateRequerySuggested(); // ensure Login/Register buttons update
            }
        }
    }
}
