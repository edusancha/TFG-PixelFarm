using System.Collections.Generic;
using TFG___PixelFarm.Cards;

namespace TFG___PixelFarm.GameState
{
    public static class CartasIniciales
    {
        public static List<Carta> generarMazoInicial()
        {
            var mazo = new List<Carta>();

            mazo.Add(new CartaInicio(new Tipo[,]
            {
                { Tipo.Maiz,    Tipo.Tomate  },
                { Tipo.Maiz,    Tipo.CampoArado }
            }, "Inicio 1"));

            mazo.Add(new CartaInicio(new Tipo[,]
            {
                { Tipo.Lechuga, Tipo.Maiz },
                { Tipo.Lechuga, Tipo.CampoArado }
            }, "Inicio 2"));

            mazo.Add(new CartaInicio(new Tipo[,]
            {
                { Tipo.Tomate, Tipo.Lechuga },
                { Tipo.Tomate,  Tipo.CampoArado    }
            }, "Inicio 3"));

            mazo.Add(new CartaInicio(new Tipo[,]
            {
                { Tipo.Lechuga, Tipo.Tomate    },
                { Tipo.Maiz, Tipo.CampoArado }
            }, "Inicio 4"));

            mazo.Add(new CartaInicio(new Tipo[,]
            {
                { Tipo.Maiz,    Tipo.Lechuga },
                { Tipo.Tomate, Tipo.CampoArado }
            }, "Inicio 5"));

            mazo.Add(new CartaInicio(new Tipo[,]
            {
                { Tipo.Tomate, Tipo.Maiz  },
                { Tipo.Lechuga,   Tipo.CampoArado }
            }, "Inicio 6"));

            return mazo;
        }
    }
}