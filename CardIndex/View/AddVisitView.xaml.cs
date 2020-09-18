using System.Windows;
using CardIndex.Model;
using CardIndex.ViewModel;
using Unity;
using Unity.Resolution;

namespace CardIndex.View
{
    /// <summary>
    /// Interaction logic for AddVisitView.xaml
    /// </summary>
    public partial class AddVisitView : Window
    {
        public AddVisitView(IUnityContainer container, NotifyObservableCollection<VisitCardDto> collection, VisitCardDto parameter, PatientCardDto patientCard)
        {

            DataContext = container.Resolve<AddVisitViewModel>(new ParameterOverride(typeof(NotifyObservableCollection<VisitCardDto>), collection),
                new ParameterOverride(typeof(VisitCardDto), parameter),
                new ParameterOverride(typeof(PatientCardDto), patientCard));
            InitializeComponent();
        }
    }
}
