using System;
using System.Collections.Generic;
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

namespace TFG___PixelFarm.Pantallas
{
    /// <summary>
    /// Lógica de interacción para DialogoPago.xaml
    /// </summary>
    // DialogoPago.xaml.cs
    public partial class DialogoPago : Window
    {
        public Tipo? tipoPagoElegido { get; private set; }
        private CartaMercado _carta;

        public bool usarMoneda => chkUsarMoneda.IsChecked == true;

        public DialogoPago(CartaMercado carta, int lechuga, int tomate, int maiz, bool puedeUsarMoneda)
        {
            InitializeComponent();
            _carta = carta;
            int coste = carta.ObtenerCosteActual();
            txtCoste.Text = $"Coste: {coste} | Tienes: {lechuga} lechuga(s) {tomate} tomate(s) {maiz} maíz";
            chkUsarMoneda.IsEnabled = puedeUsarMoneda;
        }

        private void btnLechuga_Click(object sender, RoutedEventArgs e)
        {
            tipoPagoElegido = Tipo.Lechuga;
            DialogResult = true;
        }

        private void btnTomate_Click(object sender, RoutedEventArgs e)
        {
            tipoPagoElegido = Tipo.Tomate;
            DialogResult = true;
        }

        private void btnMaiz_Click(object sender, RoutedEventArgs e)
        {
            tipoPagoElegido = Tipo.Maiz;
            DialogResult = true;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            tipoPagoElegido = null;
            DialogResult = false;
        }
    }
}
