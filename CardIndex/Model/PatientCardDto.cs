using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using CardIndexDal.Models;

namespace CardIndex.Model
{
    public class PatientCardDto : INotifyPropertyChanged, IDataErrorInfo
    {
        [Required] public int Id { get; set; }

        public string _fio;

        [Required]
        public string Fio
        {
            get { return _fio; }
            set
            {
                _fio = value;
                RaisePropertyChanged();
            }
        }

        public Gender _gender;

        [Required]
        public Gender Gender
        {
            get { return _gender; }
            set
            {
                _gender = value;
                RaisePropertyChanged();
            }
        }

        public DateTime _dateOfBirth;

        [Required]
        public DateTime DateOfBirth
        {
            get { return _dateOfBirth; }
            set
            {
                _dateOfBirth = value;
                RaisePropertyChanged();
            }
        }

        public string _address;

        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                RaisePropertyChanged();
            }
        }

        public string _phone;

        public string Phone
        {
            get { return _phone; }
            set
            {
                _phone = value;
                RaisePropertyChanged();
            }
        }

        public virtual List<VisitCardDto> Visits { get; set; }
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
                case nameof(Fio):
                {
                    if (string.IsNullOrEmpty(Fio))
                    {
                        result = "Ф.И.о. Не может быть пустым";
                    }

                    break;
                }
                case nameof(DateOfBirth):
                    break;
            }

            return result;
        }
    }
}
