using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using TFG___PixelFarm.Cards;

namespace TFG___PixelFarm.Converters
{
    public class TipoAImagenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Tipo tipo)
            {
                string ruta = tipo switch
                {
                    Tipo.Lechuga => "/Assets/Lechuga.jpg",
                    Tipo.Tomate => "/Assets/Tomate.jpg",
                    Tipo.Maiz => "/Assets/Maiz.jpg",
                    Tipo.CampoArado => "/Assets/CampoArado.jpg",
                    Tipo.Semillas => "/Assets/Semillas.jpg",
                    Tipo.Carretilla => "/Assets/Carretilla.jpg",
                    Tipo.Aspersor => "/Assets/Aspersor.jpg",
                    Tipo.Moneda => "/Assets/Moneda1.jpg",
                    Tipo.Plaga => "/Assets/plaga.png",
                    Tipo.Pala => "/Assets/Pala.png",
                    _ => null
                };

                if (ruta != null)
                {
                    var uri = new Uri(ruta, UriKind.Relative);
                    return new BitmapImage(uri);
                }
            }
            return null;
        }

     
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}