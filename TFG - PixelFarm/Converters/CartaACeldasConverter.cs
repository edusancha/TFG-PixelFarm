
using System.Globalization;
using System.Windows.Data;
using TFG___PixelFarm.Cards;
using TFG___PixelFarm.Pantallas;

namespace TFG___PixelFarm.Converters
{
    public class CartaACeldasConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is Carta carta && carta.Matriz != null)
            {
                int rotacion = 0;
                if (values[1] is Carta seleccionada &&
                    values[2] is int rot &&
                    carta == seleccionada)
                    rotacion = rot;

                var matrizRotada = carta.obtenerMatrizRotada(rotacion);
                var celdas = new List<CeldaCarta>();
                for (int i = 0; i < matrizRotada.GetLength(0); i++)
                    for (int j = 0; j < matrizRotada.GetLength(1); j++)
                        celdas.Add(new CeldaCarta { tipo = matrizRotada[i, j] });
                return celdas;
            }
            return new List<CeldaCarta>();
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    public class CartaAColumnasConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is Carta carta && carta.Matriz != null)
            {
                int rotacion = 0;
                if (values[1] is Carta seleccionada &&
                    values[2] is int rot &&
                    carta == seleccionada)
                    rotacion = rot;

                var matrizRotada = carta.obtenerMatrizRotada(rotacion);
                return matrizRotada.GetLength(1);
            }
            return 2;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
