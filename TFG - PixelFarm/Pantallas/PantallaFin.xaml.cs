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
    /// Lógica de interacción para PantallaFin.xaml
    /// </summary>
    public partial class PantallaFin : UserControl
    {
        public event Action onVolverAlInicio;

        public PantallaFin(int puntuacion)
        {
            InitializeComponent();
            txtPuntuacion.Text = puntuacion.ToString();
            txtRango.Text = calcularRango(puntuacion);
        }

        private string calcularRango(int puntuacion)
        {
            return puntuacion switch
            {
                <= 5 => " Agricultor Novel",
                <= 10 => " Agricultor Profesional",
                <= 20 => " Agricultor Emprendedor",
                _ => " Agricultor Legendario"
            };
        }

        private void btnVolverAlInicio_Click(object sender, RoutedEventArgs e)
        {
            onVolverAlInicio?.Invoke();
        }
    }
}
