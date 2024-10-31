using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Chess.Util.ViewUtil.Converters
{
    /// <summary>
    /// The converter class used in View. This class implements IValueConverter interface
    /// This class will decide if the highlight circle will be visible or not
    /// </summary>
    public class HighlightConverter : IValueConverter
    {
        /// <summary>
        /// Implementation Convert method in IValueConverter interface.
        /// </summary>
        /// <param name="value">The object value, which will actually decide the result. We expect it to be a bool value</param>
        /// <param name="targetType">The default targetType parameter</param>
        /// <param name="parameter">The default parameter object</param>
        /// <param name="culture">The default culture parameter</param>
        /// <returns>The value of enum Visibility, which is either Visible or Collapsed</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isVisible = (bool)value;
            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }


        /// <summary>
        /// Implementation ConvertBack method in IValueConverter interface. Not implemented yet
        /// </summary>
        /// <param name="value">Default parameter</param>
        /// <param name="targetType">Default parameter</param>
        /// <param name="parameter">Default parameter</param>
        /// <param name="culture">Default parameter</param>
        /// <returns>Not implemented yet</returns>
        /// <exception cref="NotImplementedException">Not implemented yet</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
