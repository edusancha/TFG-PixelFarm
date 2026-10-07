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
    /// Lógica de interacción para DialogoUsarCampos.xaml
    /// </summary>
    public partial class DialogoUsarCampos : Window
    {
        public Tipo? tipoElegido { get; private set; }

        public DialogoUsarCampos(int camposArados, int lechuga, int tomate, int maiz)
        {
            InitializeComponent();
            txtInfo.Text = $"Hay {camposArados} campo(s) arado(s) vacíos en la forma.\n" +
                           $"Tienes: {lechuga} Lechuga       {tomate}Tomate            {maiz} Maiz";
        }

        private void btnLechuga_Click(object sender, RoutedEventArgs e)
        {
            tipoElegido = Tipo.Lechuga;
            DialogResult = true;
        }

        private void btnTomate_Click(object sender, RoutedEventArgs e)
        {
            tipoElegido = Tipo.Tomate;
            DialogResult = true;
        }

        private void btnMaiz_Click(object sender, RoutedEventArgs e)
        {
            tipoElegido = Tipo.Maiz;
            DialogResult = true;
        }

        private void btnSin_Click(object sender, RoutedEventArgs e)
        {
            tipoElegido = null;
            DialogResult = false;
        }
    }
}
