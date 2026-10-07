using System;
using System.Collections.Generic;
using System.Windows;
using TFG___PixelFarm.Cards;
using TFG___PixelFarm.GameState;
using TFG___PixelFarm.Jugadores;
using TFG___PixelFarm.Pantallas;

namespace TFG___PixelFarm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            iniciarMusica();
            mostrarPantallaInicio();
        }

        // Navegar a la pantalla de inicio
        public void mostrarPantallaInicio()
        {
            var pantalla = new PantallaInicio();
            pantalla.onNuevaPartida += mostrarPantallaJuego;
            contenidoPrincipal.Content = pantalla;
        }

        // Navegar a la pantalla de juego
        public void mostrarPantallaJuego()
        {
            var juego = crearNuevaPartida();
            var pantalla = new PantallaJuego(juego);
            pantalla.onPartidaTerminada += mostrarPantallaFin;
            contenidoPrincipal.Content = pantalla;
        }

        // Navegar a la pantalla de fin
        public void mostrarPantallaFin(int puntuacion)
        {
            var pantalla = new PantallaFin(puntuacion);
            pantalla.onVolverAlInicio += mostrarPantallaInicio;
            contenidoPrincipal.Content = pantalla;
        }

        // Crear e inicializar una nueva partida---------------------------------------------------------
        private Juego crearNuevaPartida()
        {
            var jugador = new Jugador();
            var granja = new Granja(4, 4);
            var mazoPlagas = generarMazoPlagas();
            var mazoInicial = generarMazoInicial();

            var juego = new Juego(jugador, granja, mazoPlagas, mazoInicial, 1);

            juego.mazoMercado = generarMazoMercado();
            juego.mazoObjetivos = generarMazoObjetivos();
            juego.cartasMercadoVisibles = new List<CartaMercado>();
            juego.cartasObjetivoVisibles = new List<CartaObjetivo>();

            var mazoComercio = CartasComercio.generarCartasComercio();
            var rngComercio = new Random();
            juego.cartaComercio = mazoComercio[rngComercio.Next(mazoComercio.Count)];
            mazoComercio.Remove(juego.cartaComercio);
            juego.mazoComercio = mazoComercio;

            juego.inicializarMercado();
            juego.EjecutarFaseRobarInicial();

            return juego;
        }

      //Plagas
        private List<CartaPlaga> generarMazoPlagas()
        {
            Tipo pla = Tipo.Plaga, lec = Tipo.Lechuga, tom = Tipo.Tomate, mai = Tipo.Maiz;

            var matrices = new List<Tipo[,]>
            {
                new Tipo[,] { { pla, lec }, { tom, lec } },
                new Tipo[,] { { pla, tom }, { mai, tom } },
                new Tipo[,] { { pla, mai }, { lec, mai } },
                new Tipo[,] { { pla, lec }, { mai, lec } },
                new Tipo[,] { { pla, tom }, { lec, tom } },
                new Tipo[,] { { pla, mai }, { tom, mai } },
                new Tipo[,] { { pla, lec }, { lec, tom } },
                new Tipo[,] { { pla, tom }, { tom, mai } },
                new Tipo[,] { { pla, mai }, { mai, lec } },
                new Tipo[,] { { pla, lec }, { tom, mai } },
                new Tipo[,] { { pla, tom }, { mai, lec } },
                new Tipo[,] { { pla, mai }, { lec, tom } },
            };

            var mazo = new List<CartaPlaga>();
            for (int i = 0; i < matrices.Count; i++)
                mazo.Add(new CartaPlaga(matrices[i], $"Plaga {i + 1}", 0));

            barajar(mazo, new Random());
            return mazo;
        }

        private List<Carta> generarMazoInicial()
        {
            return CartasIniciales.generarMazoInicial();
        }

        private List<CartaMercado> generarMazoMercado()
        {
            var mazo = new List<CartaMercado>();

            // Fila 1 — coste 5
            mazo.Add(new CartaMercado(new Tipo[,]
            {
                { Tipo.Tomate,  Tipo.Lechuga },
                { Tipo.Tomate,  Tipo.Lechuga }
            }, "Tomate-Lechuga", 5, Tipo.Tomate));

            mazo.Add(new CartaMercado(new Tipo[,]
            {
                { Tipo.Maiz,   Tipo.Maiz    },
                { Tipo.Maiz,   Tipo.Lechuga }
            }, "Maiz-Lechuga", 5, Tipo.Maiz));

            mazo.Add(new CartaMercado(new Tipo[,]
            {
                { Tipo.Tomate, Tipo.Tomate },
                { Tipo.Tomate, Tipo.Maiz   }
            }, "Tomate-Maiz", 5, Tipo.Tomate));

            // Fila 2 — coste 6
            mazo.Add(new CartaMercado(new Tipo[,]
            {
                { Tipo.Lechuga, Tipo.Lechuga },
                { Tipo.Lechuga, Tipo.Lechuga }
            }, "Lechuga x4", 6, Tipo.Lechuga));

            mazo.Add(new CartaMercado(new Tipo[,]
            {
                { Tipo.Maiz, Tipo.Maiz },
                { Tipo.Maiz, Tipo.Maiz }
            }, "Maiz x4", 6, Tipo.Maiz));

            mazo.Add(new CartaMercado(new Tipo[,]
            {
                { Tipo.Tomate, Tipo.Tomate },
                { Tipo.Tomate, Tipo.Tomate }
            }, "Tomate x4", 6, Tipo.Tomate));

         
            mazo.Add(new CartaMercado(new Tipo[,]
            {
                { Tipo.Pala,       Tipo.Carretilla },
                { Tipo.CampoArado, Tipo.CampoArado }
            }, "Pala-Carretilla", 6, Tipo.Lechuga));

            mazo.Add(new CartaMercado(new Tipo[,]
            {
                { Tipo.Pala,     Tipo.Carretilla },
                { Tipo.Aspersor, Tipo.Aspersor   }
            }, "Pala-Aspersor", 6, Tipo.Tomate));

            mazo.Add(new CartaMercado(new Tipo[,]
            {
                { Tipo.Carretilla, Tipo.Pala       },
                { Tipo.CampoArado, Tipo.CampoArado }
            }, "Carretilla-Pala", 6, Tipo.Maiz));

            // Fila 4 — coste 4
            mazo.Add(new CartaMercado(new Tipo[,]
            {
                { Tipo.Pala,       Tipo.Lechuga },
                { Tipo.Carretilla, Tipo.Lechuga }
            }, "Pala-Lechuga", 4, Tipo.Lechuga));

            mazo.Add(new CartaMercado(new Tipo[,]
            {
                { Tipo.Carretilla, Tipo.Maiz     },
                { Tipo.Tomate,     Tipo.Aspersor }
            }, "Carretilla-Tomate", 4, Tipo.Tomate));

            mazo.Add(new CartaMercado(new Tipo[,]
            {
                { Tipo.Aspersor, Tipo.Tomate  },
                { Tipo.Pala,     Tipo.Lechuga }
            }, "Aspersor-Tomate", 4, Tipo.Tomate));

            barajar(mazo, new Random());
            return mazo;
        }

        private List<Carta> generarMazoObjetivos()
        {
            var cartas2Puntos = new List<Carta>();
            var cartas3Puntos = new List<Carta>();

            bool[,] formaI = new bool[,]
            {
                { true, true }
            };

            bool[,] formaL = new bool[,]
            {
                { false, false },
                { true,  false },
                { true,  true  }
            };

            bool[,] formaGusano3 = new bool[,]
            {
                { true, false },
                { true, false },
                { true, false }
            };

            bool[,] formaGusano4 = new bool[,]
            {
                { true, false },
                { true, false },
                { true, false },
                { true, false }
            };

            bool[,] formaT = new bool[,]
            {
                { true,  true, true  },
                { false, true, false }
            };

            cartas2Puntos.Add(new CartaObjetivo("Pedido Lechuga", 2,
                CartaObjetivo.TipoObjetivo.Pedido, null,
                new Dictionary<Tipo, int> { { Tipo.Lechuga, 2 } })
            { Matriz = new Tipo[,] { { Tipo.Lechuga, Tipo.Lechuga }, { Tipo.Lechuga, Tipo.Lechuga } } });

            cartas2Puntos.Add(new CartaObjetivo("Pedido Tomate", 2,
                CartaObjetivo.TipoObjetivo.Pedido, null,
                new Dictionary<Tipo, int> { { Tipo.Tomate, 2 } })
            { Matriz = new Tipo[,] { { Tipo.Tomate, Tipo.Tomate }, { Tipo.Tomate, Tipo.Tomate } } });

            cartas2Puntos.Add(new CartaObjetivo("Objetivo 1", 2,
                CartaObjetivo.TipoObjetivo.Parcela, formaI, null)
            { Matriz = new Tipo[,] { { Tipo.CampoArado, Tipo.CampoArado }, { Tipo.Maiz, Tipo.Maiz } } });

            cartas2Puntos.Add(new CartaObjetivo("Objetivo 4", 2,
                CartaObjetivo.TipoObjetivo.Parcela, formaI, null)
            { Matriz = new Tipo[,] { { Tipo.CampoArado, Tipo.CampoArado }, { Tipo.Aspersor, Tipo.CampoArado } } });

            cartas2Puntos.Add(new CartaObjetivo("Objetivo 5", 2,
                CartaObjetivo.TipoObjetivo.Parcela, formaL, null)
            { Matriz = new Tipo[,] { { Tipo.CampoArado, Tipo.CampoArado }, { Tipo.Tomate, Tipo.Tomate } } });

            cartas2Puntos.Add(new CartaObjetivo("Objetivo 6", 2,
                CartaObjetivo.TipoObjetivo.Parcela, formaGusano3, null)
            { Matriz = new Tipo[,] { { Tipo.CampoArado, Tipo.CampoArado }, { Tipo.Lechuga, Tipo.Lechuga } } });

            cartas2Puntos.Add(new CartaObjetivo("Objetivo 7", 1,
                CartaObjetivo.TipoObjetivo.Parcela, formaI, null)
            { Matriz = new Tipo[,] { { Tipo.CampoArado, Tipo.CampoArado }, { Tipo.Moneda, Tipo.CampoArado } } });

            cartas2Puntos.Add(new CartaObjetivo("Objetivo 8", 2,
                CartaObjetivo.TipoObjetivo.Parcela, formaL, null)
            { Matriz = new Tipo[,] { { Tipo.CampoArado, Tipo.CampoArado }, { Tipo.Semillas, Tipo.CampoArado } } });

            cartas2Puntos.Add(new CartaObjetivo("Objetivo 9", 2,
                CartaObjetivo.TipoObjetivo.Parcela, formaGusano3, null)
            { Matriz = new Tipo[,] { { Tipo.CampoArado, Tipo.CampoArado }, { Tipo.Carretilla, Tipo.CampoArado } } });

            cartas3Puntos.Add(new CartaObjetivo("Pedido lechuga tomate", 3,
                CartaObjetivo.TipoObjetivo.Pedido, null,
                new Dictionary<Tipo, int> { { Tipo.Lechuga, 1 }, { Tipo.Tomate, 1 } })
            { Matriz = new Tipo[,] { { Tipo.Lechuga, Tipo.Tomate }, { Tipo.Lechuga, Tipo.Tomate } } });

            cartas3Puntos.Add(new CartaObjetivo("Objetivo 2", 3,
                CartaObjetivo.TipoObjetivo.Parcela, formaGusano4, null)
            { Matriz = new Tipo[,] { { Tipo.CampoArado, Tipo.CampoArado }, { Tipo.Carretilla, Tipo.Aspersor } } });

            cartas3Puntos.Add(new CartaObjetivo("Objetivo 10", 3,
                CartaObjetivo.TipoObjetivo.Parcela, formaGusano4, null)
            { Matriz = new Tipo[,] { { Tipo.CampoArado, Tipo.CampoArado },{ Tipo.Lechuga, Tipo.Aspersor } } });

            cartas3Puntos.Add(new CartaObjetivo("Objetivo 3", 3,
                CartaObjetivo.TipoObjetivo.Parcela, formaT, null)
            { Matriz = new Tipo[,] { { Tipo.CampoArado, Tipo.CampoArado },{ Tipo.Aspersor, Tipo.Semillas } } });

            var rng = new Random();
            barajar(cartas2Puntos, rng);
            barajar(cartas3Puntos, rng);

            int cartasPorGrupo = 2;
            var mazo = new List<Carta>();

            mazo.AddRange(cartas2Puntos.GetRange(0, cartasPorGrupo));
            mazo.Add(new CartaEstacion(CartaEstacion.TipoEstacion.Verano));

            mazo.AddRange(cartas2Puntos.GetRange(cartasPorGrupo, cartasPorGrupo));
            mazo.Add(new CartaEstacion(CartaEstacion.TipoEstacion.Otono));

            mazo.AddRange(cartas3Puntos.GetRange(0, cartasPorGrupo));
            mazo.Add(new CartaEstacion(CartaEstacion.TipoEstacion.Invierno));

            mazo.AddRange(cartas3Puntos.GetRange(cartasPorGrupo, cartasPorGrupo));

            return mazo;
        }

        // Genérico: sirve para List<Carta>, List<CartaPlaga>, etc.
        private void barajar<T>(List<T> lista, Random rng)
        {
            for (int i = lista.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (lista[i], lista[j]) = (lista[j], lista[i]);
            }
        }

        private System.Windows.Media.MediaPlayer _musicPlayer;

        private void iniciarMusica()
        {
            _musicPlayer = new System.Windows.Media.MediaPlayer();
            _musicPlayer.Open(new Uri("pack://siteoforigin:,,,/Assets/Soundtrack.mp3"));
            _musicPlayer.Volume = 0.05;
            _musicPlayer.MediaEnded += (s, e) =>
            {
                _musicPlayer.Position = TimeSpan.Zero;
                _musicPlayer.Play();
            };
            _musicPlayer.Play();
        }
    }
}