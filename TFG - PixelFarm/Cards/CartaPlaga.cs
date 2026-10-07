using System;
using System.Collections.Generic;
using System.Text;

namespace TFG___PixelFarm.Cards
{
    public class CartaPlaga : Carta
    {
        public int Penalizacion { get; set; } = -2;
        public CartaPlaga(Tipo[,] matriz, String nombre, int coste) : base(matriz, nombre, 0, ClaseCarta.Plaga)
        {

        }
    }
}
