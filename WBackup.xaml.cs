using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Ookii.Dialogs.Wpf;
using PainelPress.Classes;
using config = PainelPress.Properties.Settings;

namespace PainelPress
{
    /// <summary>
    /// Lógica interna para WBackup.xaml
    /// </summary>
    public partial class WBackup : Window
    {
        public WBackup()
        {
            InitializeComponent();
            if (config.Default.pathdb=="")
            {
                btsiteprincipal.IsEnabled = false;
            }
            else
            {
                if (!Directory.Exists(config.Default.pathdb))
                {
                    btsiteprincipal.IsEnabled = false;
                    return;
                }
                editpath.Text = config.Default.pathdb;
            }
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btsiteprincipal_Click(object sender, RoutedEventArgs e)
        {
            progresso.Visibility = Visibility.Visible;
            stackBotoes.IsEnabled = false;
            _ = salvarDancoByUrl($"{Constants.SITE}/api?rota=backup");
            if (!RemoveAntigos()) {
                System.Windows.MessageBox.Show("Erro apagar dados antigos");
            }
            // _ = salvarDancoNew(1);
        }

        private void btquestoes_Click(object sender, RoutedEventArgs e)
        {
            progresso.Visibility = Visibility.Visible;
            stackBotoes.IsEnabled = false;
            _ = salvarDancoNew(2);
        }

        private void bturl_Click(object sender, RoutedEventArgs e)
        {
            progresso.Visibility = Visibility.Visible;
            stackBotoes.IsEnabled = false;
            _ = salvarDancoNew(3);
        }

        public async void salvarDanco(string url, int tipo)
        {
            try
            {
               // Debug.WriteLine(Conversor64.DecodeFrom64("Y29uZmlyYWNvbmN1cnNvczpbbCEyQ1FTZkhxaVg="));
            string nome = tipo == 1 ? "siteprincipal.sql.gz": tipo == 2 ? "questoes.sql.gz": "confiraurl.sql.gz";
            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.Authorization, "Basic Y29uZmlyYWNvbmN1cnNvczpbbCEyQ1FTZkhxaVg=");
            string path = string.Format(@"C:\Users\Elton\Downloads\BackupDB\Painel\{0}", nome);
            client.DownloadProgressChanged += (s, e) => { progresso.Value = e.ProgressPercentage; };
            await client.DownloadFileTaskAsync(new Uri(url), path);
            progresso.Visibility = Visibility.Hidden;
            stackBotoes.IsEnabled = true;
            }
            catch (Exception ex) {
                System.Windows.MessageBox.Show("salvarDanco error : " + ex.Message);
            }
        }
        
        public async Task<bool> salvarDancoNew(int tipo)
        {
            try
            {
                string nome = tipo == 1 ? "siteprincipal.sql.gz" : tipo == 2 ? "questoes.sql.gz" : "confiraurl.sql.gz";
                string path = string.Format(@"C:\WPanel\BancosBackup\{0}", nome);
                string url = "https://www.confiraconcursos.com.br/banco-de-dados/" + nome;
                Debug.WriteLine(url);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.File.DownloadFile;
                request.Headers.Add(HttpRequestHeader.Authorization, "Basic Y29uZmlyYWNvbmN1cnNvczpbbCEyQ1FTZkhxaVg=");
                NetworkCredential networkCredential = new NetworkCredential("confiraconcursos", "[l!2CQSfHqiX");
                CredentialCache myCredentialCache = new CredentialCache { { new Uri(url), "Basic", networkCredential } };
                request.PreAuthenticate = true;
               if(tipo ==3) request.Timeout = 3000;
           
                //  request.Credentials = myCredentialCache;
                using (WebResponse response = await request.GetResponseAsync())
                {

                    FileStream fileStream = new FileStream(path, FileMode.Create);
                    using (Stream input = response.GetResponseStream())
                    {
                       await input.CopyToAsync(fileStream);
                    }
                    fileStream.Flush();
                    fileStream.Close();
                    progresso.Visibility = Visibility.Hidden;
                    stackBotoes.IsEnabled = true;
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("salvarDancoNew error : " + ex.Message);
                progresso.Visibility = Visibility.Hidden;
                stackBotoes.IsEnabled = true;
                return false;
            }
        }

        public async Task<bool> salvarDancoByUrl(string url)
        {
            try
            {
               
                string path = $"{config.Default.pathdb}\\db_{DateTime.Now.ToString("dd-MM")}.sql";
    
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = WebRequestMethods.File.DownloadFile;
                request.Headers.Add(HttpRequestHeader.Authorization, "Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=");
                request.PreAuthenticate = true;
                request.Timeout = 5000;

                //  request.Credentials = myCredentialCache;
                using (WebResponse response = await request.GetResponseAsync())
                {

                    FileStream fileStream = new FileStream(path, FileMode.Create);
                    using (Stream input = response.GetResponseStream())
                    {
                        await input.CopyToAsync(fileStream);
                    }
                    fileStream.Flush();
                    fileStream.Close();
                    progresso.Visibility = Visibility.Hidden;
                    stackBotoes.IsEnabled = true;
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("salvarDancoByUrl error : " + ex.Message);
                progresso.Visibility = Visibility.Hidden;
                stackBotoes.IsEnabled = true;
                return false;
            }
        }

        private bool RemoveAntigos()
        {
            try
            {
                var data = DateTime.Now;
                data = data.AddMonths(-1);
                string key = data.ToString("-MM");
                DirectoryInfo d = new DirectoryInfo(config.Default.pathdb);
                foreach (var file in d.GetFiles("*.sql"))
                {
                    if (file.FullName.Contains(key))
                    {
                        file.Delete();
                    }
                }
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return true;
        }

        private async void btAll_Click(object sender, RoutedEventArgs e)
        {
            progresso.Visibility = Visibility.Visible;
            stackBotoes.IsEnabled = false;
            var task = await salvarDancoNew(1);
            if (!task) return;
            await Task.Delay(3000);
            progresso.Visibility = Visibility.Visible;
            task = await salvarDancoNew(2);
            if (!task) return;
            await Task.Delay(3000);
            progresso.Visibility = Visibility.Visible;
            task = await salvarDancoNew(3);
            if (!task)
            {
                return;
            }
            else { Close(); }
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
        private void btOK_Click(object sender, RoutedEventArgs e)
        {
            config.Default.Upgrade();
            config.Default.pathdb = editpath.Text;
            config.Default.Save();
            btOK.Visibility = Visibility.Collapsed;
        }

        private void btOpenPath_Click(object sender, RoutedEventArgs e)
        {
            VistaFolderBrowserDialog openFileDialog = new VistaFolderBrowserDialog();
            if (openFileDialog.ShowDialog()==true)
            {
                editpath.Text = openFileDialog.SelectedPath;
                btOK.Visibility = Visibility.Visible;
            }
        }
    }
}
