using Refit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
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
using Newtonsoft.Json;
using System.Net.Http;

namespace PainelPress
{
    /// <summary>
    /// Lógica interna para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private int Site = 1;
        private int Twitter = 2;
        public string Resultado = "";
        public Login()
        {
            InitializeComponent();

            ediLogin.Text = config.Default.Usuario;
            ediSenha.Password = config.Default.Senha;
            radSalvarSenha.Visibility = Visibility.Visible;
            stakConfiraConcursos.Visibility = Visibility.Visible;
        }

        public Login(int tipo)
        {
            InitializeComponent();
            if (tipo == Site)
            {
                ediLogin.Text = config.Default.Usuario;
                ediSenha.Password = config.Default.Senha;
                radSalvarSenha.Visibility = Visibility.Visible;
                stakConfiraConcursos.Visibility = Visibility.Visible;
                editSite.Text = config.Default.site;
            }
            else if (tipo == Twitter)
            {
                stakTwitter.Visibility = Visibility.Visible;
                btConfig.Visibility = Visibility.Collapsed;
            }

        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            if(stakConfiraConcursos.IsVisible) Application.Current.Shutdown();
            var acao = MessageBox.Show("Deseja encerrar a aplicação?", "Encerrar aplicação", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (acao == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
            else
            {
                Close();
            }
   
        }

        private async void FazerLogin()
        {
            try {
                if (Constants.SITE == "") return;
                User user = new User();
                user.username = ediLogin.Text.Trim();
                user.password = ediSenha.Password.Trim();
                if (string.IsNullOrEmpty(user.username) || string.IsNullOrEmpty(user.password)) return;
                var rest = RestService.For<InterfaceAPI>(Constants.SITE);
                Token resultado = await rest.Login(user);
                Debug.WriteLine(resultado.token);
                if (!string.IsNullOrEmpty(resultado.token))
                {
                    config.Default.Upgrade();
                    config.Default.Logado = true;
                    config.Default.Usuario = resultado.user_nicename;
                    if (radSalvarSenha.IsChecked == true) { config.Default.Senha = user.password; }
                    config.Default.Token = resultado.token;
                    config.Default.Save();
                    DialogResult = true;
                    this.Close();
                }
                else
                {
                    txtStatus.Text = resultado.code;
                    txtStatus.Visibility = Visibility.Collapsed;
                }
               
            } 
            catch(HttpRequestException ex)
            {
                txtStatus.Text = ex.Message;
                txtStatus.Visibility = Visibility.Visible;
            }
            catch (ApiException ex)
            {
                SetStatus(ex.Content);
                Debug.WriteLine($"Content => {ex.Content} - Message=> {ex.Message}");
            }
        }
       
        private void SetStatus(string json)
        {
            try
            {
                var objeto = JsonConvert.DeserializeObject<Token>(json);
                if (string.IsNullOrEmpty(objeto.code)) return;
                if (objeto.code.Contains("incorrect_password"))
                {
                    txtStatus.Text = "Senha inválida";
                }else if (objeto.code.Contains("invalid_email"))
                {
                    txtStatus.Text = "Email inválido";
                }
                txtStatus.Visibility = Visibility.Visible;
            }
            catch { }
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

        private void btLogin_Click(object sender, RoutedEventArgs e)
        {
            FazerLogin();
        }

        private void btTWEnviar_Click(object sender, RoutedEventArgs e)
        {
            Resultado = ediTWCodigo.Text;
            DialogResult = true;
            this.Close();

        }

        private void btConfig_Click(object sender, RoutedEventArgs e)
        {
            if (stackSite.IsVisible)
            {
                stackSite.Visibility = Visibility.Collapsed;
            }
            else
            {
                stackSite.Visibility = Visibility.Visible;
            }
        }

        private void editSite_LostFocus(object sender, RoutedEventArgs e)
        {
            if (editSite.Text.Length > 5)
            {
                config.Default.Upgrade();
                config.Default.site = editSite.Text;
                config.Default.Save();
            }

        }
    }
}
