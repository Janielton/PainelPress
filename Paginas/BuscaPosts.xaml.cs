using Newtonsoft.Json;
using PainelPress.Classes;
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

namespace PainelPress.Paginas
{
    /// <summary>
    /// Interação lógica para BuscaPosts.xam
    /// </summary>
    public partial class BuscaPosts : ContentControl
    {
        MensagemToast mensagem = new MensagemToast();
        string ultimaBusca = "";
        InterfaceAPI api;
        public BuscaPosts()
        {
            InitializeComponent();
            api = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(Constants.TOKEN)
            });
            if (Clipboard.GetText() != "")
            {
                ediBusca.Text = Clipboard.GetText();
               /// ediBusca.CaretIndex = Clipboard.GetText().Length-1;
            }
        }

        private void ediBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ediBusca.Text.Length > 2)
            {
                BuscarPosts();
            }
            else
            {
                listPost.ItemsSource = null;
            }
        }


        private async void BuscarPosts()
        {
            await Task.Delay(500);
            if (ultimaBusca.Equals(ediBusca.Text)) return;

            ultimaBusca = ediBusca.Text;
            var resultado = await api.GetPosts(ultimaBusca);
            Debug.WriteLine(JsonConvert.SerializeObject(resultado));
            if (resultado.Count == 0)
            {
                resultado.Add(new PostSimples()
                {
                    Id = 0,
                    Title = "Nenhum post encontrado",
                    Link = "",
                    Date = DateTime.Now
                });
            }
            listPost.ItemsSource = resultado;
        }

        private void listPost_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listPost.SelectedItem != null)
            {
                var post = listPost.SelectedItem as PostSimples;
                if (Convert.ToInt32(post.Id) == 0) return;
                WNavegador navegador = new WNavegador(post.Link, false);
                navegador.Show();
            }
        }
    }
}
