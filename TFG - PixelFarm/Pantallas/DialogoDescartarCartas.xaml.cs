using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TFG___PixelFarm.Cards;
using System.ComponentModel;

namespace TFG___PixelFarm.Pantallas
{
    /// <summary>
    /// Lógica de interacción para DialogoDescartarCartas.xaml
    /// </summary>
    public partial class DialogoDescartarCartas : Window
    {
        public List<Carta> cartasADescartar { get; private set; }
        private int _cantidadADescartar;

        public DialogoDescartarCartas(List<Carta> cartasEnMano, int cantidadADescartar)
        {
            InitializeComponent();
            _cantidadADescartar = cantidadADescartar;
            listaCartas.ItemsSource = cartasEnMano;
            txtInfo.Text = $"Tienes {cartasEnMano.Count} cartas en mano. " +
                           $"Debes descartar {cantidadADescartar} carta(s). " +
                           $"Selecciona cuáles descartar.";
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            if (listaCartas.SelectedItems.Count != _cantidadADescartar)
            {
                txtError.Text = $"Debes descartar {_cantidadADescartar} carta(s).";
                return;
            }

            cartasADescartar = listaCartas.SelectedItems.Cast<Carta>().ToList();
            DialogResult = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (DialogResult != true)
            {
                e.Cancel = true;
                txtError.Text = $"Debes descartar {_cantidadADescartar} carta(s) antes de continuar.";
            }
            base.OnClosing(e);
        }
    }
}
