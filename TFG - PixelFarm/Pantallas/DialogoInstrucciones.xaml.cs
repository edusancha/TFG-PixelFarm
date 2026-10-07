using System.Windows;

namespace TFG___PixelFarm.Pantallas
{
    public partial class DialogoInstrucciones : Window
    {
        public DialogoInstrucciones()
        {
            InitializeComponent();
        }

        private void btnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}