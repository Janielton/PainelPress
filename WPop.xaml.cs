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
using System.Windows.Shapes;
using PainelPress.Classes;
using config = PainelPress.Properties.Settings;
using PainelPress.Model;

namespace PainelPress
{
    /// <summary>
    /// Lógica interna para WPop.xaml
    /// </summary>
    public partial class WPop : Window
    {
        int Referencia;
        MensagemToast mensagem = new MensagemToast();
        string ultimaBusca = "";
        InterfaceAPI api;
        public WPop()
        {
            InitializeComponent();
             api = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(config.Default.Token)
            });
        }

        public WPop(int referencia)
        {
            InitializeComponent();
            this.Referencia = referencia;
            if (Referencia == 1)
            {
                janelaEdital.Visibility = Visibility.Visible;
                ediEditalSlug.Focus();
            }
            else if (Referencia == 2)
            {
                janelaUrl.Visibility = Visibility.Visible;
                ediUrlExterno.Focus();
            }
            else if (Referencia == 3)
            {
                this.Height = 200;
                janelaCache.Visibility = Visibility.Visible;
                ediCacheParam.Focus();
            }
            else if (Referencia == 4)
            {
                this.Height = 350;
                this.Width = 700;
                janelaBusca.Visibility = Visibility.Visible;
                ediBusca.Focus();
            }
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Janela_MouseMove(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void btGerarEdital_Click(object sender, RoutedEventArgs e)
        {
            if (ediEditalSlug.Text.Contains(" "))
            {
                ediEditalSlug.Text = Ferramentas.ToUrlSlug(ediEditalSlug.Text);
            }
            if (!string.IsNullOrEmpty(ediEditalUrl.Text)) btAcaoEdital();
        }

        private void btAcaoEdital()
        {
            try
            {
                if (radEditalPadrao.IsChecked == true || radEditalView.IsChecked == true)
                {
                    btGerarEdital.Content = "Gerando edital..";
                    getUrlEdital();
                }
                else
                {
                    string urlView = string.Format("https://www.confiraconcursos.com.br/edital/ver/{0}&drive={1}&view=3", Ferramentas.ToUrlSlug(ediEditalSlug.Text), ediEditalUrl.Text);
                    ediEditalView.Text = urlView;
                    Clipboard.SetText(urlView);
                    mensagem.HomeMensagem(true, "Link gerado e copiado!");
                    DialogResult = true;
                    this.Close();
                }
            }
            catch { }
        }

        private string UrlDrive()
        {
            if (ediEditalUrl.Text.Length < 40)
            {
                return ediEditalUrl.Text;
            }
            return ediEditalUrl.Text.Replace("https://drive.google.com/file/d/", "").Replace("/view", "").Replace("?usp=sharing", "");
        }

        private async void getUrlEdital()
        {
            var posts = RestService.For<InterfaceAPI>("https://www.confiraconcursos.com.br");
            UrlEdital urlEdital = new UrlEdital() { nonce = "8d0ea56e",
                url = ediEditalUrl.Text, keyword = ediEditalSlug.Text, action = "add"
            };
            try
            {
                var resultado = await posts.GerarEdital(urlEdital);

                if (resultado.Status.Equals("success"))
                {

                    mensagem.HomeMensagem(true, "Edital cadastrado!");
                    string tipo = radEditalPadrao.IsChecked == true ? "1" : "2";
                    string urlView = string.Format("{0}&view={1}", resultado.Shorturl, tipo);
                    ediEditalView.Text = urlView;
                    Clipboard.SetText(urlView);
                    await Task.Delay(150);
                    DialogResult = true;
                    this.Close();
                }
                else
                {
                    mensagem.HomeMensagem(false, resultado.Message);
                    Debug.WriteLine(resultado.StatusCode + " => " + resultado.Message);
                }

            }
            catch(Exception ex)
            {
                mensagem.HomeMensagem(false, ex.Message);
            }
            

        }

        private void btGerarUrl_Click(object sender, RoutedEventArgs e)
        {

            try
            {
                string tipo = radUrlOrganizadora.IsChecked == true ? "1" : radUrlSite.IsChecked == true ? "2" : "4";
                string url = string.Format("https://www.confiraconcursos.com.br/link-externo/?id={0}&tipo={1}", Conversor64.EncodeToBase64(ediUrlExterno.Text), tipo);

                Clipboard.SetText(url);
                ediUrlResult.Text = url;
                DialogResult = true;
                this.Close();
            }
            catch { }

        }

        private void radEditalDrive_Checked(object sender, RoutedEventArgs e)
        {
            //  ediEditalDrive.Visibility = Visibility.Visible;
        }

        private void radEditalDrive_Unchecked(object sender, RoutedEventArgs e)
        {
            // ediEditalDrive.Visibility = Visibility.Collapsed;
        }

        private void ediEditalUrl_LostFocus(object sender, RoutedEventArgs e)
        {
            if (radEditalDrive.IsChecked == true)
            {
                ediEditalUrl.Text = UrlDrive();
            }
        }

        private void ediEditalSlug_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (ediEditalSlug.Text.Contains(" "))
                {
                    ediEditalSlug.Text = Ferramentas.ToUrlSlug(ediEditalSlug.Text);
                }
                if (!string.IsNullOrEmpty(ediEditalUrl.Text)) btAcaoEdital();
            }
        }

        #region CACHER
        private void btClearCache_Click(object sender, RoutedEventArgs e)
        {

            if (radCacheCidade.IsChecked == true)
            {
                ClearCacheCidade();
            }
            else if (radCacheServer.IsChecked == true) {
                ClearCacheServer();
            }
            else
            {
                ClearCachePost();
            }
        }

        private async void ClearCacheCidade()
        {
            try
            {
                string param = ediCacheParam.Text.Replace($"{Constants.SITE}/cidade/", "");
                //var rest = RestService.For<InterfaceAPI>(Constants.SITE);
                //var resultado = await rest.ClearChache(param);
                //if (resultado.Sucesso)
                //{
                //    mensagem.HomeMensagem(true, "Cidade limpa");
                //}
                //else
                //{
                //    mensagem.HomeMensagem(false, "Erro ao limpar cidade");
                //}
            }
            catch { mensagem.HomeMensagem(false, "Erro ao limpar cidade"); }
        }
        private async void ClearCacheServer()
        {
            try {
               
            }
            catch { mensagem.HomeMensagem(false, "Erro ao limpar servidor"); }
        }

        private async void ClearCachePost()
        {
            try {
                string s = ediCacheParam.Text;
                string param = s.Replace("https://www.confiraconcursos.com.br/", "");

                var rest = RestService.For<InterfaceAPI>(Constants.SITE);
                //var resultado = await rest.ClearChachePost(param);
                //if (resultado.Sucesso)
                //{
                //    mensagem.HomeMensagem(true, "Cacher post limpo");
                //}
                //else
                //{
                //    mensagem.HomeMensagem(false, "Erro ao limpar post");
                //}
            }
            catch { mensagem.HomeMensagem(false, "Erro ao limpar post - catch"); }
        }

        #endregion

        #region Busca concurso
        private void ediBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ediBusca.Text.Length > 2)
            {
                BuscarPosts();
            }
            else {
                listPost.ItemsSource = null;
            }
        }


        private async void BuscarPosts()
        {
            await Task.Delay(500);
            if (ultimaBusca.Equals(ediBusca.Text)) return;
           
            ultimaBusca = ediBusca.Text;
            var resultado = await api.GetPosts(ultimaBusca);
            if (resultado.Count==0) {
                resultado.Add(new PostSimples()
                {
                  Id= 0, Title = "Nenhum post encontrado", Link = "", Date = DateTime.Now
                });
            }
            listPost.ItemsSource = resultado;
        }

        #endregion

        private void listPost_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listPost.SelectedItem != null) {
                var post = listPost.SelectedItem as Post;
                if (Convert.ToInt32(post.Id) == 0) return;
                WNavegador navegador = new WNavegador(post.Link, false);
                navegador.Show();
            }
        }
    }
}
