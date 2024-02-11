using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using TypeaheadAIWin.Source.ViewModel;

namespace TypeaheadAIWin.Views
{
    /// <summary>
    /// Interaction logic for SpeechSettingsWindow.xaml
    /// </summary>
    public partial class SpeechSettingsWindow : Window
    {
        public SpeechSettingsWindow()
        {
            Owner = Application.Current.MainWindow;
            InitializeComponent();
            DataContext = App.ServiceProvider.GetRequiredService<SpeechSettingsViewModel>();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
