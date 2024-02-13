using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TypeaheadAIWin.Source.ViewModel;

namespace TypeaheadAIWin.Source.Views
{
    /// <summary>
    /// Interaction logic for CursorSettingsWindow.xaml
    /// </summary>
    public partial class CursorSettingsWindow : Window
    {
        public CursorSettingsWindow()
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<CursorSettingsViewModel>();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
