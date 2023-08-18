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
using FontAwesome.WPF;
using PainelPress.Elementos;
using System.Timers;
using System.Xml;
using System.Net.Http;
using Notification.Wpf;


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
        List<Classes.Menu> listaMenu;
        public Timer painelTimer { get; set; }
        TarefasProgramadas tarefas;

        MensagemToast mensagem = new MensagemToast();

        public MainWindow()
        {
            InitializeComponent();
            tarefas = new TarefasProgramadas(this);
            if (!config.Default.Logado)
            {
                Login login = new Login(1);
                login.Topmost = true;
                this.Hide();
                if (login.ShowDialog() == true)
                {       
                    this.Show();
                    Setap();
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
           
            BT_POSTAR = btPostar;
            lbMensagem = labelMensagem;
            brMensagem = borderMensagem;
            textUser.Text = config.Default.Usuario;
            SetMenu(Classes.Menu.ListaItems());
            StartTarefas();
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
            
            if (!string.IsNullOrEmpty(Postar.postConteudo))
            {
                AlertMensagem confirme = AlertMensagem.instance.Confirme("Tem certeza que deseja ir para a pagina inical?","Alteções serão perdidas",2);
                confirme.ShowDialog();
                if (confirme.result == "no")
                {
                    return;
                }
                
            }
            contentPagina.Content = new Inicio();
            btPostar.IsEnabled = true;
           
            // new WTeste().Show();

        }


        private void btPostar_Click(object sender, RoutedEventArgs e)
        {
            contentPagina.Content = new Postar();
           
            btPostar.IsEnabled = false;
        }

       

 
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            CefSharp.Cef.Shutdown();
            Application.Current.Shutdown();
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

      

       

        private void btCloseWin_Click(object sender, RoutedEventArgs e)
        {
           if(painelTimer!=null) painelTimer.Stop();
            CefSharp.Cef.Shutdown();
            Application.Current.Shutdown();
        }

        private void btMaximizar_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                btMaximizar.Content = new ImageAwesome()
                {
                    Icon = FontAwesomeIcon.WindowRestore,
                    Foreground = Brushes.White,
                    Width = 20
                };
            }
            else
            {
                this.WindowState = WindowState.Normal;
                btMaximizar.Content = new ImageAwesome()
                {
                    Icon = FontAwesomeIcon.WindowMaximize,
                    Foreground = Brushes.White,
                    Width = 20
                };
            }

        }

        private void btMinimizar_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


        #region MENU
        private void edit_TextChanged(object sender, TextChangedEventArgs e)
        {
          
            SetMenu(Classes.Menu.ListaBusca(buscaFunc.Texto));
        }

        private void SetMenu(List<Classes.Menu> lista)
        {
            stackScrol.Children.Clear();
            listaMenu = lista;
            if (lista.Count == 0)
            {
                stackScrol.Children.Add(new Button()
                {
                    Content = "Não encontrado",
                    Padding = new Thickness(5,4,5,6),
                    IsEnabled = false,
                    Style = Resources["ButtonStyleCentro"] as Style
                });
                return;
            }
            foreach (var item in lista)
            {
                Button button = new Button();
                button.Content = item.nome;
                button.Tag = item.tag;
              
                button.Click += btMenu_Click;
                button.Style = Resources["ButtonStyleCentro"] as Style;
                stackScrol.Children.Add(button);
            }
        }

        private void btMenu_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            if(bt != null)
            {
                string tag = bt.Tag.ToString();
                if (tag == "upload")
                {
                    btUpload_Click();
                }
                else if (tag == "analytics")
                {
                    btRelatorios_Click();
                }
                else if (tag == "ftp")
                {
                    btFTP_Click();
                }
                else if (tag == "paginas")
                {
                    new WinContainer(new PaginasSites()).Show();
                }
                else if (tag == "social")
                {
                    btSocial_Click();
                }
                else if (tag == "stories")
                {
                    btStories_Click();
                }
                else if (tag == "backup")
                {
                    btBackup_Click();
                }
                else if (tag == "taxonomies")
                {
                    btTaxonomy_Click();
                }
                else if (tag == "categorias")
                {
                    new WinContainer(new Categorias()).Show();
                }
                else if (tag == "feed")
                {
                    new WinContainer(new LeitorFeed()).Show();
                }
                else if (tag == "gpt")
                {
                    new WinContainer(new ChatPage()).Show();
                }
                else if (tag == "")
                {
                 
                }
            }
        }


        private void btTaxonomy_Click()
        {
            WinTaxanomy tax = new WinTaxanomy();
            tax.ShowDialog();
        }

        private void btCache_Click()
        {
            WPop wPop = new WPop(3);
            wPop.ShowDialog();
        }

        private void btBackup_Click()
        {
            WBackup backup = new WBackup();
            backup.Show();
        }

        private void btUpload_Click()
        {
            WUpload upload = new WUpload();
            upload.Show();
        }

        private void btRelatorios_Click()
        {
            WinContainer relatorios = new WinContainer(new Relatorios());
            relatorios.Show();
        }


        private void btSocial_Click()
        {
            Social social = new Social();
            new WinContainer(social).Show();
        }

        private void btFTP_Click()
        {
            try
            {
                new FTPWindow().Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btStories_Click()
        {
            var janela = new Stories();
            new WinContainer(janela).Show();
        }
        #endregion

        #region TAREFAS

        private void StartTarefas()
        {
            try
            { 
                int tempo = 600000;
                painelTimer = new Timer();
                painelTimer.Interval = tempo;
                painelTimer.Elapsed += Tarefas;
                painelTimer.Start();
            }catch(Exception ex) { Debug.WriteLine(ex.Message); }
        }

        private async void Tarefas(object sender, EventArgs e)
        {
            try
            {
                bool acao1 = await tarefas.getPostsFeeds();
                bool acao2 = await tarefas.getPaginasUpdate();
               // painelTimer.Stop();
          
                if (!acao1 && !acao2)
                {
                    painelTimer.Stop();
                }

            }
            catch (Exception ex) { Debug.WriteLine("Tarefas: " + ex.Message); }
        }

        #endregion

        private void buscaFunc_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if (listaMenu.Count == 1)
                {
                    btMenu_Click(new Button() { Tag = listaMenu[0].tag }, null);
                }
            }
        }
    }
}
