using PainelPress.Classes;
using PainelPress.Elementos;
using PainelPress.Model;
using PainelPress.Paginas;
using Refit;
using System;
using System.Collections.Generic;
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
using XamlRadialProgressBar;
using config = PainelPress.Properties.Settings;

namespace PainelPress.Partes
{
    /// <summary>
    /// Interação lógica para ImageContainer.xam
    /// </summary>
    public partial class ImageContainer : ContentControl
    {
        public string IMAGEM = "";
        public string NOME = "";
        public string CAMPO = "";
        public bool PADRAO = true;
        byte[] ImgSelecao;
        bool start = false;
        List<string> listCampos = new List<string>();
        Postar? post;
        RestAPI rest = new RestAPI();
        InterfaceAPI apiRestBarear;
        public ImageContainer()
        {
            InitializeComponent();
            PADRAO = config.Default.miniatura;
            apiRestBarear = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(config.Default.Token)
            });
        }


        public ImageContainer(string p, bool l)
        {
            InitializeComponent();
            SetapImage(p, l);
        }

        public void SetPostar(Postar postar)
        {
            post = postar;
        }

        public async void SetapImage(string path, bool local)
        {
            stackImage.Visibility = Visibility.Visible;
            stackNoImage.Visibility = Visibility.Collapsed;
            start = false;
            if (PADRAO)
            {
                radSim.IsChecked = true; 
            }
            else
            {
                radNo.IsChecked = true;
                boxCampos.Visibility = Visibility.Visible;
            }
            start = true;
            if (local)
            {
                SetImage(path);
            }
            else
            {
                SetProgresso();
                string img = await ImagemTool.BaixarImage(path);
                SetImage(img);
                SetProgresso(false);
            }
        }

        private void SetProgresso(bool show = true)
        {      
            if (show)
            {
                if (bdProgresso.Child == null)
                {
                    bdProgresso.Child = new RadialProgressBar()
                    {
                        Foreground = CorImage.GetCorVerde(),
                        IsIndeterminate = true,
                        IndeterminateSpeedRatio = 0.2
                    };
                }
                bdProgresso.Visibility = Visibility.Visible;
            }
            else
            {
                bdProgresso.Visibility = Visibility.Collapsed;
            }   
        }

        public void SetCampos(List<string> list)
        {
            listCampos = list;
            boxCampos.ItemsSource = listCampos;
            selectCampo(config.Default.campo_imagem);
        }

        private async void SetImage(string path)
        {
            IMAGEM = path;
            ImgSelecao = IMGtoBITY(path);
            imgUp.Source = ByteArrayToImage(ImgSelecao);
            if(NOME=="") NOME = ImagemTool.getNome(path);
            if (!PADRAO)
            {
                if (CAMPO == "") return;
                if (post == null) return;
                SetProgresso();
                RequisicaoBol request = await rest.UploadImagem(NOME,ImagemTool.getExtensao(path), ImgSelecao);
                if(request != null)
                {
                    post.setValueCampo(CAMPO, request.Mensagem);
                    if(post.IDPost > 0)
                    {
                        MensagemToast.instance.HomeMensagem(true, "Imagem do post atualizada");
                    }
                    
                }
                SetProgresso(false);

            }
            else
            {
                if(post != null && post.IDPost > 0)
                {
                    bool acao = await EnviarImagem();
                    if (acao)
                    {
                        MensagemToast.instance.HomeMensagem(true,"Imagem do post atualizada");
                    }
                    else
                    {
                        MensagemToast.instance.HomeMensagem(false, "Não foi possivel atualizar a imagem do post");
                    }
                }
            }
        }

        public async void SetImageOnline(string url)
        {
            stackImage.Visibility = Visibility.Visible;
            stackNoImage.Visibility = Visibility.Collapsed;
            SetProgresso();
            ImgSelecao = await ImagemTool.UrlToImage(url);
            if(ImgSelecao != null) imgUp.Source = ByteArrayToImage(ImgSelecao);
            btRemove.Visibility = Visibility.Visible;
            stackRadios.Visibility = Visibility.Collapsed;
            SetProgresso(false);
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
            catch (Exception ex)
            {
                Debug.WriteLine("ByteArrayToImage=> " + ex.Message);
            }

            return null;
        }

        private byte[] IMGtoBITY(string imageIn)
        {
            var tiffArray = File.ReadAllBytes(imageIn);
            return tiffArray;

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!start) return;
            RadioButton radio = (RadioButton) sender;
            if(radio != null)
            {
                PADRAO = radio.Tag.ToString().Equals("sim");
                config.Default.Upgrade();
                config.Default.miniatura = PADRAO;
                config.Default.Save();
                if (PADRAO)
                {
                    boxCampos.Visibility = Visibility.Collapsed;
                }
                else
                {
                    boxCampos.Visibility = Visibility.Visible;
                    if (CAMPO == "")
                    {
                        start = false;
                        boxCampos.IsDropDownOpen = true;
                        radSim.IsChecked = true;
                        start = true;
                    }
                }
            }
       }

        private void btOpenUpload_Click(object sender, RoutedEventArgs e)
        {
            WUpload upload = new WUpload(true, IMAGEM);
            if (upload.ShowDialog() == true)
            {
                if(upload.PATH != "" && IMAGEM != upload.PATH)
                {
                    NOME = upload.NOME;
                    SetapImage(upload.PATH, true);       
                }
            }
        }

        private void imgUp_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (btRemove.IsVisible) return;
            btOpenUpload_Click(btOpenUpload, null);
        }

        private void boxCampos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(boxCampos.SelectedItem != null)
            {
                
                CAMPO = boxCampos.SelectedItem as string;
                config.Default.Upgrade();
                config.Default.campo_imagem = CAMPO;
                config.Default.Save();
                if (radNo.IsChecked!= true)
                {
                    start = false;
                    radNo.IsChecked = true;
                    start = true;
                }
            }
        }

        private void selectCampo(string nome)
        {
            if (nome == "") return;
            CAMPO = nome;
            int index = 0;
            foreach(string campo in listCampos)
            {
                if (nome == campo)
                {
                    boxCampos.SelectedIndex = index; 
                    break;
                }
                index++;
            }
        }

        private async void btRemove_Click(object sender, RoutedEventArgs e)
        {
            SetProgresso();
            RequisicaoBol request = null;
            if (PADRAO)
            {
                if (post != null && post.IDPost > 0)
                {
                    request = await apiRestBarear.DeleteImagemDestaque(post.IDPost);
                }
            }
            else
            {
                if(post !=null && CAMPO !="")
                {
                    post.setValueCampo(CAMPO, "");
                    if(post.IDPost > 0) {
                        request = await apiRestBarear.DeleteCampo(post.IDPost, CAMPO); 
                    }
                }       
            }
            if (request != null)
            {
                if (!request.Sucesso)
                {
                    MensagemToast.instance.HomeMensagem(false, "Erro ao remover imagem online");
                }
            }
            SetProgresso(false);
            Reset();
        }

        private void Reset()
        {
            stackImage.Visibility = Visibility.Collapsed;
            stackNoImage.Visibility = Visibility.Visible;
            btRemove.Visibility = Visibility.Collapsed;
            stackRadios.Visibility = Visibility.Visible;
        }

        public async Task<bool> EnviarImagem()
        {
            if(post==null) return false;
            try
            {
                RequisicaoBol request = await rest.UploadMedia(NOME, ImagemTool.getExtensao(IMAGEM), post.IDPost, ImgSelecao);
                if (request != null)
                {
                    return request.Sucesso;
                }
            }catch(Exception ex) {
                Debug.WriteLine(ex.Message);
            }
            return false;
        }

    }
}
