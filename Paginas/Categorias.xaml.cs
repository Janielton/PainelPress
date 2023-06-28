using FontAwesome.WPF;
using FtpLibrary;
using Newtonsoft.Json;
using PainelPress.Classes;
using PainelPress.Elementos;
using PainelPress.Model;
using Refit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using config = PainelPress.Properties.Settings;

namespace PainelPress.Paginas
{
    /// <summary>
    /// Interação lógica para Categorias.xam
    /// </summary>
    public partial class Categorias : ContentControl
    {
        CategoriaFull selecao;
        InterfaceAPI api;
        bool start = false;
        List<CategoriaFull> listaAll = new List<CategoriaFull>();
        string parent = "";
        bool edicao = false;
        AlertMensagem alert;
        public Categorias()
        {
            InitializeComponent();
            alert = AlertMensagem.instance;
            api = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(config.Default.Token)
            });
            start = true;
            getCats();
        }

        private async void getCats(string key = "")
        {
            if (key == "")
            {
                listaAll = await api.ListaCats();
                listItens.ItemsSource = listaAll;
                bdSubs.Visibility = Visibility.Collapsed;
                SetCategorias(listaAll);
            }
            else
            {
                listItens.ItemsSource = buscaCats(key);
                bdSubs.Visibility = Visibility.Collapsed;
            }
            progresso.Visibility = Visibility.Collapsed;
            selecao = null;

        }

        private void SetCategorias(List<CategoriaFull> list) {
            string cats = JsonConvert.SerializeObject(list);
            config.Default.Upgrade();
            config.Default.categorias = cats;
            config.Default.Save();
        }


        private List<CategoriaFull> buscaCats(string key)
        {
            List<CategoriaFull> lista = new List<CategoriaFull>();
            List<CategoriaFull> newlistaAll = new List<CategoriaFull>();

            foreach(CategoriaFull c in listaAll)
            {
                newlistaAll.Add(c);
                if (c.subs.Count > 0)
                {
                    foreach (SubCat s in c.subs) newlistaAll.Add(s.subsToFull);
                }
            }
            try
            {
                var lists = newlistaAll.Where(p => p.name.ToLower().Contains(key.ToLower())).ToList();
                lista = lists;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return lista;
        }

        private async void btDelete_Click(object sender, RoutedEventArgs e)
        {
            if (selecao == null) return;
            try
            {
                linearBotoes.IsEnabled = false;
                RequisicaoBol request = await api.DeleteCat(selecao.term_id);
                if (request.Sucesso)
                {
                    getCats();
                }
                else
                {
                    MessageBox.Show(request.Mensagem, "Erro");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro Delete");
            }
        }

     
        private void clearForm()
        {
            selecao = null;
            ediNome.Texto = "";
            ediDescri.Texto = "";
            parent = "";
            edicao = false;
            bdSubs.Visibility = Visibility.Collapsed;
        }


        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            bordEdicao.Visibility = Visibility.Collapsed;
            clearForm();
        }


        private void listItens_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listItens.SelectedItem != null)
            {
                bdSubs.Visibility = Visibility.Collapsed;
                selecao = listItens.SelectedItem as CategoriaFull;
                linearBotoes.IsEnabled = true;
                if (selecao.subs.Count > 0)
                {
                    bdSubs.Visibility = Visibility.Visible;
                    listItensSubs.ItemsSource = selecao.subs;
                }
              
            }
        }

        private void editNomeCat_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (start && editNomeCat.Texto.Length > 0)
            {
                getCats(editNomeCat.Texto);
            }
        }


        private void btCloseSubs_Click(object sender, RoutedEventArgs e)
        {
            if (bdSubs.IsVisible)
            {
                bdSubs.Visibility = Visibility.Collapsed;
            }
            else
            {
                bdSubs.Visibility = Visibility.Visible;
            }
        }

        private void listItensSubs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listItensSubs.SelectedItem != null)
            {
                var sub = listItensSubs.SelectedItem as SubCat;
                selecao = sub.subsToFull;

                linearBotoes.IsEnabled = true;
            }
        }

        #region ADD/EDIT
        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            if (bordEdicao.IsVisible)
            {
                bordEdicao.Visibility = Visibility.Collapsed;
                btAdd.Content = new ImageAwesome()
                {
                    Icon = FontAwesomeIcon.PlusCircle,
                    Foreground = CorImage.GetCor("#FF7F7B7B"),
                    Width = 27
                };
            }
            else
            {
                bordEdicao.Visibility = Visibility.Visible;
                selectCats.ItemsSource = listaAll;
                bdSubs.Visibility = Visibility.Collapsed;
                btAdd.Content = new ImageAwesome()
                {
                    Icon = FontAwesomeIcon.Close,
                    Foreground = CorImage.GetCor("#FF7F7B7B"),
                    Width = 25
                };
            }
        }

        private void selectCats_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectCats.SelectedItem != null)
            {
                var cat = selectCats.SelectedItem as CategoriaFull;
                parent = cat.term_id;
            }
        }

        private async void btSalvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dados = new Dictionary<string, object> {
                    {"nome", ediNome.Texto},
                    {"description", ediDescri.Texto},
                };
                if (parent != "") dados.Add("parent", parent);
                RequisicaoBol requisicao;
                if (!edicao)
                {
                    dados.Add("slug", Ferramentas.ToUrlSlug(ediNome.Texto));
                    requisicao = await api.AdicionarCat(dados);
                }
                else
                {
                    if (selecao == null) return;
                    if (selecao.name == ediNome.Texto.Trim())
                    {
                        dados.Remove("nome");
                    }
                    if (selecao.description == ediDescri.Texto.Trim())
                    {
                        dados.Remove("description");
                    }
                    requisicao = await api.EditarCat(selecao.term_id, dados);
                }

                if (requisicao.Sucesso)
                {
                    clearForm();
                    bordEdicao.Visibility = Visibility.Collapsed;
                    getCats();
                }
                else
                {
                    alert.Show(requisicao.Mensagem, "Erro ao salvar categoria");
                }
            }
            catch(Exception ex)
            {
                alert.Show(ex.Message, "Erro ao salvar categoria");
            }

        }

        private void btEditar_Click(object sender, RoutedEventArgs e)
        {
            if (selecao == null) return;
            linearBotoes.IsEnabled = false;
            ediNome.Texto = selecao.name;
            ediDescri.Texto = selecao.description;
            //ediSite.Text = selecao.site;
            bordEdicao.Visibility = Visibility.Visible;
            bdSubs.Visibility = Visibility.Collapsed;
            edicao = true;

        }
        #endregion
    }
}
