using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using TypeaheadAIWin.Source.Views;

namespace TypeaheadAIWin.Source.ViewModel
{
    public partial class MenuBarViewModel: ObservableObject
    {
        private readonly ChatPageViewModel _chatWindowViewModel;

        public MenuBarViewModel(ChatPageViewModel chatWindowViewModel) {
            _chatWindowViewModel = chatWindowViewModel;
        }

        [RelayCommand]
        private void New()
        {
            _chatWindowViewModel.Cancel();
            _chatWindowViewModel.Clear();
        }

        [RelayCommand]
        private void Open()
        {
            // Implement the action for the Open command
            MessageBox.Show("Not implemented yet.");
        }

        [RelayCommand]
        private void Save()
        {
            // Implement the action for the Save command
            MessageBox.Show("Not implemented yet.");
        }

        [RelayCommand]
        private void Exit()
        {
            // Implement the action for the Exit command
            Application.Current.Shutdown();
        }

        [RelayCommand]
        private void Cursor()
        {
            var cursorSettingsWindow = new CursorSettingsWindow();
            cursorSettingsWindow.ShowDialog();
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
            MessageBox.Show("Not implemented yet.");
        }
    }
}
