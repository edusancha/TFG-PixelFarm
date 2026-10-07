using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TFG___PixelFarm.Cards;

namespace TFG___PixelFarm.Converters
{
    public class TipoAColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Tipo tipo)
            {
                return tipo switch
                {
                    Tipo.Lechuga => Brushes.LightGreen,
                    Tipo.Tomate => Brushes.Tomato,
                    Tipo.Maiz => Brushes.Yellow,
                    Tipo.CampoArado => Brushes.SandyBrown,
                    Tipo.Plaga => Brushes.DarkOliveGreen,
                    _ => Brushes.Beige
                };
            }
            return Brushes.Beige;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
