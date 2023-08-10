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
using System.Windows.Media;
using PainelPress.Classes.Controller;
using System.Windows.Controls;
using System.Windows.Shapes;

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
        ScreenShot screenShot = new();
        string IMAGEM = "";
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
                NOME = ImagemTool.getNome(path);
                SetImage(path);
            }
        }

        private void Perfil_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                SetImage(files[0]);
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string draggedFileUrl = (string)e.Data.GetData(DataFormats.Text, false);
                Debug.WriteLine($"url => {draggedFileUrl}");
                BaixarImage(draggedFileUrl);
            }
        }
        private void Perfil_DragEnter(object sender, DragEventArgs e)
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

        public async void BaixarImage(string url, int tipo = 1)
        {
            if (string.IsNullOrEmpty(url)) return;
            RestAPI restAPI = new RestAPI();
            var img = await restAPI.BaixarDados(url);
            if (img != null)
            {
                var source = ImagemTool.ByteArrayToBitmap(img);
                if (source != null)
                {
                    if (tipo == 1)
                    {
                        imgPerfil.Background = new ImageBrush() { ImageSource = source };
                    }
                    else if (tipo == 2)
                    {
                        imagemFull.Background = new ImageBrush() { ImageSource = source };
                    }
                    else
                    {
                        imagemFundo.Background = new ImageBrush() { ImageSource = source };
                    }

                }

            }
        }
        private void SetImage(string path, int tipo = 2)
        {

            ImgSelecao = IMGtoBITY(path);
            var source = ImagemTool.ByteArrayToBitmap(ImgSelecao);
            if (tipo == 1)
            {
                imgPerfil.Background = new ImageBrush() { ImageSource = source };
            }
            else if (tipo == 2)
            {
                imagemFull.Children.Clear();
                imagemFull.Background = new ImageBrush() { ImageSource = source };

            }
            else
            {
                imagemFundo.Background = new ImageBrush() { ImageSource = source };
            }

        }
        private byte[] IMGtoBITY(string imageIn)
        {
            var tiffArray = File.ReadAllBytes(imageIn);
            return tiffArray;

        }



        private void btUpload_Click(object sender, RoutedEventArgs e)
        {
            if (imagemContainer.IsVisible)
            {
                ImgSelecao = screenShot.getAndTemp(imagemContainer);
            }
            else
            {
                ImgSelecao = screenShot.getAndTemp(imagemFull);
            }

            if (ImgSelecao == null)
            {
                status.Text = "Erro ao gerar imagem";
                return;
            }
            if (btUpload.Content == "Definir")
            {
                string img = ImagemTool.SaveImagem(ImgSelecao,"imagem_post");
                if (img != null)
                {
                    PATH = img;
                    NOME = editNome.Text;
                    this.DialogResult = true;
                    this.Close();
                }
            }
            else
            {
                
                UploadSend();
            }
           
        }

        private void Reset()
        {
            imgUpOk.Visibility = Visibility.Collapsed;
            editNome.Visibility = Visibility.Visible;
            gridUrl.Visibility = Visibility.Collapsed;
            editNome.Text = "";
        }

  
        private async void UploadSend() {
            try
            {

                pLoading.Visibility = Visibility.Visible;

                RequisicaoBol resultado = await restAPI.UploadImagem(editNome.Text, "png", ImgSelecao);

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
                    NOME = editNome.Text;
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

        private void ContentControl_KeyDown(object sender, KeyEventArgs e)
        {


            if (e.Key == Key.Up)
            {
                imgPerfil.Height = imgPerfil.ActualHeight + 2;
                imgPerfil.Width = imgPerfil.ActualWidth + 2;


            }
            else if (e.Key == Key.Down)
            {
                imgPerfil.Height = imgPerfil.ActualHeight - 2;
                imgPerfil.Width = imgPerfil.ActualWidth - 2;

            }

            else if (e.Key == Key.W)
            {
                if (imgPerfil.CornerRadius.TopLeft > 2)
                {
                    imgPerfil.CornerRadius = new CornerRadius(imgPerfil.CornerRadius.TopLeft - 2, imgPerfil.CornerRadius.TopRight - 2, imgPerfil.CornerRadius.BottomRight - 2, imgPerfil.CornerRadius.BottomLeft - 2);
                }

            }
            else if (e.Key == Key.S)
            {
                imgPerfil.CornerRadius = new CornerRadius(imgPerfil.CornerRadius.TopLeft + 2, imgPerfil.CornerRadius.TopRight + 2, imgPerfil.CornerRadius.BottomRight + 2, imgPerfil.CornerRadius.BottomLeft + 2);
            }
            else if (e.Key == Key.D)
            {
                imgPerfil.Margin = new Thickness(
               imgPerfil.Margin.Left + 1, imgPerfil.Margin.Top + 1,
                 imgPerfil.Margin.Right + 1, imgPerfil.Margin.Bottom + 1);
            }
            else if (e.Key == Key.A)
            {

                if (imgPerfil.Margin.Left > 2)
                {
                    imgPerfil.Margin = new Thickness(
 imgPerfil.Margin.Left - 2, imgPerfil.Margin.Top - 2,
   imgPerfil.Margin.Right - 2, imgPerfil.Margin.Bottom - 2);
                }
            }
        }

        private void imgPerfil_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void radCheker_Checked(object sender, RoutedEventArgs e)
        {
            var rad = (RadioButton)sender;
            if (rad != null)
            {
                string tag = rad.Tag.ToString();
                if (tag == "normal")
                {
                    imgPerfil.Visibility = Visibility.Collapsed;
                    imagemFull.Visibility = Visibility.Visible;
                    imagemContainer.Visibility = Visibility.Collapsed;

                }
                else if (tag == "com-imagem")
                {
                    imgPerfil.Visibility = Visibility.Visible;
                    imagemFull.Visibility = Visibility.Collapsed;
                    imagemContainer.Visibility = Visibility.Visible;
                }
                else if (tag == "full-imagem")
                {
                    imagemFull.Visibility = Visibility.Visible;
                    imagemContainer.Visibility = Visibility.Collapsed;
                }
            }
        }

        #region ImageFull
        private void imagemFull_DragEnter(object sender, DragEventArgs e)
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

        private void imagemFull_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                SetImage(files[0], 2);
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string draggedFileUrl = (string)e.Data.GetData(DataFormats.Text, false);
                BaixarImage(draggedFileUrl, 2);
            }
        }

        #endregion

        private void imagemContainer_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                SetImage(files[0], 3);
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string draggedFileUrl = (string)e.Data.GetData(DataFormats.Text, false);
                Debug.WriteLine($"url => {draggedFileUrl}");
                BaixarImage(draggedFileUrl, 3);
            }
        }

        private void imgPerfil_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void imgPerfil_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                imgPerfil.CornerRadius = new CornerRadius(imgPerfil.CornerRadius.TopLeft + 2, imgPerfil.CornerRadius.TopRight + 2, imgPerfil.CornerRadius.BottomRight + 2, imgPerfil.CornerRadius.BottomLeft + 2);
                imgPerfil.Height = imgPerfil.ActualHeight - 2;
                imgPerfil.Width = imgPerfil.ActualWidth - 2;

            }
            else
            {
                imgPerfil.CornerRadius = new CornerRadius(imgPerfil.CornerRadius.TopLeft - 2, imgPerfil.CornerRadius.TopRight - 2, imgPerfil.CornerRadius.BottomRight - 2, imgPerfil.CornerRadius.BottomLeft - 2);


            }
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