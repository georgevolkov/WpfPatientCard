using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using CardIndex.Model;
using CardIndex.ViewModel;
using Unity;
using Unity.Resolution;

namespace CardIndex.View
{
    /// <summary>
    /// Interaction logic for AddPatientCardView.xaml
    /// </summary>
    public partial class AddPatientCardView : Window
    {
        public AddPatientCardView(IUnityContainer container, NotifyObservableCollection<PatientCardDto> collection, PatientCardDto parameter)
        {
            DataContext = container.Resolve<AddPatientCardViewModel>(new ParameterOverride(typeof(NotifyObservableCollection<PatientCardDto>), collection),
                new ParameterOverride(typeof(PatientCardDto), parameter));
            InitializeComponent();
        }

        private void UIElement_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
    }
}
