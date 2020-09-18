using System.ComponentModel;
using System.Windows.Navigation;

namespace CardIndex.ViewModel
{
   public class BaseViewModel : INotifyPropertyChanged, INotifyPropertyChanging
   {

      public event PropertyChangedEventHandler PropertyChanged;
      public event PropertyChangingEventHandler PropertyChanging;

      protected void NotifyChanged(string propertyName)
      {
         var handler = PropertyChanged;
         handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }

      protected void NotifyChanging(string propertyName)
      {
         var handler = PropertyChanging;
         handler?.Invoke(this, new PropertyChangingEventArgs(propertyName));
      }

   }
}
