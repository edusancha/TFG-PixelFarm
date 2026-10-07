using System;
using System.Collections.Generic;
using System.Linq;
using TFG___PixelFarm.Cards;

namespace TFG___PixelFarm.Jugadores
{
    public class Jugador
    {
        public Dictionary<Tipo, int> Recursos { get; set; }

        public int Monedas { get; set; }

        public List<Carta> CartasEnMano { get; set; }
        public List<Carta> Mazo { get; set; }
        public List<Carta> Descarte { get; set; }

        // Solo se puede usar una vez por turno
        public bool MonedaUsadaEsteTurno { get; set; }

        public Jugador()
        {
            Recursos = new Dictionary<Tipo, int>()
            {
                { Tipo.Lechuga, 0 },
                { Tipo.Tomate, 0 },
                { Tipo.Maiz, 0 }
            };
            CartasEnMano = new List<Carta>();
            Mazo = new List<Carta>();
            Descarte = new List<Carta>();

            Monedas = 0;
        }

        public bool comprarCarta(CartaMercado carta, Tipo tipoPago, bool usarMoneda)
        {
            int coste = carta.ObtenerCosteActual();

            if (usarMoneda && Monedas > 0 && !MonedaUsadaEsteTurno)
            {
                if (Recursos[tipoPago] + 1 < coste) return false;
                Monedas--;
                MonedaUsadaEsteTurno = true;
                coste--;
            }
            else
            {
                if (Recursos[tipoPago] < coste) return false;
            }

            Recursos[tipoPago] -= coste;
            carta.resetearReduccion();
            return true;
        }

        public void robar(int cantidad, List<Carta> mazoElegido)
        {
            for (int i = 0; i < cantidad; i++)
            {
                if (mazoElegido.Count == 0)
                {
                    if (Descarte.Count == 0) break;

                    mazoElegido.AddRange(Descarte);
                    Descarte.Clear();
                    barajar(mazoElegido);
                }

                if (mazoElegido.Count > 0)
                {
                    Carta carta = mazoElegido[0];
                    mazoElegido.RemoveAt(0);
                    CartasEnMano.Add(carta);
                }
            }
        }

        private void barajar(List<Carta> lista)
        {
            var rng = new Random();
            for (int i = lista.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (lista[i], lista[j]) = (lista[j], lista[i]);
            }
        }

        // Las plagas no cuentan para el límite de 5 ni se descartan
        public void ajustarMano()
        {
            while (CartasEnMano.Count(c => c is not CartaPlaga) > 5)
            {
                var carta = CartasEnMano.First(c => c is not CartaPlaga);
                CartasEnMano.Remove(carta);
                Descarte.Add(carta);
            }
        }

        public bool puedePagarForma(Tipo tipo, int camposArados)
        {
            if (tipo == Tipo.Vacio)
                return false;

            return Recursos[tipo] >= camposArados;
        }

        public void pagarForma(Tipo tipo, int camposArados)
        {
            Recursos[tipo] -= camposArados;
        }
    }
}