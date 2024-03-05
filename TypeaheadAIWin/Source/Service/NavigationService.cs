using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace TypeaheadAIWin.Source.Service
{
    public interface INavigationService
    {
        ObservableObject CurrentView { get; }
        void NavigateTo<T>() where T : ObservableObject;
    }

    public partial class NavigationService: ObservableObject, INavigationService
    {
        private Func<Type, ObservableObject> _viewModelFactory;

        [ObservableProperty]
        private ObservableObject currentView;

        public NavigationService(Func<Type, ObservableObject> viewModelFactory)
        {
            _viewModelFactory = viewModelFactory;
        }

        public void NavigateTo<TViewModel>() where TViewModel : ObservableObject
        {
            CurrentView = _viewModelFactory.Invoke(typeof(TViewModel));
        }
    }
}
