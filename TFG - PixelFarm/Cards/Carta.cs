using System;
using System.Collections.Generic;
using System.Text;

namespace TFG___PixelFarm.Cards
{
    public abstract class Carta
    {
        

        public Tipo[,] Matriz { get; set; }
        public string nombre { get; set; }
        public int ID { get; set; }
        public ClaseCarta Clase { get; set; }
        public int Coste { get; set; }

        public int getFilas => Matriz.GetLength(0);
        public int getColumnas => Matriz.GetLength(1);
        //Constructor de la clase
        public Carta(Tipo[,] matriz, string nombre, int coste, ClaseCarta clase)
        {
            Matriz = matriz;
            this.nombre = nombre;
            Coste = coste;
            Clase = clase;
        }

       

        // Método para rotar la matriz 90 grados en sentido horario
        private Tipo[,] rotarMatriz(Tipo[,] matriz)
        {
            int filas = matriz.GetLength(0);
            int columnas = matriz.GetLength(1);

            Tipo[,] rotada = new Tipo[columnas, filas];

            for (int i = 0; i < filas; i++)
            {
                for (int j = 0; j < columnas; j++)
                {
                    rotada[j, filas - 1 - i] = matriz[i, j];
                }
            }

            return rotada;
        }

        // Método para obtener la matriz rotada un número específico de veces
        public Tipo[,] obtenerMatrizRotada(int rotaciones)
        {
            Tipo[,] resultado = Matriz;

            for (int i = 0; i < rotaciones; i++)
            {
                resultado = rotarMatriz(resultado);
            }

            return resultado;
        }



    }
}
