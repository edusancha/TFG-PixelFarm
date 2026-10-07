using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TFG___PixelFarm.Converters
{
    public class EstacionAImagenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string estacion)
            {
                string ruta = estacion switch
                {
                    "Primavera" => "/Assets/Primavera.jpg",
                    "Verano" => "/Assets/Verano.jpg",
                    "Otono" => "/Assets/Otono.jpg",
                    "Invierno" => "/Assets/Invierno.jpg",
                    _ => null
                };

                if (ruta != null)
                    return new BitmapImage(new Uri(ruta, UriKind.Relative));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
