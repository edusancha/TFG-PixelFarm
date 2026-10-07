using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TFG___PixelFarm.Cards;
using TFG___PixelFarm.GameState;

namespace TFG___PixelFarm.Pantallas
{
    /// <summary>
    /// Lógica de interacción para PantallaJuego.xaml
    /// </summary>
    public partial class PantallaJuego : UserControl
    {
        public event Action<int> onPartidaTerminada;
        private GameViewModel _vm;
        private Juego _juego;

        public PantallaJuego(Juego juego)
        {
            InitializeComponent();
            _juego = juego;
            _vm = new GameViewModel(juego);
            _vm.onJuegoTerminado += puntuacion => onPartidaTerminada?.Invoke(puntuacion);
            DataContext = _vm;
        }

        private void btnTerminarColocacion_Click(object sender, RoutedEventArgs e)
        {
            _vm.terminarColocacion();
        }

        private void btnTerminarCompra_Click(object sender, RoutedEventArgs e)
        {
            _vm.terminarCompra();
        }

        private void btnTerminarCosecha_Click(object sender, RoutedEventArgs e)
        {
            int camposVacios = _juego.Granja.contarCamposAradosVacios();

            if (camposVacios == 0)
            {
                _vm.cosecharVerduras(new Dictionary<Tipo, int>
                {
                    { Tipo.Lechuga, 0 },
                    { Tipo.Tomate,  0 },
                    { Tipo.Maiz,    0 }
                });
                return;
            }

            var disponibles = _juego.obtenerVerdurasDisponiblesParaGuardar();

            var dialogo = new DialogoCosecha(
                camposVacios,
                disponibles[Tipo.Lechuga],
                disponibles[Tipo.Tomate],
                disponibles[Tipo.Maiz]
            );
            dialogo.Owner = Window.GetWindow(this);
            if (dialogo.ShowDialog() != true) return;

            if (!_vm.cosecharVerduras(dialogo.verdurasGuardadas))
                MessageBox.Show("No puedes guardar esas verduras.");
        }

        private void btnCarta_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Carta carta)
            {
                _vm.seleccionarCarta(carta);
                e.Handled = true;
            }
        }

        private void btnRotar_Click(object sender, RoutedEventArgs e)
        {
            _vm.rotarCartaSeleccionada();
        }

        private void btnCelda_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.cartaSeleccionada == null) return;

            if (sender is Button btn && btn.DataContext is TipoCell celda)
            {
                bool ok = _vm.colocarCarta(
                    _vm.cartaSeleccionada,
                    celda.fila,
                    celda.col,
                    _vm.rotacionActual
                );

                if (!ok)
                    MessageBox.Show("No se puede colocar aquí");
                else
                    _vm.cartaSeleccionada = null;
            }
        }

        private void btnCelda_Enter(object sender, MouseEventArgs e)
        {
            if (_vm.cartaSeleccionada == null) return;

            if (sender is Button btn && btn.DataContext is TipoCell celda)
                _vm.actualizarPreview(celda.fila, celda.col);
        }

        private void btnCelda_Leave(object sender, MouseEventArgs e)
        {
            _vm.limpiarPreview();
        }

        private void btnComprarMercado_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is CartaMercado carta)
            {
                var dialogo = new DialogoPago(carta, _vm.lechuga, _vm.tomate, _vm.maiz, _vm.puedeUsarMoneda);
                dialogo.Owner = Window.GetWindow(this);

                if (dialogo.ShowDialog() == true && dialogo.tipoPagoElegido.HasValue)
                {
                    bool ok = _vm.comprarMercado(carta, dialogo.tipoPagoElegido.Value, dialogo.usarMoneda);

                    if (!ok)
                        MessageBox.Show("No puedes comprar esta carta.");
                    else
                        comprobarLimiteMano();
                }
            }
        }

        private void btnComprarObjetivo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button btn || btn.DataContext is not CartaObjetivo carta) return;

            int opcion = 0;

            if (carta.Subtipo == CartaObjetivo.TipoObjetivo.Parcela)
            {
                var opciones = _juego.Granja.buscarTodasLasFormas(carta.forma);

                if (opciones.Count > 1)
                {
                    opcion = -1;
                    for (int i = 0; i < opciones.Count && opcion == -1; i++)
                    {
                        _vm.mostrarPreviewForma(opciones[i].forma, opciones[i].fila, opciones[i].col);

                        var r = MessageBox.Show(
                            $"Parcela {i + 1} de {opciones.Count} (resaltada en azul).\n¿Usar esta?",
                            "Elegir parcela", MessageBoxButton.YesNoCancel);

                        if (r == MessageBoxResult.Yes) opcion = i;
                        else if (r == MessageBoxResult.Cancel) break;
                    }

                    _vm.limpiarPreview();
                    if (opcion == -1) return;
                }
            }

            if (!_vm.comprarObjetivo(carta, opcion))
                MessageBox.Show("No puedes comprar esta carta.");
            else
                comprobarLimiteMano();
        }

        private void btnUsarPala_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.faseActual == FaseJuego.Mercado)
            {
                var dialogo = new DialogoElegirCarta(
                    _vm.cartasMercado.Cast<Carta>()
                       .Concat(_vm.cartasObjetivo.Cast<Carta>())
                       .ToList(),
                    "Elige una carta para descartar con la Pala"
                );
                dialogo.Owner = Window.GetWindow(this);

                if (dialogo.ShowDialog() == true && dialogo.cartaElegida != null)
                {
                    if (!_vm.usarPalaEnMercado(dialogo.cartaElegida))
                        MessageBox.Show("No se puede usar la pala aquí.");
                }
            }
            else if (_vm.faseActual == FaseJuego.JugarCartas)
            {
                var dialogo = new DialogoElegirCarta(
                    _vm.cartasEnMano.Where(c => c is not CartaPlaga).ToList(),
                    "Elige una carta de tu mano para descartar y robar otra"
                );
                dialogo.Owner = Window.GetWindow(this);

                if (dialogo.ShowDialog() == true && dialogo.cartaElegida != null)
                {
                    if (!_vm.usarPalaEnCosecha(dialogo.cartaElegida))
                        MessageBox.Show("No se puede usar la pala aquí.");
                }
            }
        }

        private void btnUsarComercio_Click(object sender, RoutedEventArgs e)
        {
            if (!_vm.usarComercio())
                MessageBox.Show("No puedes usar el comercio ahora.");
        }

        private void comprobarLimiteMano()
        {
            int aDescartar = _juego.cartasADescartar();
            if (aDescartar <= 0) return;

            var dialogo = new DialogoDescartarCartas(
                _juego.Jugador.CartasEnMano.Where(c => c is not CartaPlaga).ToList(),
                aDescartar
            );
            dialogo.Owner = Window.GetWindow(this);

            if (dialogo.ShowDialog() == true)
            {
                _juego.descartarCartas(dialogo.cartasADescartar);
                _vm.actualizarTodo();
            }
        }

        private void btnInstrucciones_Click(object sender, RoutedEventArgs e)
        {
            var dialogo = new DialogoInstrucciones();
            dialogo.Owner = Window.GetWindow(this);
            dialogo.ShowDialog();
        }
    }
}