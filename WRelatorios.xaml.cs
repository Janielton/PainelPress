using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PainelPress.Classes;
using PainelPress.Model;
using PainelPress.ViewModel;
using config = PainelPress.Properties.Settings;
using Microsoft.Win32;

namespace PainelPress
{
    /// <summary>
    /// Lógica interna para WRelatorios.xaml
    /// </summary>
    public partial class WRelatorios : Window
    {
        Button tabAtual;
        public WRelatorios()
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(config.Default.pathJson)
                 || string.IsNullOrEmpty(config.Default.propriedade)
                 || !File.Exists(config.Default.pathJson))
            {
                btTab.IsEnabled = false;
                btTab2.IsEnabled = false;
                btTab3.IsEnabled = false;
                borderConfig.Visibility = Visibility.Visible;
                return;
            }
            pageContainer.Source = getPage("PaginasVistas");
            tabAtual = btTab;
            tabAtual.IsEnabled = false;
            editPath.Text = config.Default.pathJson;
            editPropriedade.Text = config.Default.propriedade;
        }

        private void btTab_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (tabAtual != null) tabAtual.IsEnabled = true;
            if (button != null)
            {
                if (button.Tag.ToString() == "1")
                {
                    pageContainer.Source = getPage("PaginasVistas");
                }
                else if (button.Tag.ToString() == "2")
                {
                    pageContainer.Source = getPage("Visitas");
                }
                else if (button.Tag.ToString() == "3")
                {
                    pageContainer.Source = getPage("Origem");
                }
                button.IsEnabled = false;
                tabAtual = button;
            }
        }

        private Uri getPage(string nome)
        {
            return new Uri($"View/{nome}.xaml",
            UriKind.RelativeOrAbsolute);
        }

        private void btSelecina_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Aquivo Json |*.json";
            if (openFileDialog.ShowDialog() == true)
            {
                //Get the path of specified file
                editPath.Text = openFileDialog.FileName;
                config.Default.Upgrade();
                config.Default.pathJson = editPath.Text;
                config.Default.Save();

            }
        }

        private void btConfig_Click(object sender, RoutedEventArgs e)
        {
            if (borderConfig.IsVisible)
            {
                borderConfig.Visibility = Visibility.Collapsed;
                if (string.IsNullOrEmpty(editPath.Text) || string.IsNullOrEmpty(editPropriedade.Text)) return;

                if (File.Exists(config.Default.pathJson))
                {
                    config.Default.Upgrade();
                    config.Default.propriedade = editPropriedade.Text;
                    config.Default.Save();
                    btTab2.IsEnabled = true;
                    btTab3.IsEnabled = true;
                    pageContainer.Source = getPage("PaginasVistas");
                    tabAtual = btTab;
                }
            }
            else
            {
                borderConfig.Visibility = Visibility.Visible;
            }
        }
    }
}