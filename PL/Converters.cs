using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL
{
    #region Invert Boolean
    /// <summary>
    /// Invert a boolean value.
    /// </summary>
    public class BoolInverter : IValueConverter
    {
        /// <summary>
        /// Convert a boolean value to its inverse.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>The inverse boolean.</returns>
        /// <exception cref="InvalidOperationException">The target type must be a boolean.</exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
            {
                throw new InvalidOperationException("The target must be a boolean");
            }
            return !(bool)value;
        }

        /// <summary>
        /// Undo boolean inversion, which is the same as performing an inversion.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>The inverse boolean.</returns>
        /// <exception cref="InvalidOperationException">The target type must be a boolean.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Convert(value, targetType, parameter, culture);
        }
    }
    #endregion

    #region Format Double (for battery)
    /// <summary>
    /// Perform a floor function on a value, and append '%'.
    /// </summary>
    public class FormatBattery : IValueConverter
    {
        /// <summary>
        /// Convert the double to a string consisting of the floor of the double and an appended '%'.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>The new formatting.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Math.Floor((double)value) + "%";
        }

        /// <summary>
        /// Reversing this conversion is impossible as we cannot determine a double from its floor.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException">This conversion is irreversible.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    #endregion

    #region Convert Boolean to Visibility
    /// <summary>
    /// Convert from a boolean value to Visibility.
    /// </summary>
    public class BoolToVisibility : IValueConverter
    {
        /// <summary>
        /// Convert "true" to Visible, and "false" to Collapsed.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>The corresponding Visibility value.</returns>
        /// <exception cref="InvalidOperationException">The target type must be of type Visibility.</exception>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
            {
                throw new InvalidOperationException("The target must be of type Visibilty");
            }

            if ((bool)value)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        /// <summary>
        /// Convert Visible to "true", and Hidden and Collapsed to "false".
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns>The corresponding boolean value.</returns>
        /// <exception cref="InvalidOperationException">The target type must be a boolean.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool))
            {
                throw new InvalidOperationException("The target must be a boolean");
            }

            if ((Visibility)value == Visibility.Visible)
            {
                return true;
            }
            return false;
        }
    }
    #endregion
}
