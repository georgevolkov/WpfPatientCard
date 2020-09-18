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
    public class AddPatientCardViewModel : BaseViewModel
    {
        private readonly IUnityContainer _container;
        private readonly NotifyObservableCollection<PatientCardDto> _patientCards;
        private IMapper _mapper;
        private bool _isSaved;

        public PatientCardDto PatientCard { get; set; }

        private ICommand _closePatientCard;
        private ICommand _closeAndSaveCommand;
        private ICommand _closeWithoutSaveCommand;
        private CardIndexDbContext _context;
        public ICommand ClosePatientCardCommand => _closePatientCard ??= new CommandHandler(ClosePatientCard, () => CanExecute);

        public bool CanExecute => true;

        public ICommand CloseAndSaveCommand => _closeAndSaveCommand ??= new CommandHandler(SaveAndClose, () => CanExecute);

        public ICommand CloseWithoutSaveCommand => _closeWithoutSaveCommand ??= new CommandHandler(CloseWithoutSave, () => CanExecute);

        public AddPatientCardViewModel(IUnityContainer container, NotifyObservableCollection<PatientCardDto> patientCards, PatientCardDto patientCard)
        {
            _container = container;
            _mapper = container.Resolve<IMapper>();
            PatientCard = patientCard;
            _isSaved = false;
            _context = _container.Resolve<CardIndexDbContext>();

            if (patientCards != null && patientCards.Any())
            {
                PatientCard = new PatientCardDto { DateOfBirth = DateTime.Now };
            }
            _patientCards = patientCards;
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

        private void CloseWithoutSave(object obj)
        {
            _isSaved = true;
            SetSelectedItemBeginState();
            if (obj is Window window)
                window.Close();
        }

        private void ClosePatientCard(object obj)
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
                var record = _context.PatientCards.FirstOrDefault(x => x.Id == PatientCard.Id);

                if (record == null)
                {
                    record = _mapper.Map<PatientCard>(PatientCard);
                    _context.Add(record);
                    _context.SaveChanges();
                    _patientCards.Add(_mapper.Map<PatientCardDto>(record));
                }
                else
                {
                    record.Fio = PatientCard.Fio;
                    record.Address = PatientCard.Address;
                    record.DateOfBirth = PatientCard.DateOfBirth;
                    record.Gender = PatientCard.Gender;
                    record.Phone = PatientCard.Phone;
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

        private void SetSelectedItemBeginState()
        {
            var record = _context.PatientCards.FirstOrDefault(x => x.Id == PatientCard.Id);
            if (record == null) return;

            PatientCard.Fio = record.Fio;
            PatientCard.Gender = record.Gender;
            PatientCard.Address = record.Address;
            PatientCard.Phone = record.Phone;
            PatientCard.Gender = record.Gender;
            PatientCard.DateOfBirth = record.DateOfBirth;
        }
    }
}
