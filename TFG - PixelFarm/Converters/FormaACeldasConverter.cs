using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using TFG___PixelFarm.Cards;
using TFG___PixelFarm.Pantallas;

namespace TFG___PixelFarm.Converters
{
    public class FormaACeldasConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CartaObjetivo carta && carta.forma != null)
            {
                var celdas = new List<CeldaForma>();
                for (int i = 0; i < carta.forma.GetLength(0); i++)
                    for (int j = 0; j < carta.forma.GetLength(1); j++)
                        celdas.Add(new CeldaForma { activa = carta.forma[i, j] });
                return celdas;
            }
            return new List<CeldaForma>();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    public class FormaAColumnasConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CartaObjetivo carta && carta.forma != null)
                return carta.forma.GetLength(1);
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
    public class ActivaAColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool activa)
                return activa
                    ? System.Windows.Media.Color.FromRgb(0, 100, 0)   // verde oscuro
                    : System.Windows.Media.Color.FromArgb(0, 0, 0, 0); // transparente
            return System.Windows.Media.Color.FromArgb(0, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}