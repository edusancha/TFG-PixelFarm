using System;
using System.Collections.Generic;
using System.Text;

namespace TFG___PixelFarm.Cards
{
    public class CartaMercado : Carta
    {
      

        // Coste base de la carta
        public int CosteBase { get; set; }

        // Reducción actual (por turnos en mercado)
        public int ReduccionCoste { get; set; }

        // Tipo de recurso necesario (opcional si quieres limitar compra)
        public Tipo TipoPago { get; set; }

        // Indica si la carta tiene un efecto especial
        public bool TieneEfecto { get; set; }

        // Lista de efectos especiales (opcional, más avanzado)
        public List<Tipo> Efectos { get; set; }


        public int CosteActual => ObtenerCosteActual();

        // Constructor
        public CartaMercado(Tipo[,] matriz, string nombre, int costeBase, Tipo tipoPago)
            : base(matriz, nombre, costeBase, ClaseCarta.Mercado)
        {
            CosteBase = costeBase;
            ReduccionCoste = 0;
            TipoPago = tipoPago;
            TieneEfecto = false;
            Efectos = new List<Tipo>();
        }

        // Calcular coste real (con reducción)
        public int ObtenerCosteActual()
        {
            int costeFinal = CosteBase - ReduccionCoste;
            return costeFinal < 0 ? 0 : costeFinal;
        }

        // Aumentar reducción (fase de replenishment)
        public void IncrementarReduccion()
        {
            if (ReduccionCoste < 3)
                ReduccionCoste++;
        }

        // Resetear reducción (cuando se compra o se descarta)
        public void resetearReduccion()
        {
            ReduccionCoste = 0;
        }

        // Añadir efecto especial
        public void AgregarEfecto(Tipo efecto)
        {
            TieneEfecto = true;
            Efectos.Add(efecto);
        }
    }
}
