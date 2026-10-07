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
    /// Lógica de interacción para DialogoElegirCarta.xaml
    /// </summary>
    public partial class DialogoElegirCarta : Window
    {
        public Carta cartaElegida { get; private set; }

        public DialogoElegirCarta(List<Carta> cartas, string titulo)
        {
            InitializeComponent();
            txtTitulo.Text = titulo;
            listaCartas.ItemsSource = cartas;
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            cartaElegida = listaCartas.SelectedItem as Carta;
            if (cartaElegida == null)
            {
                MessageBox.Show("Elige una carta primero.");
                return;
            }
            DialogResult = true;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
