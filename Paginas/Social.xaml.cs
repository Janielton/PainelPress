using Microsoft.Win32;
using PainelPress.Classes;
using PainelPress.Model;
using ShareSocial;
using ShareSocial.Twitter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
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

namespace PainelPress.Paginas
{
    /// <summary>
    /// Interação lógica para Social.xam
    /// </summary>
    public partial class Social : ContentControl
    {
        byte[] ImgSelecao;

        MensagemToast mensagem = new MensagemToast();
        MainShare mainShare = new MainShare(Constants.PASTA);
        bool start = false;
        SocialModel social = new SocialModel();
        bool twiter;
        SocialModel facebook;
        TwitterConfig twitterConfig = new TwitterConfig().get();
        public bool HIDENBW = true;
        private bool API = true;
        public Social()
        {
            InitializeComponent();
            Setap();
        }

        private async void Setap()
        {
            //new TwitterConfig().set(new TwitterConfig()
            //{
            //    ativado = true,
            //    api = false,
            //    consumer_key = "",
            //    consumer_secret = ""
            //});
            mainShare.StartPinterest();  
            if (twitterConfig !=null && twitterConfig.ativado)
            {
                radTwitter.IsChecked = true;
                API = twitterConfig.api;
                mainShare.InstanceTwitter();
                if (API)
                {
                    radApi.IsChecked = true;
                    consumer_secret.Texto = twitterConfig.consumer_secret;
                    consumer_key.Texto = twitterConfig.consumer_key;
                    stackApiTwi.Visibility = Visibility.Visible;

                }
                else
                {
                    imgUp.Opacity = 0.2f;
                    imgUp.AllowDrop = false;
                    radWeb.IsChecked = true;
                }
                string t = await mainShare.getContaTwitter();
                stackLogado.Visibility = Visibility.Visible;
                nomeTwitter.Text = t;
                btLogin.Visibility = Visibility.Collapsed;
                radNome.Child = new TextBlock()
                {
                    Text = t,
                    FontSize = 18,
                    Padding = new Thickness(10)
                };
                twiter = true;
            }
            else
            {
                twiter = false;
            }
            facebook = social.facebook();
            if (facebook.ativado)
            {
                try
                {
                    ediToken.Texto = facebook.token;
                    ediPageFace.Texto = facebook.pagina;
                }
                catch (Exception ex) { }
            }
            if (facebook.ativado == false && twiter == false)
            {
                btUpload.IsEnabled = false;
            }
            start = true;
        }
        private async void btAcao_Click(object sender, RoutedEventArgs e)
        {
            //  string resultado =  pythonEnginer.ExecutarCodigo(@"def Nome(name):
            // return 'Hello ' + name.title()", "Nome", "Elton");
            if (string.IsNullOrEmpty(editMensagem.Texto) && string.IsNullOrEmpty(editTitulo.Texto)) return;

            if ((bool)radTwitter.IsChecked)
            {
                // if (string.IsNullOrEmpty(TwitterService.USER)) return;
                SendTwitter();
            }
            else if ((bool)radFacebook.IsChecked)
            {
                await SendFacebook(editTitulo.Texto, editLink.Texto, editMensagem.Texto, true);
            }
            else
            {

                bool invalid = "" == editTitulo.Texto || "" == editMensagem.Texto || "" == editLink.Texto || "" == editUrlImage.Text;
                if (invalid) return;
                sendPintest();
            }
        }
        private async void sendPintest()
        {
            pLoading.Visibility = Visibility.Visible;
            string id = await mainShare.createPin(editTitulo.Texto, editMensagem.Texto, editLink.Texto, editUrlImage.Text);
            if (id != null)
            {
                if (id != "naoauth")
                {
                    radNome.Child = new TextBlock()
                    {
                        Text = "Pin criado",
                        FontSize = 18,
                        Padding = new Thickness(10)
                    };
                }
                else
                {
                    MessageBox.Show("Não autorizado", "Erro ao criar PIN");
                }
            }
            else
            {
                MessageBox.Show("Erro ao criar PIN");
            }
            pLoading.Visibility = Visibility.Collapsed;
        }
        private async void SendTwitter()
        {

            string msg = $"{editTitulo.Texto} - {editLink.Texto}";
            if(editMensagem.Texto != "")
            {
                if (editLink.Texto != "")
                {
                    msg = $"{editTitulo.Texto} - {editMensagem.Texto} {editLink.Texto}";
                }
                else if(editLink.Texto == "" && editLink.Texto == "")
                {
                    msg = editMensagem.Texto;
                }
                else
                {
                    msg = $"{editTitulo.Texto} - {editMensagem.Texto}";
                }
            
            }

            if (validarTwet())
            {
                pLoading.Visibility = Visibility.Visible;
                if (string.IsNullOrEmpty(editUrlImage.Text))
                {
    
                    var send = await mainShare.createTweet(msg);
                    if (send)
                    {
                        mensagem.HomeMensagem(true, "Twett enviado");
                    }
                    else
                    {
                        mensagem.HomeMensagem(false, "Twett não enviado");
                        log.Text = "Twett não enviado";
                    }
                }
                else
                {
                    var send = await mainShare.createTweet(msg, ImgSelecao);
                    if (send)
                    {
                        mensagem.HomeMensagem(true, "Twett enviado");
                    }
                    else
                    {
                        mensagem.HomeMensagem(false, "Twett não enviado");
                        log.Text = "Twett não enviado";
                    }
                }
                pLoading.Visibility = Visibility.Collapsed;
            }
            else
            {
                Debug.WriteLine("erro createTweet");
            }
            
        }

        private bool validarTwet()
        {
            string t = editTitulo.Texto;
            string d = editMensagem.Texto;
            string l = editLink.Texto;
            if (t == "" && d == "" && l == "") return false;
            if (t != "" && l == "" && d == "")
            {
                log.Text = "Você deve adicionar um link";
                return false;
            }
            return true;
        }

        public async Task SendFacebook(string titulo, string link, string msg, bool win = false)
        {
            if (facebook.token == "")
            {
                mensagem.HomeMensagem(false, "Erro ao enviar ao facebook. Você precisa configurar um token");
                return;
            }
            
            string url;
            if (link.Trim() == "")
            {
                url = Constants.urlSendFacebook.Replace("{token}", facebook.token).Replace("{idpagina}", facebook.pagina).Replace("{mensagem}", $"{titulo}\n\n{msg}").Replace("&link={link}", "");
            }
            else
            {
                if (msg != "")
                {
                    url = Constants.urlSendFacebook.Replace("{token}", facebook.token).Replace("{idpagina}", facebook.pagina).Replace("{mensagem}", $"{titulo} \n\n {msg}").Replace("{link}", link);
                }
                else
                {
                    url = Constants.urlSendFacebook.Replace("{token}", facebook.token).Replace("{idpagina}", facebook.pagina).Replace("{mensagem}", titulo).Replace("{link}", link);
                }
              
            }
            RestAPI restAPI = new RestAPI();

            var request = await restAPI.POST(url, null);
            if (win)
            {
                if (request == null)
                {
                    log.Text = "Erro ao enviar ao facebook - NULL";
                    return;
                }
                if (request.Contains("erro"))
                {
                    log.Text = "Erro ao enviar ao facebook";
                }
                else
                {
                    log.Text = "Post enviado ao facebook";
                }
            }
            else
            {
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
               // Debug.WriteLine(request);
            }
           

        }

        #region BackgroundWorker

        public void startTask(string param)
        {
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
                //  var resultado = pythonEnginer.ExecutarArquivo("teste.py", "TesteCodec", Conversor64.EncodeToBase64(text));
                ClassSeleniun classSeleniun = new ClassSeleniun();
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
            //  return;
            pLoading.Visibility = Visibility.Collapsed;
            if ((bool)result.Result)
            {
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
            editMensagem.Texto = editMensagem.Texto + " " + botao.Content;
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

        private async void rad_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rad = sender as RadioButton;
            if (rad == null || start == false) return;
            string tag = rad.Tag.ToString();
            if (tag == "twit")
            {
                radNome.Child = null;
               // mainShare.StartTwitter();
                string t = await mainShare.getContaTwitter();
                
                if (t != null)
                {
                    radNome.Child = new TextBlock()
                    {
                        Text = t,
                        FontSize = 18,
                        Padding = new Thickness(10)
                    };
                }
            }
            else if (tag == "face")
            {
                imgUp.Opacity = 0.2f;
                imgUp.AllowDrop = false;
                radNome.Child = null;
                if (!social.facebook().ativado)
                {
                    gridConfig.Visibility = Visibility.Visible;
                }

            }
            else if (tag == "pint")
            {
                radNome.Child = null;
                string t = await mainShare.getConta();
                StackPanel stack = new StackPanel();
                stack.Orientation = Orientation.Vertical;
                stack.Children.Add(new TextBlock()
                {
                    Text = t,
                    FontSize = 18,
                    Padding = new Thickness(10)
                });
                Button bt = new Button()
                {
                    Content = "Atualizar Token",
                    Margin = new Thickness(5),
                    FontSize = 12,
                    Tag = "pin",
                    Padding = new Thickness(10, 1, 10, 3),
                };
                bt.Click += userSocial_Click;
                stack.Children.Add(bt);
                radNome.Child = stack;
            }
        }

        private async void userSocial_Click(object sender, RoutedEventArgs e)
        {
            Button rad = sender as Button;
            if (rad == null) return;
            string tag = rad.Tag.ToString();
            if (tag == "pin")
            {
                bool acao = await mainShare.AtualizarToken();
                if (!acao)
                {
                    MessageBox.Show("Erro ao atualizar token");
                }
            }
        }

        #region Barra Config

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
        private void btSalvarFace(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ediToken.Texto))
            {
                social.setFacebook(new SocialModel()
                {
                    ativado = false,
                    token = "",
                    pagina = "",
                });
                log.Text = "Facebook desativado";
            }
            else
            {
                social.setFacebook(new SocialModel()
                {
                    ativado = true,
                    token = ediToken.Texto,
                    pagina = ediPageFace.Texto
                });
                log.Text = "Dados facebook salvos";
            }

        }
        private void btSalvarConfig_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ediToken.Texto))
            {
                social.setFacebook(new SocialModel()
                {
                    ativado = false,
                    token = "",
                    pagina = "",
                });
                log.Text = "Facebook desativado";
            }
            else
            {
                social.setFacebook(new SocialModel()
                {
                    ativado = true,
                    token = ediToken.Texto,
                    pagina = ediPageFace.Texto
                });
                log.Text = "Dados facebook salvos";
            }
           

        }



        #endregion

        #region TWITTER_BARRA
        private async void btLogin_Click(object sender, RoutedEventArgs e)
        {
            var user = await mainShare.loginTwitter();
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
            mainShare.SairTwitter();
            stackLogado.Visibility = Visibility.Collapsed;
            radDesa.IsChecked = true;
        }
        
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!start) return;
            RadioButton radio = sender as RadioButton;
            if (radio == null) return;
            if (twitterConfig == null) twitterConfig = new TwitterConfig();
            string tag = radio.Tag.ToString();
    
            if (tag == "api")
            {
                stackApiTwi.Visibility = Visibility.Visible;
                consumer_secret.Texto = twitterConfig.consumer_secret;
                consumer_key.Texto = twitterConfig.consumer_key;
                imgUp.Opacity = 1f;
                imgUp.AllowDrop = true;
                if (consumer_key.Texto!="" && consumer_secret.Texto != "")
                {
                    twitterConfig.enableApi();
                }
            }
            else if (tag == "web")
            {
                stackApiTwi.Visibility = Visibility.Collapsed;
                imgUp.Opacity = 0.2f;
                imgUp.AllowDrop = false;
                twitterConfig.disableApi();
            }
            else
            {
                twitterConfig.disable();
                stackLogado.Visibility = Visibility.Collapsed;
                stackApiTwi.Visibility = Visibility.Collapsed;
            }
        }
        private void btSalvarApi_Click(object sender, RoutedEventArgs e)
        {
            if (consumer_key.Texto != "" && consumer_secret.Texto != "")
            {
                var conf = new TwitterConfig();
                conf.api = true;
                conf.consumer_secret = consumer_secret.Texto;
                conf.consumer_key = consumer_key.Texto;
                conf.ativado = true;
                conf.save();
                twitterConfig = conf;
                log.Text = "Twitter salvo";
            }
            else
            {
                log.Text = "Adicione Consumer Key e Consumer Secret";
            }
 
        }
        #endregion


    }
}
