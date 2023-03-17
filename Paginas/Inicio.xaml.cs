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
                for (int i = 0; i < resultado.Count; i++)
                {
                   // resultado[i].DateView = resultado[i].Date.ToString("dd/MM/yyyy HH:mm:ss");
                }
                listPosts.ItemsSource = resultado;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            progressoBar.Visibility = Visibility.Collapsed;
            listPosts.Visibility = Visibility.Visible;

            try
            {
                if (config.Default.categorias == "")
                {
                    var catRequest = RestService.For<InterfaceAPI>(Constants.SITE);
                    var cats = await catRequest.getCategorias();
                    config.Default.Upgrade();
                    config.Default.categorias = cats;
                    config.Default.Save();
                }
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
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
            MainWindow.BT_INICIO.IsEnabled = true;
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

                var resultado = await posts.Delete(id);
                if (resultado != null && resultado.Id > 0)
                {
  
                    mensagem.HomeMensagem(true, "Post Apagado");
                    Setap();
                }
                else
                {
                    mensagem.HomeMensagem(false, "Post Não Apagado");
                }
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
    }
}
