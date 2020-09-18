using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AutoMapper;
using CardIndex.Model;
using CardIndexDal;
using CardIndexDal.Models;
using Unity;

namespace CardIndex.ViewModel
{
    public class AddVisitViewModel : BaseViewModel
    {
        private readonly IUnityContainer _container;
        private ICommand _closeVisitCommand;
        private ICommand _closeWithoutSaveCommand;
        private ICommand _closeAndSaveCommand;

        private readonly NotifyObservableCollection<VisitCardDto> _visitCards;
        private readonly CardIndexDbContext _context;
        private bool _isSaved;
        private readonly IMapper _mapper;
        private PatientCardDto _patientCard;


        public AddVisitViewModel(IUnityContainer container,
            NotifyObservableCollection<VisitCardDto> visitCards,
            VisitCardDto visitCard,
            PatientCardDto patientCard)
        {
            _container = container;
            _context = _container.Resolve<CardIndexDbContext>();
            _mapper = container.Resolve<IMapper>();
            Visit = visitCard;

            if (visitCards != null)
            {
                Visit = new VisitCardDto {VisitDate = DateTime.Now};
                _visitCards = visitCards;
                _patientCard = patientCard;
            }
        }

        public bool CanExecute => true;

        public ICommand CloseVisitCommand => _closeVisitCommand ??= new CommandHandler(CloseVisit, () => CanExecute);
        public ICommand CloseWithoutSaveCommand => _closeWithoutSaveCommand ??= new CommandHandler(CloseWithoutSave, () => CanExecute);
        public ICommand CloseAndSaveCommand => _closeAndSaveCommand ??= new CommandHandler(SaveAndClose, () => CanExecute);

        public VisitCardDto Visit { get; set; }

        private void CloseVisit(object obj)
        {
            if (_isSaved) return;

            var dialogResult = MessageBox.Show("Сохранить изменения карточки пациента?", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (dialogResult == MessageBoxResult.Yes)
                SaveRecord();
            else
                SetSelectedItemBeginState();
        }

        private void SaveRecord(object param = null)
        {
            try
            {
                if (Visit == null)
                    return;

                if (_patientCard == null)
                {
                    MessageBox.Show("Невозможно сохранить запись, т.к. не выбран пациент", "Предупреждение!",
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                    _isSaved = true;
                        return;
                }

                var record = _context.VisitCards.FirstOrDefault(x => x.Id == Visit.Id);

                if (record == null)
                {
                    Visit.PatientCard = _patientCard;
                    record = _mapper.Map<VisitCard>(Visit);
                    var patientCard = _context.PatientCards.Find(_patientCard.Id);
                    record.PatientCard = patientCard;
                    _context.Add(record);
                    patientCard.Visits.Add(record);
                    _context.SaveChanges();
                    _visitCards.Add(_mapper.Map<VisitCardDto>(record));
                }
                else
                {
                    record.VisitType = Visit.VisitType;
                    record.VisitDate = Visit.VisitDate;
                    record.Diagnosis = Visit.Diagnosis;

                    _context.Update(record);
                    _context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"Произошла ошибка при сохранении {e.Message}\n{e.InnerException?.Message}", "Ошибка!", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            _isSaved = true;
        }

        private void CloseWithoutSave(object obj)
        {
            _isSaved = true;
            SetSelectedItemBeginState();
            if (obj is Window window)
                window.Close();
        }

        private void SaveAndClose(object obj)
        {
            var isValid = IsValid(obj as DependencyObject);
            if (!isValid)
            {
                MessageBox.Show("Заполнены не все обязательные поля", "Внимание!", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }
            SaveRecord();
            if (obj is Window window)
                window.Close();
        }

        private bool IsValid(DependencyObject obj)
        {
            return !(Validation.GetHasError(obj) && !(obj is Xceed.Wpf.Toolkit.DateTimePicker)) &&
                   LogicalTreeHelper.GetChildren(obj).OfType<DependencyObject>().All(IsValid);
        }

        private void SetSelectedItemBeginState()
        {
            if (Visit == null) return;

            var record = _context.VisitCards.FirstOrDefault(x => x.Id == Visit.Id);
            if (record == null) return;

            Visit.VisitType = record.VisitType;
            Visit.VisitDate = record.VisitDate;
            Visit.Diagnosis = record.Diagnosis;
        }
    }
}
