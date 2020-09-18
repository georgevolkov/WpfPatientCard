using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutoMapper;
using CardIndex.Model;
using CardIndex.View;
using CardIndexDal;
using Unity;
using Unity.Resolution;
using Xceed.Wpf.AvalonDock.Layout;

namespace CardIndex.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IUnityContainer _container;
        private readonly IMapper _mapper;

        private ICommand _openFillCard;
        private ICommand _openFillVisit;

        private CardIndexDbContext _dbContext;

        public ICommand OpenFillCard
        {
            get { return _openFillCard ?? (_openFillCard = new CommandHandler(OpenFillCardView, () => CanExecute)); }
        }

        public ICommand OpenFillVisit
        {
            get { return _openFillVisit ?? (_openFillVisit = new CommandHandler(OpenFillVisitView, () => CanExecute)); }
        }

        [InjectionConstructor]

        public MainViewModel(IUnityContainer container, IMapper mapper)
        {
            _container = container;
            _mapper = mapper;
            _dbContext = container.Resolve<CardIndexDbContext>();
            PatientCards = GetPatientCards();

            if(PatientCardSelectedItem != null)
                PatientCardSelectedItem.PropertyChanged += SelectedValueOnPropertyChanged;
        }

        private void SelectedValueOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is PatientCardDto item)
                PatientCardSelectedItem = item;
            PatientCards = GetPatientCards();
        }

        public bool CanExecute
        {
            get { return true; }
        }

        private void OpenFillCardView(object param)
        {
            var control = param as LayoutDocumentPaneGroup;
            var pane = control.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            var view = _container.Resolve<FillPatientCardView>();
            OpenTab(pane, view, "Заполнение карточки пациента");
        }

        private void OpenFillVisitView(object param)
        {
            var control = param as LayoutDocumentPaneGroup;
            var pane = control.Descendents().OfType<LayoutDocumentPane>().FirstOrDefault();
            var view = _container.Resolve<FillVisitView>();
            OpenTab(pane, view, "Заполнение карты посещений");
        }

        private void OpenTab(LayoutDocumentPane pane, UserControl control, string title)
        {
            var tab = new LayoutDocument
            {
                Title = title,
                Content = control,
            };

            var existedTab = pane.Children.FirstOrDefault(x => x.Title == title);

            if (existedTab != null)
                existedTab.IsActive = true;
            else
            {
                pane.Children.Add(tab);
                tab.IsActive = true;
            }
        }

        private void ShowEmptySelectedRecordMessage()
        {
            MessageBox.Show("Не выбрана или отсутствует текущая запись.\nНеобходимо выбрать запись для редактирования.", "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        #region FillPatient

        public PatientCardDto PatientCardSelectedItem { get; set; }
        public NotifyObservableCollection<PatientCardDto> PatientCards { get; set; }

        private ICommand _addPatientCard;
        private ICommand _editPatientCard;
        private ICommand _deletePatientCard;

        public ICommand AddPatientCard
        {
            get
            {
                return _addPatientCard ??= new CommandHandler(OpenAddPatientCardView, () => CanExecute);
            }
        }

        public ICommand EditPatientCard
        {
            get
            {
                return _editPatientCard ??= new CommandHandler(OpenEditPatientCardView, () => CanExecute);
            }
        }

        public ICommand DeletePatientCard
        {
            get
            {
                return _deletePatientCard ??= new CommandHandler(DeletePatientCardRecord, () => CanExecute);
            }
        }

        private NotifyObservableCollection<PatientCardDto> GetPatientCards()
        {
            var patientCard = _mapper.Map<List<PatientCardDto>>(_dbContext.PatientCards.Include(x => x.Visits).ToList()) ?? new List<PatientCardDto>();
            if (PatientCardSelectedItem == null && patientCard.Any())
                PatientCardSelectedItem = patientCard.FirstOrDefault();

            if(PatientCardVisitSelectedItem == null && patientCard.Any())
                PatientCardVisitSelectedItem = patientCard.FirstOrDefault();

            return new NotifyObservableCollection<PatientCardDto>(patientCard);
        }

        private void OpenAddPatientCardView(object param)
        {
            var addPatientCardView = _container.Resolve<AddPatientCardView>(new ParameterOverride(typeof(NotifyObservableCollection<PatientCardDto>), PatientCards));
            addPatientCardView.ShowDialog();
            NotifyChanged(nameof(PatientCards));
        }

        private void OpenEditPatientCardView(object param)
        {
            if (PatientCardSelectedItem == null)
            {
                ShowEmptySelectedRecordMessage();
                return;
            }

            var addPatientCardView = _container.Resolve<AddPatientCardView>(
                new ParameterOverride(typeof(NotifyObservableCollection<PatientCardDto>), null),
                new ParameterOverride(typeof(PatientCardDto), PatientCardSelectedItem));
            addPatientCardView.ShowDialog();
        }

        private void DeletePatientCardRecord(object obj)
        {
            using (var context = _container.Resolve<CardIndexDbContext>())
            {
                if(PatientCardSelectedItem == null) return;
                var item = context.PatientCards.Include(x => x.Visits).FirstOrDefault(row => row.Id == PatientCardSelectedItem.Id);
                if (item == null) return;

                var dialogResult = MessageBox.Show($"Удалить выбранную запись с именем: {item.Fio}?", "Внимание!",
                    MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (dialogResult != MessageBoxResult.Yes) return;

                var visitCards = context.VisitCards.Where(x => x.PatientCard == item);
                context.VisitCards.RemoveRange(visitCards);
                context.PatientCards.Remove(item);
                context.SaveChanges();
                PatientCards.Remove(PatientCardSelectedItem);
                PatientCardSelectedItem = PatientCards.FirstOrDefault();
            }
        }

        #endregion FillPatient

        #region FillVisits

        private ICommand _addVisitCommand;
        private ICommand _editVisitCommand;
        private ICommand _deleteVisitCommand;

        public VisitCardDto VisitCardSelectedItem { get; set; }
        public NotifyObservableCollection<VisitCardDto> Visits { get; set; }

        public ICommand AddVisitCommand
        {
            get
            {
                return _addVisitCommand ??= new CommandHandler(OpenAddVisitCardView, () => CanExecute);
            }
        }

        public ICommand EditVisitCommand
        {
            get
            {
                return _editVisitCommand ??= new CommandHandler(OpenEditVisitCardView, () => CanExecute);
            }
        }

        public ICommand DeleteVisitCommand
        {
            get
            {
                return _deleteVisitCommand ??= new CommandHandler(DeleteVisitCard, () => CanExecute);
            }
        }

        private PatientCardDto _patientCardVisitSelectedItem;

        public PatientCardDto PatientCardVisitSelectedItem
        {
            get { return _patientCardVisitSelectedItem; }
            set
            {
                _patientCardVisitSelectedItem = value;
                Visits = GetVisits();
                NotifyChanged(nameof(Visits));
                NotifyChanged(nameof(HasRecords));
            }
        }

        public bool HasRecords => Visits != null && Visits.Any();

        private void OpenAddVisitCardView(object param)
        {
            var addPatientCardView = _container.Resolve<AddVisitView>(new ParameterOverride(typeof(NotifyObservableCollection<VisitCardDto>), Visits),
                new ParameterOverride(typeof(PatientCardDto), PatientCardVisitSelectedItem));
            addPatientCardView.ShowDialog();
            NotifyChanged(nameof(Visits));
        }

        private void OpenEditVisitCardView(object param)
        {
            if (VisitCardSelectedItem == null)
            {
                ShowEmptySelectedRecordMessage();
                return;
            }

            var view = _container.Resolve<AddVisitView>(
                new ParameterOverride(typeof(NotifyObservableCollection<VisitCardDto>), null),
                new ParameterOverride(typeof(VisitCardDto), VisitCardSelectedItem),
                new ParameterOverride(typeof(PatientCardDto), null));
            view.ShowDialog();
        }

        private void DeleteVisitCard(object obj)
        {
            if(VisitCardSelectedItem == null) return;
            var item = _dbContext.VisitCards.FirstOrDefault(row => row.Id == VisitCardSelectedItem.Id);
            if (item == null) return;

            var dialogResult = MessageBox.Show($"Удалить выбранную запись?", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (dialogResult != MessageBoxResult.Yes) return;

            _dbContext.VisitCards.Remove(item);
            _dbContext.SaveChanges();
            Visits.Remove(VisitCardSelectedItem);
            VisitCardSelectedItem = Visits.FirstOrDefault();
        }

        private NotifyObservableCollection<VisitCardDto> GetVisits()
        {
            if (PatientCardVisitSelectedItem == null)
                return new NotifyObservableCollection<VisitCardDto>();

            using (var context = _container.Resolve<CardIndexDbContext>())
            {
                var visits =
                    _mapper.Map<List<VisitCardDto>>(context.VisitCards
                        .Where(r => r.PatientCard.Id == PatientCardVisitSelectedItem.Id).ToList()) ??
                    new List<VisitCardDto>();

                if (visits.Any())
                    VisitCardSelectedItem = visits.FirstOrDefault();

                return new NotifyObservableCollection<VisitCardDto>(visits);
            }
        }

        #endregion FillVisits
    }
}
