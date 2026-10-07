using System;
using System.Collections.Generic;
using TFG___PixelFarm.Cards;

namespace TFG___PixelFarm.GameState
{
    public class Granja
    {
        public Tipo[,] Grid { get; private set; }

        // Carta visible en cada casilla (la última colocada encima)
        public Carta[,] Propietario { get; private set; }

        public int Filas { get; }
        public int Columnas { get; }

        private static readonly (int, int)[] Direcciones = { (-1, 0), (1, 0), (0, -1), (0, 1) };

        public Granja(int filas, int columnas)
        {
            Filas = filas;
            Columnas = columnas;
            Grid = new Tipo[filas, columnas];
            Propietario = new Carta[filas, columnas];

            for (int i = 0; i < filas; i++)
                for (int j = 0; j < columnas; j++)
                    Grid[i, j] = Tipo.Vacio;
        }


        // Colocación de cartas ------------------------------------------------------------------

        public bool colocarCarta(Carta carta, int fila, int col, Tipo[,] matrizRotada, bool esPrimera = false)
        {
            int f = matrizRotada.GetLength(0);
            int c = matrizRotada.GetLength(1);

            if (fila + f > Filas || col + c > Columnas)
                return false;

            int solapados = 0;
            for (int i = 0; i < f; i++)
                for (int j = 0; j < c; j++)
                    if (matrizRotada[i, j] != Tipo.Vacio &&
                        Grid[fila + i, col + j] != Tipo.CampoArado &&
                        Grid[fila + i, col + j] != Tipo.Vacio)
                        solapados++;

            if (esPrimera && solapados > 0) return false;
            if (!esPrimera && !hayAdyacenciaOrtogonal(matrizRotada, fila, col)) return false;

            for (int i = 0; i < f; i++)
                for (int j = 0; j < c; j++)
                    if (matrizRotada[i, j] != Tipo.Vacio)
                    {
                        Grid[fila + i, col + j] = matrizRotada[i, j];
                        Propietario[fila + i, col + j] = carta;
                    }

            return true;
        }

        private bool hayAdyacenciaOrtogonal(Tipo[,] matrizRotada, int fila, int col)
        {
            int f = matrizRotada.GetLength(0);
            int c = matrizRotada.GetLength(1);

            var solapadas = new List<(int, int)>();
            for (int i = 0; i < f; i++)
                for (int j = 0; j < c; j++)
                    if (matrizRotada[i, j] != Tipo.Vacio &&
                        Grid[fila + i, col + j] != Tipo.Vacio)
                        solapadas.Add((fila + i, col + j));

            if (solapadas.Count != 2) return false;

            bool mismaFila = solapadas[0].Item1 == solapadas[1].Item1;
            bool mismaColumna = solapadas[0].Item2 == solapadas[1].Item2;

            return mismaFila || mismaColumna;
        }


        // Huertos (DFS) -------------------------------------------------------------------------

        public List<(int, int)> obtenerHuertoMasGrande(Tipo tipo)
        {
            var visitado = new bool[Filas, Columnas];
            var mayor = new List<(int, int)>();

            for (int i = 0; i < Filas; i++)
                for (int j = 0; j < Columnas; j++)
                    if (!visitado[i, j] && Grid[i, j] == tipo)
                    {
                        var celdas = new List<(int, int)>();
                        DFSCeldas(i, j, tipo, visitado, celdas);
                        if (celdas.Count > mayor.Count) mayor = celdas;
                    }

            return mayor;
        }

        private void DFSCeldas(int fila, int col, Tipo tipo, bool[,] visitado, List<(int, int)> celdas)
        {
            if (fila < 0 || col < 0 || fila >= Filas || col >= Columnas) return;
            if (visitado[fila, col] || Grid[fila, col] != tipo) return;

            visitado[fila, col] = true;
            celdas.Add((fila, col));

            DFSCeldas(fila + 1, col, tipo, visitado, celdas);
            DFSCeldas(fila - 1, col, tipo, visitado, celdas);
            DFSCeldas(fila, col + 1, tipo, visitado, celdas);
            DFSCeldas(fila, col - 1, tipo, visitado, celdas);
        }

        // Cuenta casillas DISTINTAS de tipoVecino adyacentes al huerto
        public int contarAdyacentesAHuerto(List<(int, int)> huerto, Tipo tipoVecino)
        {
            var encontrados = new HashSet<(int, int)>();
            foreach (var (f, c) in huerto)
                foreach (var (df, dc) in Direcciones)
                {
                    int nf = f + df, nc = c + dc;
                    if (nf < 0 || nc < 0 || nf >= Filas || nc >= Columnas) continue;
                    if (Grid[nf, nc] == tipoVecino) encontrados.Add((nf, nc));
                }
            return encontrados.Count;
        }

        public bool esAdyacenteA(int fila, int col, HashSet<(int, int)> celdas)
        {
            foreach (var (df, dc) in Direcciones)
                if (celdas.Contains((fila + df, col + dc))) return true;
            return false;
        }

        public Dictionary<Tipo, int> calcularHuertoMasGrande()
        {
            var resultado = new Dictionary<Tipo, int>();
            foreach (var tipo in new[] { Tipo.Lechuga, Tipo.Tomate, Tipo.Maiz })
                resultado[tipo] = obtenerHuertoMasGrande(tipo).Count;
            return resultado;
        }

        public Dictionary<Tipo, int> calcularProduccion()
        {
            var resultado = new Dictionary<Tipo, int>();
            foreach (var tipo in new[] { Tipo.Lechuga, Tipo.Tomate, Tipo.Maiz })
            {
                var huerto = obtenerHuertoMasGrande(tipo);
                int produccion = huerto.Count;
                if (produccion > 0)
                    produccion += contarAdyacentesAHuerto(huerto, Tipo.Aspersor);
                resultado[tipo] = produccion;
            }
            return resultado;
        }

        // Verduras de las cartas que no forman parte del huerto más grande
        public Dictionary<Tipo, int> contarVerdurasNoUsadas()
        {
            var huertos = calcularHuertoMasGrande();
            var resultado = new Dictionary<Tipo, int>();

            foreach (var tipo in new[] { Tipo.Lechuga, Tipo.Tomate, Tipo.Maiz })
                resultado[tipo] = Math.Max(0, contarTipo(tipo) - huertos[tipo]);

            return resultado;
        }

        private int contarTipo(Tipo tipo)
        {
            int count = 0;
            for (int i = 0; i < Filas; i++)
                for (int j = 0; j < Columnas; j++)
                    if (Grid[i, j] == tipo) count++;
            return count;
        }

        public int contarCamposAradosVacios()
        {
            return contarTipo(Tipo.CampoArado);
        }


        // Formas de los objetivos de parcela ---------------------------------------------------

        private bool[,] rotarForma(bool[,] matriz)
        {
            int filas = matriz.GetLength(0);
            int columnas = matriz.GetLength(1);

            bool[,] rotada = new bool[columnas, filas];

            for (int i = 0; i < filas; i++)
                for (int j = 0; j < columnas; j++)
                    rotada[j, filas - 1 - i] = matriz[i, j];

            return rotada;
        }

        private List<bool[,]> ObtenerRotaciones(bool[,] forma)
        {
            var lista = new List<bool[,]>();
            var actual = forma;

            for (int i = 0; i < 4; i++)
            {
                lista.Add(actual);
                actual = rotarForma(actual);
            }

            return lista;
        }

        public void consumirForma(bool[,] forma, int fila, int col)
        {
            int f = forma.GetLength(0);
            int c = forma.GetLength(1);

            for (int i = 0; i < f; i++)
                for (int j = 0; j < c; j++)
                    if (forma[i, j])
                        Grid[fila + i, col + j] = Tipo.CampoArado;
        }

        

        public List<(int fila, int col, bool[,] forma, int camposArados, Tipo tipo)> buscarTodasLasFormas(bool[,] forma)
        {
            var opciones = new List<(int, int, bool[,], int, Tipo)>();
            var celdasVistas = new List<HashSet<(int, int)>>();

            foreach (var rot in ObtenerRotaciones(forma))
                for (int i = 0; i < Filas; i++)
                    for (int j = 0; j < Columnas; j++)
                    {
                        var r = encajaFormaConCampos(rot, i, j);
                        if (!r.encaja) continue;

                       
                        var celdas = new HashSet<(int, int)>();
                        for (int a = 0; a < rot.GetLength(0); a++)
                            for (int b = 0; b < rot.GetLength(1); b++)
                                if (rot[a, b])
                                    celdas.Add((i + a, j + b));

                        
                        bool repetida = false;
                        foreach (var vista in celdasVistas)
                            if (vista.SetEquals(celdas)) { repetida = true; break; }
                        if (repetida) continue;

                        celdasVistas.Add(celdas);
                        opciones.Add((i, j, rot, r.camposArados, r.tipoBase));
                    }

            return opciones;
        }

        private (bool encaja, int camposArados, Tipo tipoBase) encajaFormaConCampos(bool[,] forma, int fila, int col)
        {
            int f = forma.GetLength(0);
            int c = forma.GetLength(1);

            Tipo? tipoBase = null;
            int camposArados = 0;

            for (int i = 0; i < f; i++)
                for (int j = 0; j < c; j++)
                {
                    if (!forma[i, j]) continue;

                    int gf = fila + i;
                    int gc = col + j;

                    if (gf >= Filas || gc >= Columnas)
                        return (false, 0, Tipo.Vacio);

                    Tipo tile = Grid[gf, gc];

                    if (tile == Tipo.Vacio)
                        return (false, 0, Tipo.Vacio);

                    if (tile == Tipo.CampoArado)
                    {
                        camposArados++;
                        continue;
                    }

                    if (tipoBase == null)
                        tipoBase = tile;

                    if (tile != tipoBase)
                        return (false, 0, Tipo.Vacio);
                }

            return (true, camposArados, tipoBase ?? Tipo.Vacio);
        }
    }
}