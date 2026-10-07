using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace TFG___PixelFarm.Converters
{
    public class ReduccionAImagenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int reduccion)
            {
                string ruta = reduccion switch
                {
                    1 => "/Assets/Token1.jpg",
                    2 => "/Assets/Token2.jpg",
                    3 => "/Assets/Token3.jpg",
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
