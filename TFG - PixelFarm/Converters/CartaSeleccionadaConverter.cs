using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using TFG___PixelFarm.Cards;

namespace TFG___PixelFarm.Converters
{
    public class CartaSeleccionadaConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is Carta carta && values[1] is Carta seleccionada)
                return carta == seleccionada ? Brushes.Yellow : Brushes.Brown;

            return Brushes.Brown;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}