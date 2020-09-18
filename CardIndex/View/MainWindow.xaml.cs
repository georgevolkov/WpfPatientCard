using System.Windows;
using CardIndex.ViewModel;
using Unity;

namespace CardIndex
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(IUnityContainer container)
        {
            var mainViewModel = container.Resolve<MainViewModel>();
            DataContext = mainViewModel;
            InitializeComponent();
        }
    }
}
