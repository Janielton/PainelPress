using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using PainelPress.Classes;
using PainelPress.Paginas;
using PainelPress.Elementos;
using FontAwesome.WPF;

namespace PainelPress
{
    /// <summary>
    /// Lógica interna para WUpload.xaml
    /// </summary>
    public partial class WUpload : Window
    {
        byte[] ImgSelecao;
        bool Post = false;
        public string PATH = "";
        public string NOME = "";
        RestAPI restAPI = new RestAPI();
        public WUpload()
        {
            InitializeComponent();
        }

        public WUpload(bool post, string path = "")
        {
            InitializeComponent();
            this.Post = post;
            btUpload.Content = "Definir";
            if (path != "")
            {
                SetImage(path);    
            }
        }

        private void btSelectImg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Imagem Arquivos|*.png;*.jpg;*.webp";

            if (openFileDialog.ShowDialog() == true)
            {
                SetImage(openFileDialog.FileName);
            }

        }

        private void btUpload_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(editUrlImage.Text)) {
                if (btUpload.Content.ToString().Equals("Enviar")){
                    UploadSend();
                }
                else
                {
                    PATH = editUrlImage.Text;
                    NOME = editNome.Text;
                    DialogResult = true;
                    this.Close();
                }        
            }
        }

        private void Reset()
        {
            imgUpOk.Visibility = Visibility.Collapsed;
            editNome.Visibility = Visibility.Visible;
            gridUrl.Visibility = Visibility.Collapsed;
            editUrlImage.Text = "";
            editNome.Text = "";
            imgUp.Source = CorImage.GetImagemProjeto("noimagem");
        }

  
        private async void UploadSend() {
            try
            {

                string param = $"extensao={ImagemTool.getExtensao(editUrlImage.Text)}&nome={editNome.Text}";
                string urlUplod = Constants.SITE + $"/api?rota=upload&{param}";
                pLoading.Visibility = Visibility.Visible;
               
              
                RequisicaoBol resultado = await restAPI.UploadImagem(editNome.Text, ImagemTool.getExtensao(editUrlImage.Text), ImgSelecao);

                pLoading.Visibility = Visibility.Collapsed;
                if (resultado == null)
                {
                    AlertMensagem.instance.Show("Erro ao fazer upload");
                    return;
                }

                if (resultado.Sucesso)
                {
                    imgUpOk.Visibility = Visibility.Visible;
                    editNome.Visibility = Visibility.Collapsed;
                    Clipboard.SetText(resultado.Mensagem);
                    gridUrl.Visibility = Visibility.Visible;
                    editUrl.Texto = resultado.Mensagem;
                }
                else
                {
                    AlertMensagem.instance.Show(resultado.Mensagem, "Erro ao enviar imagem");
                    imgUpErro.Visibility = Visibility.Visible;
                }
               
             
            }
            catch(Exception ex) {
                Debug.WriteLine("UploadSend => "+ex.Message);
                pLoading.Visibility = Visibility.Collapsed;
            }
        }

        public static BitmapFrame ByteArrayToImage(byte[] byteArrayIn)
        {
            try
            {
                MemoryStream ms = new MemoryStream(byteArrayIn);
                return BitmapFrame.Create(ms,
                                          BitmapCreateOptions.None,
                                          BitmapCacheOption.OnLoad);
            }
            catch (Exception ex) {
                Debug.WriteLine("ByteArrayToImage=> "+ex.Message);
            }
            
            return null;
        }

        private void SetImage(string path) 
        {
            editUrlImage.Text = path;
            ImgSelecao = IMGtoBITY(path);
            imgUp.Source = ByteArrayToImage(ImgSelecao);
            editNome.Text = ImagemTool.getNome(path);

        }
        private byte[] IMGtoBITY(string imageIn)
        {
            var tiffArray = File.ReadAllBytes(imageIn);
            return tiffArray;

        }

        private void StackPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Html))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }

        }

        private async void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                SetImage(files[0]);
            }
            else if(e.Data.GetDataPresent(DataFormats.Text))
            {
                string draggedFileUrl = (string)e.Data.GetData(DataFormats.Text, false);
              
               string path = await ImagemTool.BaixarImage(draggedFileUrl);
                SetImage(path);
            }
        }

        private void editUrlImage_GotFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(editUrlImage.Text)) return;
            editUrlImage.SelectAll();  
           
        }

        private void imgUpOk_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Reset();
        }

        private void btCloseWin_Click(object sender, RoutedEventArgs e)
        {
           // DialogResult = false;
            this.Close();
        }

        private void btCloseUrl_Click(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void btCopyUrl_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(editUrl.Texto);
            btCopyUrl.Content = new ImageAwesome()
            {
                Icon = FontAwesomeIcon.Check,
                Width = 25,
                Foreground = CorImage.GetCorVerde()
            };
        }
    }
}