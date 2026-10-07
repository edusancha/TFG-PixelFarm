using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TFG___PixelFarm.Cards;

namespace TFG___PixelFarm.Converters
{
    public class CeldaAColorConverter : IMultiValueConverter
    {
        // values[0] = tipo, values[1] = esPreview, values[2] = tipoPreview
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 3 ||
                values[0] is not Tipo tipo ||
                values[1] is not bool esPreview ||
                values[2] is not Tipo tipoPreview)
                return Brushes.SandyBrown;

            if (esPreview)
            {
                return tipoPreview switch
                {
                    Tipo.Lechuga => new SolidColorBrush(Color.FromArgb(180, 144, 238, 144)),
                    Tipo.Tomate => new SolidColorBrush(Color.FromArgb(180, 255, 99, 71)),
                    Tipo.Maiz => new SolidColorBrush(Color.FromArgb(180, 255, 255, 0)),
                    Tipo.CampoArado => new SolidColorBrush(Color.FromArgb(180, 210, 180, 140)),
                    Tipo.Aspersor => new SolidColorBrush(Color.FromArgb(180, 135, 206, 235)),
                    Tipo.Semillas => new SolidColorBrush(Color.FromArgb(180, 144, 238, 144)),
                    Tipo.Carretilla => new SolidColorBrush(Color.FromArgb(180, 210, 180, 140)),
                    Tipo.Pala => new SolidColorBrush(Color.FromArgb(180, 210, 180, 140)),
                    Tipo.Moneda => new SolidColorBrush(Color.FromArgb(180, 255, 215, 0)),
                    _ => new SolidColorBrush(Color.FromArgb(180, 173, 216, 230))
                };
            }

            return tipo switch
            {
                Tipo.Lechuga => Brushes.LightGreen,
                Tipo.Tomate => Brushes.Tomato,
                Tipo.Maiz => Brushes.Yellow,
                Tipo.CampoArado => Brushes.SandyBrown,
                Tipo.Plaga => Brushes.DarkOliveGreen,
                Tipo.Aspersor => Brushes.LightBlue,
                Tipo.Semillas => Brushes.LightGreen,
                Tipo.Carretilla => Brushes.Peru,
                Tipo.Pala => Brushes.Peru,
                Tipo.Moneda => Brushes.Gold,
                _ => Brushes.Transparent
            };
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}