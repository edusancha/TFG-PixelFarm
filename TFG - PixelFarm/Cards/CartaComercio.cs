using System;
using System.Collections.Generic;
using System.Text;

namespace TFG___PixelFarm.Cards
{
    public class CartaComercio : Carta
    {
        // Intercambio (parte inferior) — uso una vez por ronda
        public Tipo Da { get; set; }
        public int CantidadDa { get; set; }
        public int CantidadRecibe { get; set; } // siempre monedas

        // Bonus de puntos (parte superior) — se evalúa al final
        public Dictionary<Tipo, int> CostePuntos { get; set; }
        public int PuntosPorCoste { get; set; }

        public bool usadaEsteTurno { get; set; } = false;
        public bool tieneTokenObjetivo { get; set; } = false;
        public int nivelComercio { get; set; } = 0;

        public CartaComercio(
            Tipo da, int cantidadDa, int cantidadRecibe,
            Dictionary<Tipo, int> costePuntos, int puntosPorCoste)
            : base(null, "Comercio", 0, ClaseCarta.Comercio)
        {
            Da = da;
            CantidadDa = cantidadDa;
            CantidadRecibe = cantidadRecibe;
            CostePuntos = costePuntos;
            PuntosPorCoste = puntosPorCoste;
        }
    }
}
