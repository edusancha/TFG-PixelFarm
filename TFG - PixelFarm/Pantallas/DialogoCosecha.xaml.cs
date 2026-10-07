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
    /// Lógica de interacción para DialogoCosecha.xaml
    /// </summary>
    public partial class DialogoCosecha : Window
    {
        public Dictionary<Tipo, int> verdurasGuardadas { get; private set; }

        private int _camposVacios;
        private int _lechugaDisponible;
        private int _tomateDisponible;
        private int _maizDisponible;

        private int _lechuga = 0;
        private int _tomate = 0;
        private int _maiz = 0;

        public DialogoCosecha(int camposVacios, int lechuga, int tomate, int maiz)
        {
            InitializeComponent();
            verdurasGuardadas = new Dictionary<Tipo, int>
                    {
                        { Tipo.Lechuga, 0 }, { Tipo.Tomate, 0 }, { Tipo.Maiz, 0 }
                    };


            _camposVacios = camposVacios;
            _lechugaDisponible = lechuga;
            _tomateDisponible = tomate;
            _maizDisponible = maiz;

            txtCampos.Text = $"Campos arados vacíos: {camposVacios} | " +
                             $"{lechuga}lechuga(s) {tomate}tomate(s) {maiz}maiz";
        }

        private int totalGuardado => _lechuga + _tomate + _maiz;

        private void actualizarTextos()
        {
            txtLechuga.Text = _lechuga.ToString();
            txtTomate.Text = _tomate.ToString();
            txtMaiz.Text = _maiz.ToString();
        }

        private void btnLechugaMas_Click(object sender, RoutedEventArgs e)
        {
            if (totalGuardado < _camposVacios && _lechuga < _lechugaDisponible)
            { _lechuga++; actualizarTextos(); }
        }

        private void btnLechugaMenos_Click(object sender, RoutedEventArgs e)
        {
            if (_lechuga > 0) { _lechuga--; actualizarTextos(); }
        }

        private void btnTomateMas_Click(object sender, RoutedEventArgs e)
        {
            if (totalGuardado < _camposVacios && _tomate < _tomateDisponible)
            { _tomate++; actualizarTextos(); }
        }

        private void btnTomateMenos_Click(object sender, RoutedEventArgs e)
        {
            if (_tomate > 0) { _tomate--; actualizarTextos(); }
        }

        private void btnMaizMas_Click(object sender, RoutedEventArgs e)
        {
            if (totalGuardado < _camposVacios && _maiz < _maizDisponible)
            { _maiz++; actualizarTextos(); }
        }

        private void btnMaizMenos_Click(object sender, RoutedEventArgs e)
        {
            if (_maiz > 0) { _maiz--; actualizarTextos(); }
        }

        private void btnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            verdurasGuardadas = new Dictionary<Tipo, int>
        {
            { Tipo.Lechuga, _lechuga },
            { Tipo.Tomate,  _tomate  },
            { Tipo.Maiz,    _maiz    }
        };
            DialogResult = true;
        }

        private void btnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
