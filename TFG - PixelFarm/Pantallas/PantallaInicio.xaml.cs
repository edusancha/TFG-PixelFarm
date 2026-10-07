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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TFG___PixelFarm.Pantallas
{
    /// <summary>
    /// Lógica de interacción para PantallaInicio.xaml
    /// </summary>
    public partial class PantallaInicio : UserControl
    {
        public event Action onNuevaPartida;

        public PantallaInicio()
        {
            InitializeComponent();
        }

        private void btnNuevaPartida_Click(object sender, RoutedEventArgs e)
        {
            onNuevaPartida?.Invoke();
        }

        private void btnInstrucciones_Click(object sender, RoutedEventArgs e)
        {
            var dialogo = new DialogoInstrucciones();
            dialogo.Owner = Window.GetWindow(this);
            dialogo.ShowDialog();
        }
    }
}
