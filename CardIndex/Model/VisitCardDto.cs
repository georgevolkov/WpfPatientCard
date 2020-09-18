using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using CardIndexDal.Models;

namespace CardIndex.Model
{
    public class VisitCardDto : INotifyPropertyChanged, IDataErrorInfo
    {
        [Required] public int Id { get; set; }

        private DateTime _visitDate;

        [Required]
        public DateTime VisitDate
        {
            get { return _visitDate; }
            set
            {
                _visitDate = value;
                RaisePropertyChanged();
            }
        }

        private VisitType _visitType;
        [Required]
        public VisitType VisitType
        {
            get { return _visitType;}
            set
            {
                _visitType = value;
                RaisePropertyChanged();
            }
        }

        private string _diagnosis;
        [Required]
        public string Diagnosis
        {
            get { return _diagnosis; }
            set
            {
                _diagnosis = value;
                RaisePropertyChanged();
            }
        }

        public virtual PatientCardDto PatientCard { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged([CallerMemberName] string caller = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(caller));
        }

        public string Error { get; }

        public string this[string columnName]
        {
            get
            {
                string result = string.Empty;
                result = GetErrors(columnName, result);

                return result;
            }
        }

        private string GetErrors(string columnName, string result)
        {
            switch (columnName)
            {
                case nameof(Diagnosis):
                {
                    if (string.IsNullOrEmpty(Diagnosis))
                    {
                        result = "Диагноз. Не может быть пустым";
                    }

                    break;
                }
            }

            return result;
        }
    }
}
