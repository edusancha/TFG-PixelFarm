using System;
using System.Collections.Generic;
using System.Text;

using TFG___PixelFarm.Cards;

namespace TFG___PixelFarm.Cards
{
    public class CartaInicio : Carta
    {
        public CartaInicio(Tipo[,] matriz, string nombre)
            : base(matriz, nombre, 0, ClaseCarta.Inicio)
        {
        }
    }
}

