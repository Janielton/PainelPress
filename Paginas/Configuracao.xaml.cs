﻿using Refit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PainelPress.Classes;
using PainelPress.Model;
using PainelPress.ViewModel;
using System.Text.RegularExpressions;

namespace PainelPress.Paginas
{
    /// <summary>
    /// Interação lógica para Configuracao.xam
    /// </summary>
    public partial class Configuracao : ContentControl
    {

    
        public Configuracao()
        {
            DataContext = new ConfigViewModel();
            InitializeComponent();
        }

        private void ApenasNumero(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

 
    }
}
