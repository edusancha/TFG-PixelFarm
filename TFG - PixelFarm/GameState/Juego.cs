using System;
using System.Collections.Generic;
using System.Linq;
using TFG___PixelFarm.Cards;
using TFG___PixelFarm.Jugadores;
using static TFG___PixelFarm.Cards.CartaEstacion;

namespace TFG___PixelFarm.GameState
{
    public class Juego
    {
        public Jugador Jugador { get; private set; }
        public Granja Granja { get; private set; }

        public FaseJuego faseActual { get; private set; }
        public TipoEstacion estacionActual { get; set; }

        public List<CartaPlaga> mazoPlagas { get; set; }
        public List<Carta> mazoInicial { get; set; }
        public List<Carta> Mazo { get; set; }

        public List<CartaMercado> cartasMercadoVisibles { get; set; }
        public List<CartaObjetivo> cartasObjetivoVisibles { get; set; }

        public List<CartaMercado> mazoMercado { get; set; }
        public List<Carta> mazoObjetivos { get; set; }

        public int cartasMercadoVisiblesMax { get; private set; }
        public int cartasObjetivoVisiblesMax { get; private set; }

        public CartaComercio cartaComercio { get; set; }
        public List<CartaComercio> mazoComercio { get; set; }
        public List<CartaComercio> cartasComercioCompletadas { get; } = new List<CartaComercio>();

        public bool juegoTerminado { get; private set; }
        public int numJugadores { get; private set; }

        // ── Compras (solitario: 1 de mercado + 1 objetivo + 1 extra por carretilla) ──
        public int comprasMercadoRestantes { get; private set; }
        public int comprasObjetivoRestantes { get; private set; }
        public int comprasExtraRestantes { get; private set; }

        public int comprasRestantes => comprasMercadoRestantes + comprasObjetivoRestantes + comprasExtraRestantes;

        private bool puedeComprarMercado => comprasMercadoRestantes > 0 || comprasExtraRestantes > 0;
        private bool puedeComprarObjetivo => comprasObjetivoRestantes > 0 || comprasExtraRestantes > 0;

        // ── Palas (cada pala: un uso en el mercado y otro en la cosecha) ──
        public int palasMercado { get; private set; }
        public int palasCosecha { get; private set; }

        public bool puedeUsarPala =>
            (faseActual == FaseJuego.Mercado && palasMercado > 0) ||
            (faseActual == FaseJuego.JugarCartas && _cartasColocadasEsteTurno == 0 && palasCosecha > 0);

        private int semillasEsteRound = 0;
        private int _cartasColocadasEsteTurno = 0;
        private List<Carta> _cartasEnGranja = new List<Carta>();
        private List<CartaPlaga> _plagasQueVuelven = new List<CartaPlaga>();


        public Juego(Jugador jugador, Granja granja, List<CartaPlaga> plagas, List<Carta> mazoInicial, int numJugadores)
        {
            Jugador = jugador;
            Granja = granja;
            mazoPlagas = plagas;
            this.mazoInicial = mazoInicial;
            this.numJugadores = numJugadores;
            estacionActual = TipoEstacion.Primavera;
            faseActual = FaseJuego.JugarCartas;

            cartasMercadoVisiblesMax = numJugadores + 2;
            cartasObjetivoVisiblesMax = numJugadores + 1;

            cartasMercadoVisibles = new List<CartaMercado>();
            cartasObjetivoVisibles = new List<CartaObjetivo>();
            mazoObjetivos = new List<Carta>();
        }


        // ── Límite de mano (las plagas no cuentan ni se pueden descartar) ──

        public int cartasADescartar()
        {
            return Math.Max(0, Jugador.CartasEnMano.Count(c => c is not CartaPlaga) - 5);
        }

        public void descartarCartas(List<Carta> cartas)
        {
            foreach (var carta in cartas)
            {
                if (carta is CartaPlaga) continue;
                Jugador.CartasEnMano.Remove(carta);
                Jugador.Descarte.Add(carta);
            }
        }


        //Fase 1 del turno, jugar cartas ---------------------------------------------------------

        public void EjecutarFaseRobarInicial()
        {
            Jugador.Mazo.AddRange(mazoInicial);
            var rng = new Random();
            barajar(Jugador.Mazo, rng);

            Jugador.robar(3, Jugador.Mazo);
            faseActual = FaseJuego.JugarCartas;
        }

        private void barajar(List<Carta> lista, Random rng)
        {
            for (int i = lista.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (lista[i], lista[j]) = (lista[j], lista[i]);
            }
        }

        public bool ejecutarColocarCarta(Carta carta, int fila, int col, int rotacion)
        {

            if (faseActual != FaseJuego.JugarCartas) return false;
            var matriz = carta.obtenerMatrizRotada(rotacion);
            bool esPrimera = _cartasColocadasEsteTurno == 0;

            bool colocada = Granja.colocarCarta(carta, fila, col, matriz, esPrimera);

            if (colocada)
            {
                Jugador.CartasEnMano.Remove(carta);
                _cartasEnGranja.Add(carta);
                _cartasColocadasEsteTurno++;
            }

            return colocada;
        }

        public void terminarFaseJugarCartas()
        {
            semillasEsteRound = contarTipoEnGrid(Tipo.Semillas);

            palasMercado = contarTipoEnGrid(Tipo.Pala);
            palasCosecha = palasMercado;

            // Las plagas no colocadas se quedan en la mano
            foreach (var carta in Jugador.CartasEnMano.Where(c => c is not CartaPlaga))
                Jugador.Descarte.Add(carta);

            Jugador.CartasEnMano.RemoveAll(c => c is not CartaPlaga);
            _cartasColocadasEsteTurno = 0;

            faseActual = FaseJuego.ControlPlagas;
        }


        //Fase 2 del turno, control de plagas ----------------------------------------------------

        public void ejecutarControlPlagas()
        {
            var produccion = calcularProduccionConPlagas();
            comprobarPlagasPorProduccionCero(produccion);
            resolverPlagasDelGrid();
            faseActual = FaseJuego.Produccion;
        }

        // Las plagas nunca pasan por el descarte: van del mazo de plagas a la mano
        private void recibirPlaga()
        {
            if (mazoPlagas.Count == 0) return;
            Jugador.CartasEnMano.Add(mazoPlagas[0]);
            mazoPlagas.RemoveAt(0);
        }

        private void comprobarPlagasPorProduccionCero(Dictionary<Tipo, int> produccion)
        {
            if (estacionActual == TipoEstacion.Invierno) return;

            foreach (var tipo in new[] { Tipo.Lechuga, Tipo.Tomate, Tipo.Maiz })
                if (produccion[tipo] == 0)
                    recibirPlaga();
        }

        // Decide qué plagas de la granja no afectaron a ningún huerto productor
        private void resolverPlagasDelGrid()
        {
            _plagasQueVuelven.Clear();

            var celdasHuertos = new HashSet<(int, int)>();
            foreach (var tipo in new[] { Tipo.Lechuga, Tipo.Tomate, Tipo.Maiz })
                foreach (var celda in Granja.obtenerHuertoMasGrande(tipo))
                    celdasHuertos.Add(celda);

            foreach (var plaga in _cartasEnGranja.OfType<CartaPlaga>())
            {
                bool afecta = false;
                for (int i = 0; i < Granja.Filas && !afecta; i++)
                    for (int j = 0; j < Granja.Columnas && !afecta; j++)
                        if (Granja.Propietario[i, j] == plaga &&
                            Granja.Grid[i, j] == Tipo.Plaga &&
                            Granja.esAdyacenteA(i, j, celdasHuertos))
                            afecta = true;

                if (!afecta) _plagasQueVuelven.Add(plaga);
            }
        }

        private Dictionary<Tipo, int> calcularProduccionConPlagas()
        {
            var produccion = Granja.calcularProduccion();

            foreach (var tipo in new[] { Tipo.Lechuga, Tipo.Tomate, Tipo.Maiz })
            {
                var huerto = Granja.obtenerHuertoMasGrande(tipo);
                int plagas = Granja.contarAdyacentesAHuerto(huerto, Tipo.Plaga);
                produccion[tipo] = Math.Max(0, produccion[tipo] - plagas);
            }
            return produccion;
        }


        //Fase 3 del turno, producción y mercado -------------------------------------------------

        public void inicializarMercado()
        {
            rellenarMercado();
            rellenarObjetivos();

            if (cartasMercadoVisibles.Count >= 1)
                cartasMercadoVisibles[0].ReduccionCoste = 1;
            if (cartasMercadoVisibles.Count >= 2)
                cartasMercadoVisibles[1].ReduccionCoste = 2;
        }

        public void ejecutarProduccion()
        {
            var produccion = calcularProduccionConPlagas();

            aplicarBonusEstacion(produccion);

            foreach (var kv in produccion)
                Jugador.Recursos[kv.Key] += kv.Value;

            faseActual = FaseJuego.Mercado;
        }

        private void aplicarBonusEstacion(Dictionary<Tipo, int> produccion)
        {
            switch (estacionActual)
            {
                case TipoEstacion.Primavera:
                    if (produccion[Tipo.Lechuga] > 0) produccion[Tipo.Lechuga]++;
                    break;
                case TipoEstacion.Verano:
                    if (produccion[Tipo.Tomate] > 0) produccion[Tipo.Tomate]++;
                    break;
                case TipoEstacion.Otono:
                    if (produccion[Tipo.Maiz] > 0) produccion[Tipo.Maiz]++;
                    break;
                case TipoEstacion.Invierno:
                    aplicarInvierno(produccion);
                    break;
            }
        }

        private void aplicarInvierno(Dictionary<Tipo, int> produccion)
        {
            if (!produccion.Any(k => k.Value > 0)) return;

            Tipo tipoMinimo = produccion
                .Where(k => k.Value > 0)
                .OrderBy(k => k.Value)
                .Select(k => k.Key)
                .First();

            produccion[tipoMinimo] = 0;
        }

        public void iniciarFaseCompra()
        {
            comprasMercadoRestantes = 1;
            comprasObjetivoRestantes = 1;
            comprasExtraRestantes = contarTipoEnGrid(Tipo.Carretilla);
            faseActual = FaseJuego.Mercado;
        }

        public int contarTipoEnGrid(Tipo tipo)
        {
            int count = 0;
            for (int i = 0; i < Granja.Filas; i++)
                for (int j = 0; j < Granja.Columnas; j++)
                    if (Granja.Grid[i, j] == tipo)
                        count++;
            return count;
        }

        private void consumirCompraMercado()
        {
            if (comprasMercadoRestantes > 0) comprasMercadoRestantes--;
            else comprasExtraRestantes--;
        }

        private void consumirCompraObjetivo()
        {
            if (comprasObjetivoRestantes > 0) comprasObjetivoRestantes--;
            else comprasExtraRestantes--;
        }

        public bool comprarCartaMercado(CartaMercado carta, Tipo tipoPago, bool usarMoneda)
        {
            if (!puedeComprarMercado) return false;

            bool teniaTokens = carta.ReduccionCoste > 0;

            bool comprada = Jugador.comprarCarta(carta, tipoPago, usarMoneda);
            if (!comprada) return false;

            cartasMercadoVisibles.Remove(carta);
            consumirCompraMercado();

            if (!teniaTokens)
            {
                Jugador.CartasEnMano.Add(carta);
                mejorarCartaComercio();
            }
            else
                Jugador.Descarte.Add(carta);

            return true;
        }

        public bool comprarCartaObjetivo(CartaObjetivo carta, int opcionForma = 0)
        {
            if (!puedeComprarObjetivo) return false;

            bool comprada = intentarComprarObjetivo(carta, opcionForma);
            if (!comprada) return false;

            cartasObjetivoVisibles.Remove(carta);
            consumirCompraObjetivo();

            return true;
        }

        private bool intentarComprarObjetivo(CartaObjetivo carta, int opcionForma)
        {
            if (carta.Subtipo == CartaObjetivo.TipoObjetivo.Pedido)
            {
                foreach (var kv in carta.CostePedido)
                    if (Jugador.Recursos[kv.Key] < kv.Value) return false;

                foreach (var kv in carta.CostePedido)
                    Jugador.Recursos[kv.Key] -= kv.Value;
            }
            else
            {
                var opciones = Granja.buscarTodasLasFormas(carta.forma);
                if (opcionForma < 0 || opcionForma >= opciones.Count) return false;

                var op = opciones[opcionForma];
                if (!Jugador.puedePagarForma(op.tipo, op.camposArados)) return false;

                Granja.consumirForma(op.forma, op.fila, op.col);
                Jugador.pagarForma(op.tipo, op.camposArados);
            }

            if (!carta.tieneMoneda)
            {
                Jugador.CartasEnMano.Add(carta);
                mejorarCartaComercio();
            }
            else
            {
                Jugador.Descarte.Add(carta);
                Jugador.Monedas++;
            }

            return true;
        }

        public bool usarPalaEnMercado(Carta cartaADescartar)
        {
            if (!puedeUsarPala || faseActual != FaseJuego.Mercado) return false;

            if (cartaADescartar is CartaMercado cartaMercado && cartasMercadoVisibles.Contains(cartaMercado))
            {
                cartasMercadoVisibles.Remove(cartaMercado);
                rellenarMercado();
            }
            else if (cartaADescartar is CartaObjetivo cartaObjetivo && cartasObjetivoVisibles.Contains(cartaObjetivo))
            {
                cartasObjetivoVisibles.Remove(cartaObjetivo);
                rellenarObjetivos();
            }
            else return false;

            palasMercado--;
            return true;
        }


        // Comercio ------------------------------------------------------------------------------

        public bool usarCartaComercio()
        {
            if (cartaComercio == null) return false;
            if (cartaComercio.usadaEsteTurno) return false;

            if (Jugador.Recursos[cartaComercio.Da] < cartaComercio.CantidadDa)
                return false;

            Jugador.Recursos[cartaComercio.Da] -= cartaComercio.CantidadDa;
            Jugador.Monedas += cartaComercio.CantidadRecibe;

            cartaComercio.usadaEsteTurno = true;
            return true;
        }

        public void mejorarCartaComercio()
        {
            if (cartaComercio == null) return;

            if (cartaComercio.nivelComercio < 3)
            {
                cartaComercio.nivelComercio++;
                return;
            }

            cartaComercio.tieneTokenObjetivo = true;
            if (!cartasComercioCompletadas.Contains(cartaComercio))
                cartasComercioCompletadas.Add(cartaComercio);

            if (mazoComercio != null && mazoComercio.Count > 0)
            {
                cartaComercio = mazoComercio[0];
                mazoComercio.RemoveAt(0);
            }
        }

        public void resetearComercio()
        {
            if (cartaComercio != null)
                cartaComercio.usadaEsteTurno = false;
        }


        //Fase 4, reabastecimiento ---------------------------------------------------------------

        public void ejecutarReabastecimiento()
        {
            // Plagas que no afectaron a ningún huerto: al fondo del mazo de plagas + 1 moneda
            foreach (var plaga in _plagasQueVuelven)
            {
                _cartasEnGranja.Remove(plaga);
                mazoPlagas.Add(plaga);
                Jugador.Monedas++;
            }
            _plagasQueVuelven.Clear();

            List<CartaObjetivo> objetivosADescartar = cartasObjetivoVisibles
                .Where(c => c.tieneMoneda)
                .ToList();

            foreach (CartaObjetivo carta in objetivosADescartar)
            {
                cartasObjetivoVisibles.Remove(carta);
                Jugador.Monedas++;
            }

            List<CartaMercado> mercadoADescartar = cartasMercadoVisibles
                .Where(c => c.ReduccionCoste >= 3)
                .ToList();

            foreach (CartaMercado carta in mercadoADescartar)
                cartasMercadoVisibles.Remove(carta);

            foreach (CartaObjetivo carta in cartasObjetivoVisibles)
                carta.tieneMoneda = true;

            foreach (CartaMercado carta in cartasMercadoVisibles)
                carta.IncrementarReduccion();

            rellenarMercado();
            rellenarObjetivos();

            Jugador.MonedaUsadaEsteTurno = false;
            resetearComercio();
            faseActual = FaseJuego.CosechaVerduras;
        }

        private void rellenarMercado()
        {
            int huecosNecesarios = cartasMercadoVisiblesMax - cartasMercadoVisibles.Count;

            for (int i = 0; i < huecosNecesarios; i++)
            {
                if (mazoMercado.Count == 0) break;

                cartasMercadoVisibles.Add(mazoMercado[0]);
                mazoMercado.RemoveAt(0);
            }
        }

        private void rellenarObjetivos()
        {
            int huecosNecesarios = cartasObjetivoVisiblesMax - cartasObjetivoVisibles.Count;

            for (int i = 0; i < huecosNecesarios; i++)
            {
                if (mazoObjetivos.Count == 0)
                {
                    juegoTerminado = true;
                    break;
                }

                Carta carta = mazoObjetivos[0];
                mazoObjetivos.RemoveAt(0);

                if (carta is CartaEstacion estacion)
                {
                    procesarCartaEstacion(estacion);
                    i--;
                }
                else
                {
                    cartasObjetivoVisibles.Add(carta as CartaObjetivo);
                }
            }
        }

        private void procesarCartaEstacion(CartaEstacion estacion)
        {
            estacionActual = estacion.Estacion;

            if (estacion.Estacion != TipoEstacion.Invierno)
                recibirPlaga();
        }


        //Fase 5, cosecha de verduras ------------------------------------------------------------

        public bool ejecutarCosechaVerduras(Dictionary<Tipo, int> verdurasAGuardar)
        {
            int camposVacios = Granja.contarCamposAradosVacios();
            int totalGuardadas = verdurasAGuardar.Values.Sum();

            if (totalGuardadas > camposVacios) return false;

            // Se puede guardar de la reserva o de las verduras no usadas de las cartas
            var disponibles = obtenerVerdurasDisponiblesParaGuardar();
            foreach (var kv in verdurasAGuardar)
                if (disponibles.GetValueOrDefault(kv.Key) < kv.Value) return false;

            Jugador.Recursos[Tipo.Lechuga] = verdurasAGuardar.GetValueOrDefault(Tipo.Lechuga, 0);
            Jugador.Recursos[Tipo.Tomate] = verdurasAGuardar.GetValueOrDefault(Tipo.Tomate, 0);
            Jugador.Recursos[Tipo.Maiz] = verdurasAGuardar.GetValueOrDefault(Tipo.Maiz, 0);

            // Las plagas vuelven a la mano; el resto, al descarte
            foreach (var carta in _cartasEnGranja)
            {
                if (carta is CartaPlaga) Jugador.CartasEnMano.Add(carta);
                else Jugador.Descarte.Add(carta);
            }
            _cartasEnGranja.Clear();

            Granja = new Granja(Granja.Filas, Granja.Columnas);
            _cartasColocadasEsteTurno = 0;

            Jugador.robar(3 + semillasEsteRound, Jugador.Mazo);
            Jugador.ajustarMano();

            semillasEsteRound = 0;
            faseActual = FaseJuego.JugarCartas;
            return true;
        }

        public Dictionary<Tipo, int> obtenerVerdurasDisponiblesParaGuardar()
        {
            var noUsadas = Granja.contarVerdurasNoUsadas();

            return new Dictionary<Tipo, int>
            {
                { Tipo.Lechuga, Jugador.Recursos[Tipo.Lechuga] + noUsadas[Tipo.Lechuga] },
                { Tipo.Tomate,  Jugador.Recursos[Tipo.Tomate]  + noUsadas[Tipo.Tomate]  },
                { Tipo.Maiz,    Jugador.Recursos[Tipo.Maiz]    + noUsadas[Tipo.Maiz]    }
            };
        }

        public bool usarPalaEnCosecha(Carta cartaADescartar)
        {
            if (!puedeUsarPala || faseActual != FaseJuego.JugarCartas) return false;
            if (cartaADescartar is CartaPlaga) return false;
            if (!Jugador.CartasEnMano.Contains(cartaADescartar)) return false;

            Jugador.CartasEnMano.Remove(cartaADescartar);
            Jugador.Descarte.Add(cartaADescartar);
            Jugador.robar(1, Jugador.Mazo);

            palasCosecha--;
            return true;
        }


        // Puntuación ----------------------------------------------------------------------------

        public int calcularPuntuacionFinal()
        {
            int puntos = 0;

            puntos += calcularPuntosObjetivos();
            puntos += calcularPuntosMonedas();
            puntos -= calcularPenalidadPlagas();
            puntos += calcularPuntosComercio();

            return puntos;
        }

        private int calcularPuntosObjetivos()
        {
            int puntos = 0;

            foreach (Carta carta in todasLasCartas())
                if (carta is CartaObjetivo objetivo)
                    puntos += objetivo.Puntos;

            return puntos;
        }

        private int calcularPuntosMonedas()
        {
            return Jugador.Monedas;
        }

        private int calcularPenalidadPlagas()
        {
            return todasLasCartas().OfType<CartaPlaga>().Count() * 2;
        }

        private int calcularPuntosComercio()
        {
            int total = 0;
            foreach (var carta in cartasComercioCompletadas)
                total += calcularPuntosDeCartaComercio(carta);
            return total;
        }

        private int calcularPuntosDeCartaComercio(CartaComercio carta)
        {
            int vecesQueSeCumple = int.MaxValue;

            foreach (var kv in carta.CostePuntos)
            {
                int disponible = kv.Key == Tipo.Moneda
                    ? Jugador.Monedas
                    : contarTipoEnTodasLasCartas(kv.Key);

                vecesQueSeCumple = Math.Min(vecesQueSeCumple, disponible / kv.Value);
            }

            if (vecesQueSeCumple == int.MaxValue) return 0;
            return vecesQueSeCumple * carta.PuntosPorCoste;
        }

        private int contarTipoEnTodasLasCartas(Tipo tipo)
        {
            int count = 0;
            foreach (var carta in todasLasCartas())
            {
                if (carta.Matriz == null) continue;
                for (int i = 0; i < carta.Matriz.GetLength(0); i++)
                    for (int j = 0; j < carta.Matriz.GetLength(1); j++)
                        if (carta.Matriz[i, j] == tipo)
                            count++;
            }
            return count;
        }

        private IEnumerable<Carta> todasLasCartas()
        {
            foreach (Carta carta in Jugador.Mazo) yield return carta;
            foreach (Carta carta in Jugador.Descarte) yield return carta;
            foreach (Carta carta in Jugador.CartasEnMano) yield return carta;
            foreach (Carta carta in _cartasEnGranja) yield return carta;
        }
    }
}