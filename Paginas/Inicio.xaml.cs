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
using System.Windows.Navigation;
using System.Windows.Shapes;
using PainelPress.Classes;
using config = PainelPress.Properties.Settings;
using PainelPress.Model;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System.Linq;

namespace PainelPress.Paginas
{
    /// <summary>
    /// Interação lógica para Inicio.xam
    /// </summary>
    public partial class Inicio : ContentControl
    {
        bool ShowBusca;
        PostSimples selecaoPost;
        InterfaceAPI apiRest;
        MensagemToast mensagem = new MensagemToast();
        public Inicio()
        {
            InitializeComponent();
            Setap();
        }

        private async void Setap()
        {
            apiRest = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(config.Default.Token)
            });
            try
            {
      
                var resultado = await apiRest.GetPosts();
                listPosts.ItemsSource = resultado;
                startApp();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            progressoBar.Visibility = Visibility.Collapsed;
            listPosts.Visibility = Visibility.Visible;

        }

        private async void startApp()
        {
            if (config.Default.usuarios == "")
            {
                var usuarios = await apiRest.getUsuarios();
                List<Usuario> lista = new List<Usuario>();
                foreach (var usario in usuarios)
                {
                    lista.Add(new Usuario()
                    {
                        Id = usario.id,
                        Nome = usario.nome
                    });

                }
                config.Default.Upgrade();
                config.Default.usuarios = JsonConvert.SerializeObject(lista);
                config.Default.Save();
            }

            if (config.Default.taxonomys == "")
            {
                var taxs = await apiRest.getCustomTaxonomies();
                if (taxs.Count > 0)
                {
                    config.Default.Upgrade();
                    config.Default.taxonomys = JsonConvert.SerializeObject(taxs);
                    config.Default.Save();
                }              
            }

            if (config.Default.campos == "")
            {
                var campos = await apiRest.getCampos();
              
                if (campos.Count > 0)
                {
                    List<string> list = campos.Select(s => s.nome).ToList();
                    config.Default.Upgrade();
                    config.Default.campos = JsonConvert.SerializeObject(list);
                    config.Default.Save();
                }
            }

            if (Constants.SITEMAP == "")
            {
                Constants.SITEMAP = Constants.SITE+"/sitemap.xml";
            }
        }

        private async void BuscarPosts(string palavra)
        {
           
            var resultado = await apiRest.GetPosts(palavra);
            for (int i = 0; i < resultado.Count; i++)
            {
                //resultado[i].DateView = resultado[i].Date.ToString("dd/MM/yyyy HH:mm:ss");
            }
            listPosts.ItemsSource = resultado;
            progressoBar.Visibility = Visibility.Collapsed;
            listPosts.Visibility = Visibility.Visible;

        }

        private void btBusca_Click(object sender, RoutedEventArgs e)
        {
          
            if (ShowBusca)
            {
                stackBusca.Width = 30;
                ShowBusca = false;
            }
            else
            {
                stackBusca.Width = 200;
                ediBusca.Focus();
                ShowBusca = true;

            }
        }

        private void ediBusca_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !string.IsNullOrEmpty(ediBusca.Text)) {
                progressoBar.Visibility = Visibility.Visible;
                listPosts.Visibility = Visibility.Collapsed;
                BuscarPosts(ediBusca.Text);
            }
        }

       

        private void mVisualizar_Click(object sender, RoutedEventArgs e)
        {
            WNavegador navegador = new WNavegador(selecaoPost.Link, true);
            navegador.ShowDialog();
        }

        private void mEditar_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.framePrincipal.Content = new Postar(selecaoPost);
           /// MainWindow.BT_INICIO.IsEnabled = true;
            //MainWindow.BT_POSTAR.IsEnabled = false;
        }

        private void mApagar_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Tem certeza que deseja apagar o post '" + selecaoPost.Title+"'", "Apagar post", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                progressoBar.Visibility = Visibility.Visible;
                listPosts.Visibility = Visibility.Collapsed;
                DeletePost(selecaoPost.Id);

            }
        }

        private async void DeletePost(int id)
        {
            try { 
            var posts = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(config.Default.Token)
            });
                var request = await posts.Delete(id);
                Post resultado = JsonConvert.DeserializeObject<Post>(request);
                if (resultado != null && resultado.id > 0)
                {
  
                    mensagem.HomeMensagem(true, "Post Apagado");
                    Setap();
                }
                else
                {
                    mensagem.HomeMensagem(false, "Post Não Apagado");
                }
               
            }
            catch (ApiException ex)
            {
                new MensagemToast().HomeMensagem(false, ex.Message);
                Debug.WriteLine(ex.Message.ToString());
            }
            catch (Exception ex)
            {
                new MensagemToast().HomeMensagem(false, ex.Message);
                Debug.WriteLine(ex.Message.ToString());
            }
        }

        private void listPosts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listPosts.SelectedItem != null)
            {
                selecaoPost = listPosts.SelectedItem as PostSimples;
                Debug.WriteLine(selecaoPost.Id);
            }
        }

        private void ediBusca_LostFocus(object sender, RoutedEventArgs e)
        {
            stackBusca.Width = 30;
            ShowBusca = false;
        }

        private void btConfig_Click(object sender, RoutedEventArgs e)
        {
            new WinContainer(1).Show();
        }
    }
}
