using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using H.Hooks;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TypeaheadAIWin.Source.Model;
using TypeaheadAIWin.Source.Service;

namespace TypeaheadAIWin.Source.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly ChatPageViewModel _chatPageViewModel;
        private readonly Supabase.Client _supabaseClient;
        private readonly UserDefaults _userDefaults;
        private LowLevelKeyboardHook _lowLevelKeyboardHook;

        [ObservableProperty]
        private bool _activated;

        [ObservableProperty]
        private INavigationService _navigationService;

        public MainWindowViewModel(
            ChatPageViewModel chatPageViewModel,
            Supabase.Client supabaseClient,
            INavigationService navigationService,
            UserDefaults userDefaults)
        {
            _chatPageViewModel = chatPageViewModel;
            _supabaseClient = supabaseClient;
            _navigationService = navigationService;
            _userDefaults = userDefaults;

            _lowLevelKeyboardHook = new LowLevelKeyboardHook();
            _lowLevelKeyboardHook.HandleModifierKeys = true;
            _lowLevelKeyboardHook.Handling = true;
            _lowLevelKeyboardHook.IsCapsLock = true;
            _lowLevelKeyboardHook.Down += LowLevelKeyboardHook_Down;
            _lowLevelKeyboardHook.Up += LowLevelKeyboardHook_Up;
            _lowLevelKeyboardHook.Start();

            if (_supabaseClient.Auth.CurrentSession != null)
            {
                NavigateToChatPage();
            }
            else
            {
                NavigateToLoginPage();
            }
        }

        [RelayCommand]
        private void NavigateToLoginPage()
        {
            NavigationService.NavigateTo<LoginPageViewModel>();
        }

        [RelayCommand]
        private void NavigateToChatPage()
        {
            NavigationService.NavigateTo<ChatPageViewModel>();
        }

        private void LowLevelKeyboardHook_Down(object? sender, KeyboardEventArgs e)
        {
            // Activate Window hotkey
            if ((e.Keys.Are(Key.Shift, Key.LeftWindows, Key.Space) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftLeftWindow) ||
                (e.Keys.Are(Key.Shift, Key.Insert, Key.Space) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftInsert) ||
                (e.Keys.Are(Key.Shift, Key.CapsLock, Key.Space) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftCapsLock))
            {
                e.IsHandled = true;
                Activated = !Activated;
            }
            // New Window hotkey
            else if ((e.Keys.Are(Key.Shift, Key.LeftWindows, Key.N) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftLeftWindow) ||
                     (e.Keys.Are(Key.Shift, Key.Insert, Key.N) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftInsert) ||
                     (e.Keys.Are(Key.Shift, Key.CapsLock, Key.N) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftCapsLock))
            {
                e.IsHandled = true;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _chatPageViewModel.Clear();
                    Activated = true;
                });
            }
            // Screenshot Window hotkey
            else if ((e.Keys.Are(Key.Shift, Key.LeftWindows, Key.I) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftLeftWindow) ||
                     (e.Keys.Are(Key.Shift, Key.Insert, Key.I) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftInsert) ||
                     (e.Keys.Are(Key.Shift, Key.CapsLock, Key.I) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftCapsLock))
            {
                e.IsHandled = true;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _chatPageViewModel.Clear();
                    _chatPageViewModel.TakeScreenshot();
                    Activated = true;
                });
            }
            // Cancel speaking
            else if (e.Keys.Are(Key.LeftWindows, Key.Escape))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _chatPageViewModel.Cancel();
                });
            }
        }

        private void LowLevelKeyboardHook_Up(object? sender, KeyboardEventArgs e)
        {
            // Activate Window hotkey
            if ((e.Keys.Are(Key.Shift, Key.LeftWindows, Key.Space) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftLeftWindow) ||
                (e.Keys.Are(Key.Shift, Key.Insert, Key.Space) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftInsert) ||
                (e.Keys.Are(Key.Shift, Key.CapsLock, Key.Space) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftCapsLock))
            {
                e.IsHandled = true;
            }
            // New Window hotkey
            else if ((e.Keys.Are(Key.Shift, Key.LeftWindows, Key.N) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftLeftWindow) ||
                     (e.Keys.Are(Key.Shift, Key.Insert, Key.N) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftInsert) ||
                     (e.Keys.Are(Key.Shift, Key.CapsLock, Key.N) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftCapsLock))
            {
                e.IsHandled = true;
            }
            // Screenshot Window hotkey
            else if ((e.Keys.Are(Key.Shift, Key.LeftWindows, Key.I) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftLeftWindow) ||
                     (e.Keys.Are(Key.Shift, Key.Insert, Key.I) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftInsert) ||
                     (e.Keys.Are(Key.Shift, Key.CapsLock, Key.I) && _userDefaults.TypeaheadKey == TypeaheadKey.ShiftCapsLock))
            {
                e.IsHandled = true;
            }
        }
    }
}
