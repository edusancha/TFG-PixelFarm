using System.Collections.Generic;
using TFG___PixelFarm.Cards;

namespace TFG___PixelFarm.GameState
{
    public static class CartasComercio
    {
        public static List<CartaComercio> generarCartasComercio()
        {
            var lista = new List<CartaComercio>();

            lista.Add(new CartaComercio(
                Tipo.Tomate, 3, 2,
                new Dictionary<Tipo, int>
                {
                    { Tipo.Lechuga, 1 },
                    { Tipo.Tomate,  1 },
                    { Tipo.Maiz,    1 }
                }, 1));

            lista.Add(new CartaComercio(
                Tipo.Lechuga, 3, 2,
                new Dictionary<Tipo, int>
                {
                    { Tipo.Moneda, 1 }
                }, 1));

            lista.Add(new CartaComercio(
                Tipo.Maiz, 3, 2,
                new Dictionary<Tipo, int>
                {
                    { Tipo.Lechuga, 2 }
                }, 1));

            lista.Add(new CartaComercio(
                Tipo.Tomate, 3, 2,
                new Dictionary<Tipo, int>
                {
                    { Tipo.Maiz, 2 }
                }, 1));

            lista.Add(new CartaComercio(
                Tipo.Lechuga, 3, 2,
                new Dictionary<Tipo, int>
                {
                    { Tipo.Tomate, 2 }
                }, 1));

            lista.Add(new CartaComercio(
                Tipo.Maiz, 3, 2,
                new Dictionary<Tipo, int>
                {
                    { Tipo.Maiz, 2 }
                }, 1));

            lista.Add(new CartaComercio(
                Tipo.Tomate, 3, 2,
                new Dictionary<Tipo, int>
                {
                    { Tipo.Aspersor, 1 }
                }, 1));

            lista.Add(new CartaComercio(
                Tipo.Lechuga, 3, 2,
                new Dictionary<Tipo, int>
                {
                    { Tipo.Semillas, 1 }
                }, 1));

            lista.Add(new CartaComercio(
                Tipo.Maiz, 3, 2,
                new Dictionary<Tipo, int>
                {
                    { Tipo.Carretilla, 1 }
                }, 1));

            lista.Add(new CartaComercio(
                Tipo.Tomate, 3, 2,
                new Dictionary<Tipo, int>
                {
                    { Tipo.Maiz, 2 }
                }, 1));

        
            lista.Add(new CartaComercio(
                Tipo.Lechuga, 3, 2,
                new Dictionary<Tipo, int>
                {
                    { Tipo.Tomate, 2 }
                }, 1));

         
            lista.Add(new CartaComercio(
                Tipo.Maiz, 3, 2,
                new Dictionary<Tipo, int>
                {
                    { Tipo.Maiz, 2 }
                }, 1));

            return lista;
        }
    }
}