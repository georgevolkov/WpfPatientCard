using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace CardIndex.Converters
{
    [ValueConversion(typeof(Enum), typeof(IEnumerable<Enum>))]
    public class EnumToComboBoxConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var values = Enum.GetValues(value.GetType());
            return values;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
