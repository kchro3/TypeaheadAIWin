using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using TypeaheadAIWin.Views;

namespace TypeaheadAIWin.Source.ViewModel
{
    public partial class MenuBarViewModel: ObservableObject
    {
        public MenuBarViewModel() { }

        [RelayCommand]
        private void New()
        {
            // Implement the action for the New command
            MessageBox.Show("New command executed.");
        }

        [RelayCommand]
        private void Open()
        {
            // Implement the action for the Open command
            MessageBox.Show("Open command executed.");
        }

        [RelayCommand]
        private void Save()
        {
            // Implement the action for the Save command
            MessageBox.Show("Save command executed.");
        }

        [RelayCommand]
        private void Exit()
        {
            // Implement the action for the Exit command
            Application.Current.Shutdown();
        }

        [RelayCommand]
        private void Speech()
        {
            var speechSettingsWindow = new SpeechSettingsWindow();
            speechSettingsWindow.ShowDialog();
        }

        [RelayCommand]
        private void About()
        {
            // Implement the action for the About command
            MessageBox.Show("About command executed.");
        }
    }
}
