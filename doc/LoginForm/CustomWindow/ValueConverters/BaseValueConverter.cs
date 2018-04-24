
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace CustomWindow.ValueConverters
{
    public abstract class BaseValueConverter<T> : MarkupExtension, IValueConverter
        where T : class , new ()
    {
        #region Private Members

        private static T _mConverter;

        #endregion

        #region Markup extension method

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _mConverter ?? (_mConverter = new T());
        }

        #endregion


        #region Value Converter Method

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

        #endregion

    }
}
