using System;
using System.Collections.Generic;
using System.Text;

namespace TFG___PixelFarm.Cards
{
    public class CartaEstacion : Carta
    {


        
        public enum TipoEstacion
        {
            Primavera,
            Verano,
            Otono,
            Invierno
        }
        public TipoEstacion Estacion { get; set; }

        public CartaEstacion(TipoEstacion estacion) : base(null, estacion.ToString(), 0, ClaseCarta.Estacion)
        {
            Estacion = estacion;
        }
    }
}
