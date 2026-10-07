using System;
using System.Collections.Generic;
using System.Text;

namespace TFG___PixelFarm.Cards
{
    public class CartaObjetivo : Carta
    {
        public int Puntos { get; set; }

        public bool tieneMoneda { get; set; } = false;

        // Para objetivos de tipo Parcela: forma que debe tener la parcela
        public bool[,] forma { get; set; }

        // Para objetivos de tipo Pedido: coste en verduras
        public Dictionary<Tipo, int> CostePedido { get; set; }

        public enum TipoObjetivo
        {
            Pedido, // gastar verduras
            Parcela // forma en la granja
        }

        public TipoObjetivo Subtipo { get; set; }

        // Propiedades para la UI (panel de coste en PantallaJuego.xaml)
        public bool esObjetivoPedido => Subtipo == TipoObjetivo.Pedido;
        public int CosteLechuga => CostePedido?.GetValueOrDefault(Tipo.Lechuga) ?? 0;
        public int CosteTomate => CostePedido?.GetValueOrDefault(Tipo.Tomate) ?? 0;
        public int CosteMaiz => CostePedido?.GetValueOrDefault(Tipo.Maiz) ?? 0;

        public CartaObjetivo(string nombre, int puntos, TipoObjetivo subtipo, bool[,] forma, Dictionary<Tipo, int> CostePedido)
            : base(null, nombre, 0, ClaseCarta.Objetivo)
        {
            Puntos = puntos;
            Subtipo = subtipo;
            this.forma = forma;
            this.CostePedido = CostePedido;
        }
    }
}