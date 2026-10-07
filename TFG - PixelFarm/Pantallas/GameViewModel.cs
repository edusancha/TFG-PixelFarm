// ViewModels/GameViewModel.cs
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using TFG___PixelFarm.Cards;
using TFG___PixelFarm.GameState;
using TFG___PixelFarm.Pantallas;

namespace TFG___PixelFarm.Pantallas
{

    public class TipoCell : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private Tipo _tipo;


        public Tipo tipo
        {
            get => _tipo;
            set { _tipo = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(tipo))); }
        }

        private bool _esPreview;
        public bool esPreview
        {
            get => _esPreview;
            set { _esPreview = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(esPreview))); }
        }

        public int fila { get; set; }
        public int col { get; set; }

        private Tipo _tipoPreview;
        public Tipo tipoPreview
        {
            get => _tipoPreview;
            set { _tipoPreview = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(tipoPreview))); }
        }
    }


    public class CeldaCarta
    {
        public Tipo tipo { get; set; }
    }
    public class CeldaForma
    {
        public bool activa { get; set; }
    }


    public class GameViewModel : BaseViewModel
    {
        private Juego _juego;

        
        public bool hayPalas => _juego.puedeUsarPala;
        // Fase actual — 
        private FaseJuego _faseActual;
        public FaseJuego faseActual
        {
            get => _faseActual;
            private set { _faseActual = value; onPropertyChanged(nameof(faseActual)); }
        }



        // Recursos del jugador
        private int _lechuga;
        public int lechuga
        {
            get => _lechuga;
            private set { _lechuga = value; onPropertyChanged(nameof(lechuga)); }
        }

        private int _tomate;
        public int tomate
        {
            get => _tomate;
            private set { _tomate = value; onPropertyChanged(nameof(tomate)); }
        }

        private int _maiz;
        public int maiz
        {
            get => _maiz;
            private set { _maiz = value; onPropertyChanged(nameof(maiz)); }
        }

        private int _monedas;
        public int monedas
        {
            get => _monedas;
            private set { _monedas = value; onPropertyChanged(nameof(monedas)); }
        }
      
        


        // Cartas en mano
        public ObservableCollection<Carta> cartasEnMano { get; private set; }

        // Cartas visibles del mercado y objetivos
        public ObservableCollection<CartaMercado> cartasMercado { get; private set; }
        public ObservableCollection<CartaObjetivo> cartasObjetivo { get; private set; }

        // Grid de la granja — matriz aplanada para mostrar en WPF
        public ObservableCollection<TipoCell> celdas { get; private set; }

        public int granjaFilas => _juego.Granja.Filas;

        public int comprasRestantes => _juego.comprasRestantes;

        public int semillasActuales => _juego.contarTipoEnGrid(Tipo.Semillas);

        public string textoCompras =>
           $"Mercado: {_juego.comprasMercadoRestantes} | " +
           $"Objetivos: {_juego.comprasObjetivoRestantes} | " +
           $"Extra: {_juego.comprasExtraRestantes}";

        public int puntosActuales => _juego.calcularPuntuacionFinal();
        public int granjaColumnas => _juego.Granja.Columnas;
        private Carta _cartaSeleccionada;
        public Carta cartaSeleccionada
        {
            get => _cartaSeleccionada;
            set { _cartaSeleccionada = value; onPropertyChanged(nameof(cartaSeleccionada)); }
        }

        private int _rotacionActual = 0;
        public int rotacionActual
        {
            get => _rotacionActual;
            set { _rotacionActual = value % 4; onPropertyChanged(nameof(rotacionActual)); }
        }
       
        public void seleccionarCarta(Carta carta)
        {
            cartaSeleccionada = cartaSeleccionada == carta ? null : carta;
            rotacionActual = 0;
        }

        public void rotarCartaSeleccionada()
        {
            if (cartaSeleccionada != null)
                rotacionActual++;
        }
        public int nivelComercio => _juego.cartaComercio?.nivelComercio ?? 0;
        public bool comercioTieneTokenObjetivo => _juego.cartaComercio?.tieneTokenObjetivo ?? false;

        public event Action<int> onJuegoTerminado;

        public string estacionActual
        {
            get
            {
                return _juego.estacionActual switch
                {
                    CartaEstacion.TipoEstacion.Primavera => "Primavera",
                    CartaEstacion.TipoEstacion.Verano => "Verano",
                    CartaEstacion.TipoEstacion.Otono => "Otono",
                    CartaEstacion.TipoEstacion.Invierno => "Invierno",
                    _ => ""
                };
            }
        }

        // ── Constructor ─────────────────────────────────────────

        public GameViewModel(Juego juego)
        {
            _juego = juego;
            cartasEnMano = new ObservableCollection<Carta>();
            cartasMercado = new ObservableCollection<CartaMercado>();
            cartasObjetivo = new ObservableCollection<CartaObjetivo>();
            celdas = new ObservableCollection<TipoCell>();

            actualizarTodo();
        }

        // ── Acciones del jugador ─────────────────────────────────

        public bool colocarCarta(Carta carta, int fila, int col, int rotacion)
        {
            bool ok = _juego.ejecutarColocarCarta(carta, fila, col, rotacion);
            if (ok) actualizarTodo();
            return ok;
        }

        public void terminarColocacion()
        {
            _juego.terminarFaseJugarCartas();
            _juego.ejecutarControlPlagas();
            _juego.ejecutarProduccion();
            _juego.iniciarFaseCompra();
            actualizarTodo();
        }

        public bool comprarMercado(CartaMercado carta, Tipo tipoPago, bool usarMoneda)
        {
            bool ok = _juego.comprarCartaMercado(carta, tipoPago, usarMoneda);
            if (ok) actualizarTodo();
            return ok;
        }

        public bool comprarObjetivo(CartaObjetivo carta, int opcionForma = 0)
        {
            bool ok = _juego.comprarCartaObjetivo(carta, opcionForma);
            if (ok) actualizarTodo();
            return ok;
        }

        public void terminarCompra()
        {
            _juego.ejecutarReabastecimiento();
            actualizarTodo();
        }

        public bool cosecharVerduras(Dictionary<Tipo, int> verdurasAGuardar)
        {
            bool ok = _juego.ejecutarCosechaVerduras(verdurasAGuardar);
            if (ok) actualizarTodo();
            return ok;
        }

        public string descripcionComercio
        {
            get
            {
                if (_juego.cartaComercio == null) return "Sin carta de comercio";
                var c = _juego.cartaComercio;
                string intercambio = $"{c.CantidadDa} {c.Da} → {c.CantidadRecibe} Monedas";
                string nivel = c.nivelComercio > 0 ? $" | Nivel {c.nivelComercio}" : "";
                string objetivo = c.tieneTokenObjetivo ? " | Token objetivo" : "";
                return $"{intercambio}{nivel}{objetivo}";
            }
        }

        public bool puedeUsarComercio => _juego.cartaComercio != null
                                        && !_juego.cartaComercio.usadaEsteTurno;

        public bool puedeUsarMoneda => _juego.Jugador.Monedas > 0
                            && !_juego.Jugador.MonedaUsadaEsteTurno;

        public bool usarComercio()
        {
            bool ok = _juego.usarCartaComercio();
            if (ok) actualizarTodo();
            return ok;
        }

        public void mostrarPreviewForma(bool[,] forma, int fila, int col)
        {
            _celdasPreview.Clear();
            for (int i = 0; i < forma.GetLength(0); i++)
                for (int j = 0; j < forma.GetLength(1); j++)
                    if (forma[i, j])
                        _celdasPreview[(fila + i, col + j)] = Tipo.Vacio;   // se ve en azul claro
            actualizarGranja();
        }

        // Sincronización con el modelo ---------------------------------------------

        public void actualizarTodo()
        {
            faseActual = _juego.faseActual;

            lechuga = _juego.Jugador.Recursos[Tipo.Lechuga];
            tomate = _juego.Jugador.Recursos[Tipo.Tomate];
            maiz = _juego.Jugador.Recursos[Tipo.Maiz];
            monedas = _juego.Jugador.Monedas;
            onPropertyChanged(nameof(comprasRestantes));
            onPropertyChanged(nameof(descripcionComercio));
            onPropertyChanged(nameof(puedeUsarComercio));
            onPropertyChanged(nameof(puntosActuales));
            onPropertyChanged(nameof(estacionActual));
            onPropertyChanged(nameof(nivelComercio));
            onPropertyChanged(nameof(comercioTieneTokenObjetivo));
            onPropertyChanged(nameof(textoCompras));

            actualizarMano();
            actualizarMercado();
            actualizarGranja();
            comprobarFinJuego();
        }

        private void actualizarMano()
        {
            cartasEnMano.Clear();
            foreach (var carta in _juego.Jugador.CartasEnMano)
                cartasEnMano.Add(carta);
        }

        private void actualizarMercado()
        {
            cartasMercado.Clear();
            foreach (var carta in _juego.cartasMercadoVisibles)
                cartasMercado.Add(carta);

            cartasObjetivo.Clear();
            foreach (var carta in _juego.cartasObjetivoVisibles)
                cartasObjetivo.Add(carta);
        }

        private void actualizarGranja()
        {
            if (celdas.Count == _juego.Granja.Filas * _juego.Granja.Columnas)
            {
                int index = 0;
                for (int i = 0; i < _juego.Granja.Filas; i++)
                {
                    for (int j = 0; j < _juego.Granja.Columnas; j++)
                    {
                        celdas[index].tipo = _juego.Granja.Grid[i, j];
                        celdas[index].esPreview = esCeldaPreview(i, j);
                        celdas[index].tipoPreview = obtenerTipoPreview(i, j);
                        index++;
                    }
                }
            }
            else
            {
                celdas.Clear();
                for (int i = 0; i < _juego.Granja.Filas; i++)
                    for (int j = 0; j < _juego.Granja.Columnas; j++)
                        celdas.Add(new TipoCell
                        {
                            tipo = _juego.Granja.Grid[i, j],
                            fila = i,
                            col = j,
                            esPreview = esCeldaPreview(i, j),
                            tipoPreview = obtenerTipoPreview(i, j)
                        });
            }

            onPropertyChanged(nameof(hayPalas));
            onPropertyChanged(nameof(semillasActuales));
        }

        public Dictionary<(int, int), Tipo> _celdasPreview = new Dictionary<(int, int), Tipo>();


        public void actualizarPreview(int fila, int col)
        {
            _celdasPreview.Clear();

            if (cartaSeleccionada == null) return;

            var matriz = cartaSeleccionada.obtenerMatrizRotada(rotacionActual);
            int f = matriz.GetLength(0);
            int c = matriz.GetLength(1);

            for (int i = 0; i < f; i++)
                for (int j = 0; j < c; j++)
                    if (matriz[i, j] != Tipo.Vacio)
                        _celdasPreview[(fila + i, col + j)] = matriz[i, j];

            actualizarGranja();
        }

        public void limpiarPreview()
        {
            _celdasPreview.Clear();
            actualizarGranja();
        }

        // Clase auxiliar para representar cada celda del grid en la UI
        public bool esCeldaPreview(int fila, int col)
        {
            return _celdasPreview.ContainsKey((fila, col));
        }


        

        public bool usarPalaEnMercado(Carta carta)
        {
            bool ok = _juego.usarPalaEnMercado(carta);
            if (ok) actualizarTodo();
            return ok;
        }

        public bool usarPalaEnCosecha(Carta carta)
        {
            bool ok = _juego.usarPalaEnCosecha(carta);
            if (ok) actualizarTodo();
            return ok;
        }

        //Comprobaciones para terminar el juego ------------------------------------------------------


        private void comprobarFinJuego()
        {
            if (_juego.juegoTerminado)
            {
                int puntuacion = _juego.calcularPuntuacionFinal();
                onJuegoTerminado?.Invoke(puntuacion);
            }
        }

        //Mejoras visuales------------------------------------------------------


        public Tipo obtenerTipoPreview(int fila, int col)
        {
            if (_celdasPreview.TryGetValue((fila, col), out Tipo tipo))
                return tipo;
            return Tipo.CampoArado;
        }

    }


}