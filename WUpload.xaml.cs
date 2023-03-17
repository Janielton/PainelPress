using Microsoft.Win32;
using Refit;
using System;
using System.Collections.Generic;
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
using Newtonsoft.Json;
using PainelPress.Paginas;

namespace PainelPress
{
    /// <summary>
    /// Lógica interna para WUpload.xaml
    /// </summary>
    public partial class WUpload : Window
    {
        byte[] ImgSelecao;
        Ferramentas ferramenta = new Ferramentas();
        bool Post = false;


        public WUpload()
        {
            InitializeComponent();
        }

        public WUpload(bool post)
        {
            InitializeComponent();
            this.Post = post;
            //Debug.WriteLine(Conversor64.EncodeToBase64("painelpress:85454588s$3344"));
        }

        private void btSelectImg_Click(object sender, RoutedEventArgs e)
        {
            if (editUrlImage.Text.StartsWith("http"))
            {
                BaixarImage(editUrlImage.Text);
            }
            else
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Imagem Arquivos|*.png;*.jpg;*.webp";

                if (openFileDialog.ShowDialog() == true)
                {
                    SetImage(openFileDialog.FileName);
                }
            }
           
        }

        private void btUpload_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(editUrlImage.Text)) {
                UploadSend();
            }
        }

        private void Reset()
        {
            imgUpOk.Visibility = Visibility.Collapsed;
            editNome.Visibility = Visibility.Visible;
            editUrlImage.Text = "";
            editNome.Text = "";
        }

        private async void BaixarImage(string url)
        {
            editUrlImage.Text = url;
            RestAPI restAPI = new RestAPI();
            var img = await restAPI.BaixarDados(url);
            if (img != null)
            {
                imgUp.Source = ByteArrayToImage(img);
            }

        }

        private async void UploadSend() {
            try
            {

                string param = $"extensao={getExtensao(editUrlImage.Text)}&nome={editNome.Text}";
                string urlUplod = Constants.SITE + $"/api?rota=upload&{param}";
                pLoading.Visibility = Visibility.Visible;
               
                RestAPI restAPI = new RestAPI();
                RequisicaoBol resultado = await restAPI.UploadImagemStrem(urlUplod, ImgSelecao);

                pLoading.Visibility = Visibility.Collapsed;
                if (resultado.Sucesso)
                {
                    imgUpOk.Visibility = Visibility.Visible;
                    editNome.Visibility = Visibility.Collapsed;
                    editUrlImage.Text = resultado.Mensagem;
                    Clipboard.SetText(editUrlImage.Text);
                    if (Post)
                    {
                        Postar._ediImg.Text = editUrlImage.Text;
                        DialogResult = true;
                        this.Close();
                    }
                  
                }
                else
                {
                    imgUpErro.Visibility = Visibility.Visible;
                }
               
             
            }
            catch(Exception ex) {
                Debug.WriteLine("UploadSend => "+ex.Message);
                pLoading.Visibility = Visibility.Collapsed;
            }
        }

        private string getExtensao(string caminho) {
            var split = caminho.Split(".");
            int index = split.Length - 1;
            return split[index];
        }

        private string getNome(string caminho)
        {

            var split = caminho.Split("\\");
            int index = split.Length - 1;
            var split2 = split[index].Split(".");
            return Ferramentas.ToUrlSlug(split2[0]);
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
            editNome.Text = getNome(path);

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

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                SetImage(files[0]);
            }
            else if(e.Data.GetDataPresent(DataFormats.Text))
            {
                string draggedFileUrl = (string)e.Data.GetData(DataFormats.Text, false);
                Debug.WriteLine($"url => {draggedFileUrl}");
                BaixarImage(draggedFileUrl);
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

       
    }
}