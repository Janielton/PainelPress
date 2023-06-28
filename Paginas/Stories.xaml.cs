using OpenQA.Selenium;
using PainelPress.Classes;
using PainelPress.Elementos;
using PainelPress.Model;
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
using static Google.Api.ResourceDescriptor.Types;

namespace PainelPress.Paginas
{
    /// <summary>
    /// Interação lógica para Stories.xam
    /// </summary>
    public partial class Stories : ContentControl
    {
        InterfaceAPI apiRest;
        int idStory = 0;
        Story selecaoStory;
    

        public Stories()
        {
            InitializeComponent();
            apiRest = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                Task.FromResult(Constants.TOKEN)
            });
            bdList.Visibility = Visibility.Visible;
            getStories();
        }

        public Stories(PostView post)
        {
            InitializeComponent();
            editTitulo.Text = post.Title;
            idPost.Text = post.Id.ToString();
            editResumo.Text = post.Resumo;
           // editImagem.Text = post.Meta;
            slug.Text = post.Slug.Replace(Constants.SITE, "").Replace("/","");
            apiRest = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                Task.FromResult(Constants.TOKEN)
            });
            bdAdd.Visibility = Visibility.Visible;
            PuxarStory(idPost.Text.Trim());
         
        }

        private async void PuxarStory(string id)
        {
            if (bdList.IsVisible)
            {
                bdList.Visibility = Visibility.Collapsed;
                bdAdd.Visibility = Visibility.Visible;
            }
            try
            {
                var request = await apiRest.getStoryByPost(id);
                if(request != null)
                {
                    idStory = Convert.ToInt32(request.id);
                    setEdicao(request);
                }
                else
                {
                    if (editImagem.Text.Length > 199)
                    {
                        textCountImagem.Text = "Url da imagem deve ter menos que 200 caracteres";
                    }
                }
            }catch(Exception ex)
            {
                Alert(ex.Message, "Erro ao carregar story");
            }
            finally {

                loading.Visibility = Visibility.Collapsed;
            }
        }

        private async void getStories()
        {
            try
            {
                var request = await apiRest.getStories();
                listStories.ItemsSource = request;
            }
            catch (Exception ex)
            {
                Alert(ex.Message, "Erro ao carregar stories");
            }
            finally
            {

                loading.Visibility = Visibility.Collapsed;
            }
        }

        private void setEdicao(Story story)
        {
            editTitulo.Text = story.titulo;
            idPost.Text = story.id_post;
            editResumo.Text = story.resumo;
            editImagem.Text = story.imagem;
            slug.Text = story.slug;
            btAdicionar.Content = "Atualizar";
        }

        private void editTitulo_LostFocus(object sender, RoutedEventArgs e)
        {
           if(btAdicionar.IsEnabled && idStory==0) slug.Text = Ferramentas.ToUrlSlug(editTitulo.Text);
        }


        private async void btAdicionar_Click(object sender, RoutedEventArgs e)
        {
            Story story = montarStory();
            if (story == null)
            {
                Alert("Por favor preencha todos os campos", "Campo(s) vazio(s)");
                return;
            }
            try
            {
                RequisicaoBol request;
                bool edit = idStory > 0;
                if (edit)
                {
                     story.id = idStory.ToString();
                     request = await apiRest.EditStory(story);
                }
                else
                {
                     request = await apiRest.AddStory(story);
                }
               
                if (request.Sucesso)
                {
                    status.Text = edit ? "Story atualizado": "Story adicionado";
                    btVisualizar.Visibility = Visibility.Visible;
                    Disable();
                }
                else
                {
                    Alert(request.Mensagem);
                }
            }catch (Exception ex) {
                Alert(ex.Message);
            }

        }

        private Story? montarStory()
        {
            if(string.IsNullOrEmpty(editTitulo.Text)) return null;
            if (string.IsNullOrEmpty(slug.Text)) return null;
            if (string.IsNullOrEmpty(editData.Text)) return null;
            if (string.IsNullOrEmpty(editImagem.Text)) return null;
            if(editImagem.Text.Length>200) return null;
            if (string.IsNullOrEmpty(editResumo.Text)) return null;
            return new Story()
            {
                titulo = editTitulo.Text,
                resumo = editResumo.Text,
                slug = slug.Text,
                imagem = editImagem.Text,
                id_post = idPost.Text,
                data = getData()
            };
        }

        private string getData()
        {
            try
            {
                return editData.Text.Equals("Agora")
               ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
               : Convert.ToDateTime(editData.Text).ToString("yyyy-MM-dd HH:mm:ss");
            }catch (Exception ex) {
                return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        private void Disable()
        {
            editData.IsEnabled = false;
            editImagem.IsEnabled = false;
            editResumo.IsEnabled = false;
            editTitulo.IsEnabled = false;
            slug.IsEnabled = false;
            btAdicionar.IsEnabled = false;
        }
        private void editData_GotFocus(object sender, RoutedEventArgs e)
        {
            editData.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void Alert(string mensagem, string titulo = "")
        {
            if (titulo == "")
            {
                AlertMensagem.instance.Show(mensagem);
            }
            else
            {
                AlertMensagem.instance.Show(mensagem, titulo);
            }

        }

 

        private void btVisualizar_Click(object sender, RoutedEventArgs e)
        {
            string url = $"{Constants.SITE}/story/{slug.Text}";
            Ferramentas.OpenUrl(url);
        }


        private void editResumo_LostFocus(object sender, RoutedEventArgs e)
        {
            textCountResumo.Text = editResumo.Text.Length + " caracteres";
        }

        private void editImagem_LostFocus(object sender, RoutedEventArgs e)
        {
            if (editImagem.Text.Length > 199)
            {
                textCountImagem.Text = "Url da imagem deve ter menos que 200 caracteres";
            }
            else
            {
                textCountImagem.Text = "";
            }
        }

        private void listStories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(listStories.SelectedItem !=null)
            {
                selecaoStory = listStories.SelectedItem as Story;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            if (item == null) return;
            if (selecaoStory == null) return;
            if(item.Tag.ToString() == "ver")
            {
                string url = Constants.SITE + "/story/" + selecaoStory.slug;
                Ferramentas.OpenUrl(url);
            }
            else if(item.Tag.ToString() == "edit"){
                PuxarStory(selecaoStory.id_post);
            }
            else if (item.Tag.ToString() == "del")
            {
                Apagar(selecaoStory.id);
            }
        }

        private async void Apagar(string id)
        {
            try
            {
                var acao = MessageBox.Show("Tem certeza que deseja apagar story: "+selecaoStory.titulo+"?", "Apagar story",MessageBoxButton.YesNo);
                if (acao == MessageBoxResult.No) return;
                var request = await apiRest.DeleteStory(id);
                if (request.Sucesso)
                {
                    getStories();
                }
                else
                {
                    Alert("Erro ao apagar story");
                }

            }
            catch(ApiException ex)
            {
                Alert(ex.Message, "Erro ao apagar");
            }
        }
    }
}
