using CommunityToolkit.Mvvm.ComponentModel;

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
