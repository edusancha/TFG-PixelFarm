using System;
using System.Globalization;
using System.Windows.Data;
using TFG___PixelFarm.GameState;

namespace TFG___PixelFarm.Converters
{
    public class FaseATextoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is FaseJuego fase)
            {
                return fase switch
                {
                    FaseJuego.JugarCartas => "Nueva cosecha",
                    FaseJuego.ControlPlagas => "Control de plagas",
                    FaseJuego.Produccion => "Producción",
                    FaseJuego.Mercado => "Mercado",
                    FaseJuego.CosechaVerduras => "Cosecha de verduras",
                    FaseJuego.FinTurno => "Fin de turno",
                    _ => fase.ToString()
                };
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}