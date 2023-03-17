using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Lógica interna para WSocial.xaml
    /// </summary>
    public partial class WSocial : Window
    {

        byte[] ImgSelecao;
        TwitterService twitterService;
        MensagemToast mensagem = new MensagemToast();
        Social social = new Social();
        bool twiter;
        Social facebook;
        public bool HIDENBW = true;
        public WSocial()
        {
            InitializeComponent();
            Setap();
        }


        private async void Setap()
        {
            twitterService = new TwitterService();
            if (social.getTwiterCredencial() != "")
            {
                var user = await twitterService.Setap();
                twiter = social.twitter();

                if (user != "")
                {
                    stackLogado.Visibility = Visibility.Visible;
                    nomeTwitter.Text = user;
                    btLogin.Visibility = Visibility.Collapsed;
                }
                else
                {
                    twiter = false;
                }
            }
            facebook = social.facebook();
            if (facebook.ativado)
            {
                try
                {
                    ediToken.Text = facebook.token;
                    ediPageFace.Text = facebook.pagina;
                }
                catch (Exception ex) { }
            }
            if(facebook.ativado ==false && twiter == false)
            {
                btEnviar.IsEnabled= false;
            }
        }
        private async void btAcao_Click(object sender, RoutedEventArgs e)
        {
            //  string resultado =  pythonEnginer.ExecutarCodigo(@"def Nome(name):
            // return 'Hello ' + name.title()", "Nome", "Elton");
            if (string.IsNullOrEmpty(editMensagem.Text)) return;

            if ((bool)radTwitter.IsChecked)
            {
                if (twiter) SendTwitter();
            }
            else {
                await SendFacebook(editMensagem.Text, null);
            }
        }

        private async void SendTwitter()
        {
            pLoading.Visibility = Visibility.Visible;
            if (string.IsNullOrEmpty(editUrlImage.Text))
            {
                var send = await twitterService.PublicarTweetSimples(editMensagem.Text);
                if (send) {
                    mensagem.HomeMensagem(true, "Twett enviado");
                }
                else
                {
                    mensagem.HomeMensagem(false, "Twett não enviado");
                }
            }
            else
            {
                var send = await twitterService.PublicarTweetImage(editMensagem.Text, ImgSelecao);
                if (send)
                {
                    mensagem.HomeMensagem(true, "Twett enviado");
                }
                else
                {
                    mensagem.HomeMensagem(false, "Twett não enviado");
                }
            }
            pLoading.Visibility = Visibility.Collapsed;
        }

        public async Task SendFacebook(string msg, string link)
        {
            if(facebook.token=="")
            {
                mensagem.HomeMensagem(false, "Erro ao enviar ao facebook. Você precisa configurar um token");
                return;
            }
            if (link == null && msg.Contains("https"))
            {
                var dados = msg.Split("https", 2);
                msg = dados[0].Trim();
                link = "https" + dados[1].Trim();
            }
            string url;
            if (link == null)
            {
                url = Constants.urlSendFacebook.Replace("{token}", facebook.token).Replace("{idpagina}", facebook.pagina).Replace("{mensagem}", msg).Replace("&link={link}", "");
            }
            else
            {
                 url = Constants.urlSendFacebook.Replace("{token}", facebook.token).Replace("{idpagina}", facebook.pagina).Replace("{mensagem}", msg).Replace("{link}", link);
            }
            RestAPI restAPI = new RestAPI();
           
              var request = await restAPI.POST(url, null);
            if (request == null)
            {
                mensagem.HomeMensagem(false, "Erro ao enviar ao facebook - NULL");
                return;
            }
            if (request.Contains("erro"))
            {
                mensagem.HomeMensagem(false, "Erro ao enviar ao facebook");
            }
            else
            {
                mensagem.HomeMensagem(true, "Post enviado ao facebook");
            }
            Debug.WriteLine(request);

        }

        #region BackgroundWorker

        public void startTask(string param)
        {
            if (!facebook.ativado) return;
            HIDENBW = (bool)hidenBrowser.IsChecked;
            pLoading.Visibility = Visibility.Visible;
            BackgroundWorker worker = new BackgroundWorker();
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork;
            worker.ProgressChanged += worker_ProgressChanged;
            worker.RunWorkerAsync(argument: param);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            string text = (string)e.Argument;
            var worker = sender as BackgroundWorker;
            worker.ReportProgress(0, "Carregando...");
            try
            {
                ClassSeleniun classSeleniun = new ClassSeleniun(HIDENBW);
                var resultado = classSeleniun.PostaFace(text);
                e.Result = resultado;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception worker_DoWork: " + ex.Message);
                e.Result = false;
            }

            worker.ReportProgress(100, "Carregado 100%");
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs result)
        {

            pLoading.Visibility = Visibility.Collapsed;
            if ((bool)result.Result) {
                mensagem.HomeMensagem(true, "Post enviado");
            }
            else
            {
                mensagem.HomeMensagem(false, "Erro ao enviar");
            }


        }
        #endregion


        public static BitmapFrame ByteArrayToImage(byte[] byteArrayIn)
        {
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn);
                return BitmapFrame.Create(ms,
                                          BitmapCreateOptions.None,
                                          BitmapCacheOption.OnLoad);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return null;
        }

        private void SetImage(string path)
        {
            editUrlImage.Text = path;
            ImgSelecao = IMGtoBITY(path);
            imgUp.Source = ByteArrayToImage(ImgSelecao);

        }
        private byte[] IMGtoBITY(string imageIn)
        {
            var tiffArray = File.ReadAllBytes(imageIn);
            return tiffArray;

        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Note that you can have more than one file.
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                SetImage(files[0]);
            }
        }

        private void editUrlImage_GotFocus(object sender, RoutedEventArgs e)
        {
            editUrlImage.SelectAll();
            Clipboard.SetText(editUrlImage.Text);
        }
        private void btTag_Click(object sender, RoutedEventArgs e)
        {
            var botao = (Button)sender;
            editMensagem.Text = editMensagem.Text + " " + botao.Content;
        }

        private void btSelectImg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Imagem Arquivos|*.png;*.jpg";

            if (openFileDialog.ShowDialog() == true)
            {
                SetImage(openFileDialog.FileName);
            }
        }

        private void btConfig_Click(object sender, RoutedEventArgs e)
        {
            if (gridConfig.IsVisible)
            {
                gridConfig.Visibility = Visibility.Collapsed;
            }
            else
            {
                gridConfig.Visibility = Visibility.Visible;
            }
        }

        private void btSalvarConfig_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ediToken.Text))
            {
                social.setFacebook(new Social()
                {
                    ativado = false,
                    token = "",
                    pagina = "",
                });
                log.Text = "Facebook desativado";
            }
            else
            {
                social.setFacebook(new Social()
                {
                    ativado = true,
                    token = ediToken.Text,
                    pagina = ediPageFace.Text
                });
                log.Text = "Dados facebook salvos";
            }

        }

        private async void btLogin_Click(object sender, RoutedEventArgs e)
        {
            var user = await twitterService.AuthTwitter();
            if (user != null)
            {
                stackLogado.Visibility = Visibility.Visible;
                nomeTwitter.Text = user;
                btLogin.Visibility = Visibility.Collapsed;
                social.setTwiter(true);
            }
            else
            {
                log.Text = "Erro ao autenticar";
            }
        }

        private void btSairTwitter_Click(object sender, RoutedEventArgs e)
        {
            social.sairTwiter();
            stackLogado.Visibility = Visibility.Collapsed;
            btLogin.Visibility = Visibility.Visible;
        }
    }
}
