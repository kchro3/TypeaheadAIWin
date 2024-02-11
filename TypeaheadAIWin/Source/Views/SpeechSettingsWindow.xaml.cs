using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TypeaheadAIWin.Source.Speech;
using TypeaheadAIWin.Source.ViewModel;

namespace TypeaheadAIWin.Views
{
    /// <summary>
    /// Interaction logic for SpeechSettingsWindow.xaml
    /// </summary>
    public partial class SpeechSettingsWindow : Window
    {
        public SpeechSettingsWindow(Window parent, SpeechSettingsViewModel viewModel)
        {
            Owner = parent;
            InitializeComponent();
            this.DataContext = viewModel;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
