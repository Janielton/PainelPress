using Newtonsoft.Json;
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
using Tweetinvi;
using Tweetinvi.Exceptions;
using PainelPress.Classes;
using PainelPress.Model;
using PainelPress.Paginas;
using FtpLibrary;
using config = PainelPress.Properties.Settings;
namespace PainelPress
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ContentControl framePrincipal;
        public static Button BT_INICIO;
        public static Button BT_POSTAR;
        public static Label lbMensagem;
        public static Border brMensagem;

        MensagemToast mensagem = new MensagemToast();

        public MainWindow()
        {
            InitializeComponent();
        
            if (!config.Default.Logado)
            {
                Login login = new Login(1);
                login.Topmost = true;
                this.Hide();
                if (login.ShowDialog() == true)
                {
                    Setap();
                    this.Show();
                } 
            }
            else
            {
                IsValidToken();
                Setap();

            }


        }

        private void Setap()
        {
            contentPagina.Content = new Inicio();
            framePrincipal = contentPagina;
            BT_INICIO = btInicio;
            BT_POSTAR = btPostar;
            lbMensagem = labelMensagem;
            brMensagem = borderMensagem;
            textUser.Text = config.Default.Usuario;
        }
        private async void IsValidToken()
        {
            var retri = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(config.Default.Token)
            });

            try
            {
                var resultado = await retri.VerificaToken();

                if (resultado.Data.Status != 200)
                {
                    this.Hide();
                    Login login = new Login(1);
                    login.ShowDialog();
                    this.Show();
                }
                //IsValidTokenTwitter();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                this.Hide();
                Login login = new Login(1);
                login.ShowDialog();
                this.Show();
            }

        }

        #region TWITTER
        private async void IsValidTokenTwitter()
        {
            try
            {
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };

                var uc = JsonConvert.DeserializeObject<Credencial>(config.Default.credenciais, settings);

                var appClient = new TwitterClient(uc.ConsumerKey, uc.ConsumerSecret, uc.AccessToken, uc.AccessTokenSecret);
                var user = await appClient.Users.GetAuthenticatedUserAsync();
                Debug.WriteLine(user.Name);
                if (string.IsNullOrEmpty(user.Name))
                {
                    Auth_Twitter();
                }

            }
            catch (TwitterException ex)
            {
                Debug.WriteLine($"Erro ao autenticar 1: {ex.Message} - {ex.Content}");
                Auth_Twitter();
            }
            catch (Exception ex)
            {
                Auth_Twitter();
                Debug.WriteLine($"Erro ao autenticar 2: {ex.Message}");
            }

        }

        private async void Auth_Twitter()
        {
            var quest = MessageBox.Show("Altenticar agora?", "Twitter não altenticado", MessageBoxButton.YesNo);
            if (quest == MessageBoxResult.No) return;


            var client = new TwitterClient(Constants.CONSUMER_KEY, Constants.CONSUMER_SECRET);
            var authenticationRequest = await client.Auth.RequestAuthenticationUrlAsync();

            Process.Start(new ProcessStartInfo(authenticationRequest.AuthorizationURL)
            {
                UseShellExecute = true
            });


            Login mensagemAlerta = new Login(2);
            if (mensagemAlerta.ShowDialog() == true)
            {
                var userCredentials = await client.Auth.RequestCredentialsFromVerifierCodeAsync(mensagemAlerta.Resultado, authenticationRequest);
                config.Default.Upgrade();
                config.Default.credenciais = JsonConvert.SerializeObject(userCredentials);
                config.Default.Save();
                Debug.WriteLine(config.Default.credenciais);

                // You can now save those credentials or use them as followed
                var appClient = new TwitterClient(userCredentials);
                var user = await appClient.Users.GetAuthenticatedUserAsync();
                mensagem.HomeMensagem(true, user.Name + " altenticado com sucesso");
            }

        }

        #endregion

        private void btInicio_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Postar.postConteudo))
            {
                contentPagina.Content = new Inicio();
                btInicio.IsEnabled = false;
                btPostar.IsEnabled = true;
                return;
            }

            // new WTeste().Show();

        }


        private void btPostar_Click(object sender, RoutedEventArgs e)
        {
            contentPagina.Content = new Postar();
            btInicio.IsEnabled = true;
            btPostar.IsEnabled = false;
        }

        private void btTaxonomy_Click(object sender, RoutedEventArgs e)
        {
            WinTaxanomy tax = new WinTaxanomy();
            tax.ShowDialog();
        }

 
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            CefSharp.Cef.Shutdown();
            Application.Current.Shutdown();
        }


        private void btCache_Click(object sender, RoutedEventArgs e)
        {
            WPop wPop = new WPop(3);
            wPop.ShowDialog();
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollViewer = (ScrollViewer)sender;
            if (e.Delta < 0)
            {
                scrollViewer.LineRight();
            }
            else
            {
                scrollViewer.LineLeft();
            }
            e.Handled = true;
        }

        private void btBackup_Click(object sender, RoutedEventArgs e)
        {
            WBackup backup = new WBackup();
            backup.Show();
        }

        private void btUpload_Click(object sender, RoutedEventArgs e)
        {
            WUpload upload = new WUpload();
            upload.Show();
        }
        private void btRelatorios_Click(object sender, RoutedEventArgs e)
        {
            WRelatorios relatorios = new WRelatorios();
            relatorios.Show();
        }

       
        private void btSocial_Click(object sender, RoutedEventArgs e)
        {
            WSocial social = new WSocial();
            social.Show();
        }

        private void btGerarTokenTwitter_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btSair_Click(object sender, RoutedEventArgs e)
        {
            config.Default.Upgrade();
            config.Default.Logado = false;
            config.Default.Save();
            CefSharp.Cef.Shutdown();
            Application.Current.Shutdown();
        }

        private void btFTP_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                new FTPWindow().Show();
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btConfig_Click(object sender, RoutedEventArgs e)
        {
            new WinContainer(1).Show();
        }
    }
}
