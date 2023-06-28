using Refit;
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
using System.Windows.Shapes;
using PainelPress.Classes;
using PainelPress.Model;

namespace PainelPress
{
    /// <summary>
    /// Lógica interna para WinTaxanomy.xaml
    /// </summary>
    public partial class WinTaxanomy : Window
    {
        int Referencia = 0;
        List<Taxonomy> listaTags = new List<Taxonomy>();
        List<Taxonomy> listaTax1 = new List<Taxonomy>();
        List<Taxonomy> listaTax2 = new List<Taxonomy>();
        BaseDados baseDados = new BaseDados();
        InterfaceAPI terms = RestService.For<InterfaceAPI>(Constants.SITE);
        public WinTaxanomy()
        {
            InitializeComponent();
        }

        public WinTaxanomy(int referencia)
        {
            InitializeComponent();
            this.Referencia = referencia;
            
        }

        
        private void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            loadingItens.Visibility = Visibility.Visible;
            tbItemCarregados.Text = "";
            btSalvar.IsEnabled = false;
            var taxs = Constants.taxonomy;
            if (radTags.IsChecked ==true)
            {
                BuscarOline(taxs[0]);
            }
            else if (rad1.IsChecked == true)
            {

                BuscarOline(taxs[1]);

            }
            else if (rad2.IsChecked == true)
            {
                BuscarOline(taxs[2]);
            }
        }

        private async void BuscarOline(string param)
        {
            loadingItens.Visibility = Visibility.Visible;
            tbItemCarregados.Text = $"Carregando {param}...";
            var list = await terms.getTaxonomies(param);
            SalvarTax(param, list);
        }

        private async void SalvarTax(string tipo, List<Taxonomy> lista)
        {
          
            tbItemCarregados.Text =  $"Salvando {lista.Count} {tipo}";
            await baseDados.SalvarTaxonomy(tipo, lista);
            loadingItens.Visibility = Visibility.Collapsed;
            tbItemCarregados.Text = lista.Count + " itens salvos";
            btSalvar.IsEnabled = true;

        }


        private void btClearDB_Click(object sender, RoutedEventArgs e)
        {
            if (baseDados.ClearTabela(Referencia))
            {
                tbItemCarregados.Text = "Tabela limpa";
            }
        }
    }
}
