using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.ComponentModel;

namespace TFG___PixelFarm
{
    public class TipoAColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Brushes.Transparent;

            if (value is Brush brush) return brush;
            if (value is Color color) return new SolidColorBrush(color);

            if (value is string s)
            {
                try
                {
                    var conv = new ColorConverter();
                    var obj = conv.ConvertFromString(s);
                    if (obj is Color c) return new SolidColorBrush(c);
                }
                catch { }
            }

            if (value is int argb)
            {
                var c = Color.FromArgb((byte)((argb >> 24) & 0xFF), (byte)((argb >> 16) & 0xFF), (byte)((argb >> 8) & 0xFF), (byte)(argb & 0xFF));
                return new SolidColorBrush(c);
            }

            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SolidColorBrush sb) return sb.Color;
            return Binding.DoNothing;
        }
    }

    public class FaseAVisibilidadConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b) return b ? Visibility.Visible : Visibility.Collapsed;

            if (value is string s)
            {
                if (bool.TryParse(s, out var bv)) return bv ? Visibility.Visible : Visibility.Collapsed;
                if (string.Equals(s, "Visible", StringComparison.OrdinalIgnoreCase)) return Visibility.Visible;
                if (string.Equals(s, "Collapsed", StringComparison.OrdinalIgnoreCase) || string.Equals(s, "Hidden", StringComparison.OrdinalIgnoreCase)) return Visibility.Collapsed;
            }

            if (value is int i) return i > 0 ? Visibility.Visible : Visibility.Collapsed;

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(bool) && value is Visibility v) return v == Visibility.Visible;
            return Binding.DoNothing;
        }
    }
}