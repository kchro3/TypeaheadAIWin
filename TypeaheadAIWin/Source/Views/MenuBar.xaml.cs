using System.Windows.Controls;
using TypeaheadAIWin.Source.ViewModel;

namespace TypeaheadAIWin.Source.Views
{
    /// <summary>
    /// Interaction logic for MenuBar.xaml
    /// </summary>
    public partial class MenuBar : UserControl
    {
        public MenuBar()
        {
            InitializeComponent();
            this.DataContext = new MenuBarViewModel();
        }
    }
}
