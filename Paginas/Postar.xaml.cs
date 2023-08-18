using CefSharp;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using PainelPress.Classes;
using PainelPress.Model;
using configs = PainelPress.Properties.Settings;
using PainelPress.Elementos;
using ShareSocial;
using System.Linq;
using FontAwesome.WPF;
using System.Windows.Media;
using ImageProcessor.Imaging.Quantizers.WuQuantizer;
using System.Windows.Data;
using HarfBuzzSharp;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using PainelPress.Partes;
using Tweetinvi.Core.Extensions;

namespace PainelPress.Paginas
{
    /// <summary>
    /// Interação lógica para Postar.xam
    /// </summary>
    public partial class Postar : ContentControl
    {
        public int IDPost;
        bool Add = true;
        string statusPost = "publish";
        string dataPost = "";
        public static string postConteudo = "";
        public static TextBox _ediImg;
        Usuario selecaoUsuario;
        List<string> listKeyTax = new List<string>();
        List<string> listKeyCampo = new List<string>();
        Dictionary<string, List<string>> taxanomyList = new Dictionary<string, List<string>>();
        Taxonomy selecaoTag;
        InterfaceAPI apiRestBasic;
        InterfaceAPI apiRestBarear;
        MainShare mainShare;
        RestAPI restAPI = new RestAPI();
        bool facebook = false;
        bool twitter = false;
        PostModel postAtual;
        CategoriasCheck categoriasCheck = new CategoriasCheck();
        ImageContainer imageContainer = new ImageContainer();
        MensagemToast mensagemToast = new MensagemToast();
        Dictionary<string, int> configLayout = new Dictionary<string, int>();
        Configuracoes configuracoes = new Configuracoes();
        Editor editorPost;
        public Postar()
        {
            InitializeComponent();
            apiRestBasic = RestService.For<InterfaceAPI>(Constants.SITE);
            apiRestBarear = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(configuracoes.getToken())
            });
            Setap();
        }

        public Postar(Post post)
        {
            InitializeComponent();Título: 
            apiRestBasic = RestService.For<InterfaceAPI>(Constants.SITE);
            apiRestBarear = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(configuracoes.getToken())
            });
            SetUpdate(post);
        }

        public Postar(string conteudo)
        {
            InitializeComponent();
            apiRestBasic = RestService.For<InterfaceAPI>(Constants.SITE);
            apiRestBarear = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(configuracoes.getToken())
            });
            Setap();
            if (conteudo.StartsWith("Título:"))
            {
                var array = conteudo.Split('\n');
                ediTitulo.Texto = array[0].Replace("Título:","").Trim();
                editorPost.setConteudo(conteudo.Replace(array[0],"").Trim());
            }
            else
            {
                editorPost.setConteudo(conteudo);
            }
         
        }

        public Postar(PostSimples post)
        {
            InitializeComponent();
            apiRestBasic = RestService.For<InterfaceAPI>(Constants.SITE);
            apiRestBarear = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(configuracoes.getToken())
            });
            ShowHideLoading(true);
            SetUpdate(post);
        }

        private async void Setap()
        {
           // ShowHideLoading(true);
            ////LAYOUT
            try
            {
                configLayout = configuracoes.getLayout();
                if (configLayout != null)
                {
                    if (configLayout.ContainsKey("centro"))
                    {
                        int wd = configLayout["centro"];
                        colunaCentro.Width = new GridLength(wd);
                        containerSidebar.Width = wd-5;
                    }
                   if (configLayout.ContainsKey("cats")) rowCats.Height = new GridLength(configLayout["cats"]);
                }
            }
            catch { }
            
            //USUARIOS
            List<Usuario> lista = Usuario.listaUsuarios();
            comboUser.ItemsSource = lista;
            int i = 0;
            foreach (var user in lista)
            {
                if(user.Id == configuracoes.getUsuario())
                {
                    comboUser.SelectedIndex = i;
                }
                i++;
            }

            ///CONTAINERS
            frameCats.Content = categoriasCheck;
            imageControl.Content = imageContainer;
            imageContainer.SetPostar(this);

            ////EDITOR
            editorPost = new Editor(2, Add);
            editor.Content = editorPost;

            if (editorPost.TIPO == 1)
            {
                if (configLayout.ContainsKey("editor_tamanho"))
                {
                    editorPost.setTela(configLayout["editor_tamanho"]);
                }
            }
            else
            {
                btHtml.Content = "VISUAL (F2)";
            }

            postConteudo = "";
            
            ///SOCIAL
            SocialModel social = new SocialModel();
            mainShare = new MainShare(Constants.PASTA);
            var confT = mainShare.getTwitterConfig;
            if (confT != null)
            {
              //  mainShare.StartTwitter(confT.api);
               // twitter = true;
                // var userTw = await mainShare.getContaTwitter();
                // if (userTw != "no") twitter = true;
            }

            facebook = social.facebook().ativado;
            
            setTaxs();
            setCampos();
           
        }

        #region TAXONOMYS
        private void setTaxs()
        {
            List<string> taxs = configuracoes.getTaxonomies();
            if (taxs != null)
            {

                foreach (string nome in taxs)
                {
                    containerTaxs.Children.Add(getBoxTax(nome));
                    taxanomyList.Add(nome, new List<string>());
                    listKeyTax.Add(nome);
                }
            }
        }

        private Border getBoxTax(string nome)
        {
            Border border = new Border() {
              Tag = "bd_" + nome,
              Margin = new Thickness(5),
              Background = CorImage.GetCor("#ffffff"),
              Padding = new Thickness(2),
              CornerRadius = new CornerRadius(5)
            };

            StackPanel stack = new StackPanel()
            {
                Tag = "stack_" + nome,
                Orientation = Orientation.Vertical,
                Margin = new Thickness(2)
            };

            Grid grid = new Grid();

            grid.Children.Add(new TextBlock()
            {
                Text = "post_tag"== nome? "Tags" : Ferramentas.CapitalizeTexto(nome),
                FontSize = 16,
            });

            Button buttonCl = new Button()
            {
                Tag = nome,
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 20,
                Height = 20,
                Padding = new Thickness(0),
                Background = CorImage.Transparente(),
                Visibility = Visibility.Hidden,
                Content = new ImageAwesome()
                {
                    Foreground = CorImage.GetCorIcone(),
                    Icon = FontAwesomeIcon.WindowClose,
                }
            };
            buttonCl.Click += btClearTax_Click;
            grid.Children.Add(buttonCl);
            stack.Children.Add(grid);

            TextBox edit = new TextBox()
            {
                Tag = nome,
                TextWrapping = TextWrapping.Wrap,
                MinWidth = 205,
                Margin = new Thickness(0,2,0,0),
                FontSize = 16,           
            };
            edit.TextChanged += ediTags_TextChanged;
            edit.PreviewKeyDown += ediTags_KeyDown;
            stack.Children.Add(edit);

            ListBox lista = new ListBox()
            {
                Tag = "list_" + nome,
                Background = Brushes.WhiteSmoke,
                MaxHeight = 150,
                BorderThickness = new Thickness(0),
                DisplayMemberPath = "Name",
                Cursor = Cursors.Hand
            };

            lista.SelectionChanged += boxTags_SelectionChanged;
            stack.Children.Add(lista);

            StackPanel stackList = new StackPanel()
            {
                Tag = "sl_" + nome,
                MinHeight = 20,
            };
            stack.Children.Add(stackList);

            border.Child = stack;
            return border;
        }


        private void btClearTax_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            if (item == null) return;
            string tag = item.Tag.ToString();
            string tagB = "bd_" + tag;
            Border container = FindElemento.FindBorder(containerTaxs, tagB);
            if (container == null) return;
      
            string tagS = "sl_" + tag;
            StackPanel stack = FindElemento.FindStack((StackPanel)container.Child, tagS);
            if (stack == null) return;
     
            stack.Children.Clear();
            taxanomyList[tag] = new List<string>();
      
            Button button = FindElemento.FindButton((StackPanel)container.Child, tag);
            if (button != null)
            {
                button.Visibility = Visibility.Hidden;
            }
        }

        private void boxTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var box = sender as ListBox;
            if (box != null && box.SelectedItem != null)
            {
                Taxonomy tag = box.SelectedItem as Taxonomy;
                AddTax(tag);
                box.ItemsSource = null;
            }
        }

        private void AddTax(Taxonomy tax)
        {
            selecaoTag = tax;
            string tag = selecaoTag.Tipo;
            string valor = tag == "post_tag" ? selecaoTag.TermId : selecaoTag.Name;
            var list = taxanomyList[tag];
            if (list.Contains(valor))
            {
                return;
            }

            string tagB = "bd_" + tag;
            Border container = FindElemento.FindBorder(containerTaxs, tagB);

            if (container == null)
            {
                Debug.WriteLine("No container");
                return;
            }
            StackPanel stack = container.Child as StackPanel;
            if (stack == null)
            {
                Debug.WriteLine("No stack");
                return;
            }
            string tagL = "sl_" + tag;
            StackPanel stackList = FindElemento.FindStack(stack, tagL);

            if (stackList.Children.Count == 0)
            {
                TextBlock text = new TextBlock()
                {
                    Text = selecaoTag.Name,
                    MinHeight = 20,
                    Width = 200,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(2)

                };
                stackList.Children.Add(text);
            }
            else
            {
                var text = stackList.Children[0] as TextBlock;
                text.Text += ", " + selecaoTag.Name;
            }
            TextBox edit = FindElemento.FindTextBox(stack, tag);
            edit.Text = "";
           
            list.Add(valor);
            taxanomyList[tag] = list;
            
            Button button = FindElemento.FindButton((StackPanel)container.Child, tag);
            if (button != null)
            {
                button.Visibility = Visibility.Visible;
            }

        }

        private async void AddTaxs(string tipo, string taxs)
        {

            taxs = taxs.Replace(";", ",");
            if (tipo== "post_tag")
            {          
                if (taxs.Contains(","))
                {
                    var array = taxs.Split(",");
                    foreach (var item in array)
                    {
                        string nome = item.Trim();
                        await VerificaTag(nome);
                    }
                }
                else
                {
                    await VerificaTag(taxs);
                }
               
            }
            else
            {
                if (taxs.Contains(","))
                {
                    var array = taxs.Split(",");
                    foreach (var item in array)
                    {
                        string nome = item.Trim();
                        Taxonomy tag = new Taxonomy()
                        {
                            Tipo = tipo,
                            Name = nome
                        };
                        AddTax(tag);
                    }

                }
                else
                {
                    Taxonomy tag = new Taxonomy()
                    {
                        Tipo = tipo,
                        Name = taxs
                    };
                    AddTax(tag);
                }   
               
            }

        }

        private async Task<Taxonomy> VerificaTag(string name)
        {
            try
            {
                var param = new Dictionary<string, object>()
                {
                    { "nome", name }
                };
                Taxonomy tag = await apiRestBarear.getTaxonomy("post_tag", param);
                if (tag != null)
                {
                    AddTax(tag);
                    return tag;
                }
                else
                {
                    AlertMensagem alert = AlertMensagem.instance.Aviso($"Essa tag não existe. Deseja cadastrar a tag: {name}?", "Tag não existe",1);
                    if(alert.ShowDialog() == true && alert.result =="sim")
                    {
                        ShowHideLoading(true);
                        CriaTag(name);
                    }
                }
            }
            catch(ApiException ex)
            {
                AlertMensagem.instance.Show(ex.Message,"Erro ao verificar tag");
                Ferramentas.PrintObjeto(ex);
            }
            return null;

        }
        
        private async void CriaTag(string tag)
        {
            try
            {
                string json = "{\"name\":\"" + tag + "\",\"slug\":\"" + Ferramentas.ToUrlSlug(tag) + "\"}";
                var post = await apiRestBarear.CriarTag(json);
                Taxonomy tax = new Taxonomy();
                tax.TermId = post.Id.ToString();
                tax.Name = post.Name;
                tax.Tipo = "post_tag";
                AddTax(tax);
                ShowHideLoading(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CriaTag => " + ex.Message);
                ShowHideLoading(false);
            }
        }

        private void ediTags_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.Enter))
            {
                TextBox textBox = sender as TextBox;
                if (textBox == null) return;
                string tag = textBox.Tag.ToString().Trim();
                string tagB = "bd_" + tag;
                Border container = FindElemento.FindBorder(containerTaxs, tagB);
                if (container == null) return;
                string tagL = "list_" + tag;
                ListBox box = FindElemento.FindListBox((StackPanel)container.Child, tagL);
                if (box == null) return;
                box.SelectedIndex = 0;
            }
            else if (e.Key == Key.Enter)
            {
                TextBox textBox = sender as TextBox;
                if (textBox == null) return;
                string tag = textBox.Tag.ToString().Trim();
                string tagB = "bd_" + tag;
                Border container = FindElemento.FindBorder(containerTaxs, tagB);
                if (container == null) return;
                string tagL = "list_" + tag;
                ListBox box = FindElemento.FindListBox((StackPanel)container.Child, tagL);
                if (box == null) return;
                box.ItemsSource = null;
                AddTaxs(tag, textBox.Text);
            }

        }

        private void ediTags_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var item = sender as TextBox;
                if (item == null) return;
                string tag = item.Tag.ToString();
                string key = item.Text.ToString();
                BuscarTax(tag, key);
            }
            catch {}
        }

        public string getValueTaxs(string nome)
        {
            string tagB = "bd_" + nome;
            Border container = FindElemento.FindBorder(containerTaxs, tagB);

            if (container == null)
            {
                Debug.WriteLine("No container");
                return "";
            }
            StackPanel stack = container.Child as StackPanel;
            if (stack == null)
            {
                Debug.WriteLine("No stack");
                return "";
            }
            string tagL = "sl_" + nome;
            StackPanel stackList = FindElemento.FindStack(stack, tagL);

            if (stackList.Children.Count == 0) return "";
            
            var text = stackList.Children[0] as TextBlock;
            return text.Text;
        }

        private async void BuscarTax(string tag, string palavra)
        {
            try
            {
                string tagB = "bd_" + tag;

                Border container = FindElemento.FindBorder(containerTaxs, tagB);
            
                if(container == null)
                {
                    Debug.WriteLine("No container");
                    return;
                }
                StackPanel stack = container.Child as StackPanel;
                if(stack== null)
                {
                    Debug.WriteLine("No stack");
                    return;
                }
                string tagL = "list_" + tag;
                ListBox list = FindElemento.FindListBox(stack, tagL);
                   
                if (list != null)
                {
                    if (palavra.Length == 0)
                    {
                        list.ItemsSource = null;
                        return;
                    }
                    var dados = new Dictionary<string, object>
                    {
                     { "taxonomy", tag },
                     { "key", palavra }
                    };
                    List<Taxonomy> taxs = await apiRestBarear.buscaTaxonomies(dados);
                    list.ItemsSource = taxs;
                    //list.ItemsSource = taxs.Select(s => s.Name).ToList();
                }
                else
                {
                    Debug.WriteLine("no busca");
                }
            }
            catch { }

        }

        private void setTerms(object terms)
        {
            if (terms == null) return;

            try
            {
                var result = JsonConvert.DeserializeObject<JToken>(terms.ToString());

                foreach (string item in listKeyTax)
                {
                    if(result[item] != null)
                    {
                        AddTax(new Taxonomy()
                        {
                            Tipo = item,
                            Name = result[item].ToString()
                        });
                        if (item != "post_tag")
                        {
                            var lista = result[item].ToString().Split(",").ToList();
                            taxanomyList[item] = lista;
                        }
                    }
                   
                }
            }catch(Exception ex) {
                Debug.WriteLine(ex.Message);
            }


         
        }

        #endregion

        #region CAMPOS

        private void setCampos()
        {
            var camps = configuracoes.getCampos();
            if (camps != null)
            {
                imageContainer.SetCampos(camps);
                foreach (string nome in camps)
                {
                    containerCampos.Children.Add(getBoxCampo(nome));
                    listKeyCampo.Add(nome);
                }
            }

        }

        public void setValueCampo(string nome, string value)
        {
            string tagB = "bd_" + nome;
            Border border = FindElemento.FindBorder(containerCampos, tagB);
            if (border == null) return;

            TextBox element = FindElemento.FindTextBox((StackPanel)border.Child, nome);
            if (element == null) return;
            element.Text = value;
        }

        public string getValueCampo(string nome)
        {
            string tagB = "bd_" + nome;
            Border border = FindElemento.FindBorder(containerCampos, tagB);
            if (border == null) return "";

            TextBox element = FindElemento.FindTextBox((StackPanel)border.Child, nome);
            if (element == null) return "";
            return element.Text;
        }

        private Border getBoxCampo(string nome)
        {
            Border border = new Border()
            {
                Tag = "bd_" + nome,
                Margin = new Thickness(5),
                Background = CorImage.GetCor("#ffffff"),
                Padding = new Thickness(2),
                CornerRadius = new CornerRadius(5)
            };

            StackPanel stack = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(2)

            };

            Grid grid = new Grid();
            grid.Children.Add(new TextBlock()
            {
                Text = Ferramentas.CapitalizeTexto(nome),
                FontSize = 16,
            });

            Button buttonEd = new Button()
            {
                Tag = "edit_"+ nome,
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 20,
                Height = 20,
                Margin = new Thickness(0, 0, 30, 0),
                Padding = new Thickness(0),
                Background = CorImage.Transparente(),
                Visibility = Add ? Visibility.Hidden: Visibility.Visible,
                Content = new ImageAwesome()
                {
                    Foreground = CorImage.GetCorIcone(),
                    Icon = FontAwesomeIcon.Edit,
                }
            };

            buttonEd.Click += btEditCampo_Click;
            grid.Children.Add(buttonEd);

            Button buttonCl = new Button()
            {
                Tag = "del_" + nome,
                HorizontalAlignment = HorizontalAlignment.Right,
                Width = 20,
                Height = 20,
                Padding = new Thickness(0),
                Background = CorImage.Transparente(),
                Visibility = Add ? Visibility.Hidden : Visibility.Visible,
                Content = new ImageAwesome()
                {
                    Foreground = CorImage.GetCorIcone(),
                    Icon = FontAwesomeIcon.WindowClose,
                }
            };
            buttonCl.Click += btClearCampo_Click;
            grid.Children.Add(buttonCl);
            stack.Children.Add(grid);

            TextBox edit = new TextBox()
            {
                Tag = nome,
                TextWrapping = TextWrapping.Wrap,
                MinWidth = 200,
                MinHeight = 50,
                IsEnabled = Add ? true : false,
                Margin = new Thickness(0, 2, 0, 0),
                FontSize = 16,
            };
            stack.Children.Add(edit);
            border.Child = stack;
            return border;
        }

        private void btClearCampo_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            if (item == null) return;
            string tag = item.Tag.ToString().Replace("del_", "");
           
            string tagB = "bd_" + tag; 
            Border border = FindElemento.FindBorder(containerCampos, tagB);
            if (border == null) return;
            
            TextBox element = FindElemento.FindTextBox((StackPanel)border.Child, tag);
            if (element == null) return;
            element.Text = "";
            
            if (!Add && element.IsEnabled == false)
            {
                element.IsEnabled = true;
                ApagarCampo(tag, IDPost);
            }
        }

        private async void ApagarCampo(string nome, long id)
        {
            var request = await apiRestBarear.DeleteCampo(id, nome);
            if (!request.Sucesso)
            {
                MessageBox.Show("Erro ao apagar campo", "Erro");
            }
        }
        
        private void btEditCampo_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            if (item == null) return;
            string tag = item.Tag.ToString().Replace("edit_", "");
            string tagB = "bd_" + tag;
             Border border = FindElemento.FindBorder(containerCampos, tagB);
            if (border == null) return;
            TextBox edit = FindElemento.FindTextBox((StackPanel)border.Child, tag);
            if (edit == null) return;
            if (edit.IsEnabled == false)
            {
                edit.IsEnabled = true;
            }
            else if(!Add)
            {
                edit.IsEnabled = true;
                EditarCampo(tag, IDPost, edit);
            }

        }

        private async void EditarCampo(string nome, long id, TextBox edit)
        {
            var dados = new Dictionary<object, string>
            {
                     { "key", nome },
                     { "value", edit.Text }
            };
            var request = await apiRestBarear.EditarCampo(id, dados);
            if (!request.Sucesso)
            {
                MessageBox.Show("Erro ao editar campo", "Erro");
            }
            else
            {
                edit.IsEnabled = false;
            }
        }

        private void setMetas(List<MetaGetPost> metas)
        {
            if (metas == null || metas.Count == 0) return;
            metas.ForEach(met => {
                string tagB = "bd_" + met.MetaKey;
                Border border = FindElemento.FindBorder(containerCampos, tagB);
                Debug.WriteLine(tagB);
                if (border != null)
                {
                    Debug.WriteLine("bd ok");
                    TextBox element = FindElemento.FindTextBox((StackPanel)border.Child, met.MetaKey);
                    if (element != null)
                    {
                        element.Text = met.MetaValue;
                    }
                }            
            });
        }


        #endregion

        private void SetUpdate(Post post)
        {
            this.IDPost = post.getID;
            Add = false;
            ediTitulo.Texto = post.getTitulo;
            btPublicar.Content = "Atualizar";
            btStory.Visibility = Visibility.Visible;
            MainWindow.BT_POSTAR.Content = "Novo Post";
            Setap();
            getPost(IDPost);
        }

        private void SetUpdate(PostSimples post)
        {
            this.IDPost = post.Id;
            Add = false;
            ediTitulo.Texto = post.Title;
            btPublicar.Content = "Atualizar";
            MainWindow.BT_POSTAR.Content = "Novo Post";
            btStory.Visibility = Visibility.Visible;
            Setap();
            getPost(IDPost);
        }

        private async void TarefasAposPost(bool post) {
            if (statusPost.Equals("future") || statusPost.Equals("draft")) return;
            if(Constants.SITE.Contains(".appmania.com")) return;
            if (Constants.SITE.Contains(".test")) return;
            bdStatusTask.Visibility = Visibility.Visible;
           // tbStatusTask.Text = "Gerando cache do post";
           // await setCachePost();
           // await Task.Delay(100);
            if (post) {
                //tbStatusTask.Text = "Limpando cache categorias";
                //await ClearCacheCategoria();
                try
                {
                    if (imageContainer.PADRAO)
                    {
                        tbStatusTask.Text = "Fazendo upload de imagem";
                        bool upIm = await imageContainer.EnviarImagem();
                        if(!upIm) Mensagem(false, "Erro fazer upload de imagem");
                    }
                   
                    tbStatusTask.Text = "Efetuando pings";
                    bool ping = await restAPI.Pings();
                    if (!ping) Mensagem(false, "Erro fazer ping");
                 
                    if (twitter)
                    {
                        tbStatusTask.Text = "Enviando post ao Twitter";
                        await SendTwitter();
                    }
                    if (facebook)
                    {  
                        tbStatusTask.Text = "Enviando post ao Facebook";
                        await SendFacebook();
                    }                  
                }
                catch(Exception ex)
                {
                    AlertMensagem.instance.Show(ex.Message, "Erro em TarefasAposPost");
                }              
            }

            bdStatusTask.Visibility = Visibility.Collapsed;

        }

        #region TAREFAS
        private async Task setCachePost()
        {
            try
            {
                string s = tbSlugTitle.Text;
                string param = s.Replace(Constants.SITE, "");
                RestAPI restAPI = new RestAPI();
                var cacher = new Cacher();
                cacher.tipo = "post";
                cacher.slug = param;
                RequisicaoBol resultado = await apiRestBasic.ClearChache(cacher);
                if (!resultado.Sucesso)
                {
                    Mensagem(false, "Erro em ClearChachePost");
                    return;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("=> "+ex.Message);
                Mensagem(false, "Exception: ClearChachePost: " + ex.Message);

            }
        }

        private async Task SendTwitter()
        {
            string mensagem = ediTitulo.Texto + " "+tbSlugTitle.Text;
           var send = await mainShare.createTweet(mensagem);
            if (!send) Mensagem(false, "Erro ao enviar tweet");
        }

        private async Task SendFacebook()
        {
            Social social = new Social();
            await social.SendFacebook(ediTitulo.Texto,tbSlugTitle.Text,"");
        }

        private async Task ClearCacheCidade()
        {
            return;
            //string s = tbCidades.Text;
            //var size = s.Split(",");
            //if (size.Length > 9) return;
            //string param = s.Contains(",") ? FormataCidadeSlug(s) : Ferramentas.ToUrlSlug(s);

            //var cacher = new Cacher();
            //cacher.tipo = "concurso";
            //cacher.slug = param;
            //Ferramentas.PrintObjeto(cacher);
           
            //var resultado = await apiRestBasic.ClearChache(cacher);
            //if (!resultado.Sucesso)
            //{
            //    Mensagem(false, "Erro ao limpar cache");
            //}

        }
       
        #endregion

        #region Notificaçao Push
        private async Task getTopicos()
        {
         
        }

        private void CriarBTnoticao(List<Inscritos> list)
        {
            Button botao;
            foreach (var item in list)
            {
                botao = new Button();
                botao.Content = item.Nome;
                botao.Tag = item.Topico;
                botao.Padding = new Thickness(10, 0, 10, 2);
                botao.Margin = new Thickness(5, 0, 5, 0);
                botao.Click += btNotifiSend_Click;
                //  botao.Style = Resources["ButtonStyle1"] as Style;
                stackBotoesMensagem.Children.Add(botao);
            }
            
        }

        private async void btNotifiSend_Click(object sender, RoutedEventArgs e)
        {
            //var botao = (Button)sender;
            //long id = IDPost > 0 ? IDPost : 51954;
            //string corpo = string.IsNullOrEmpty(editImgNotifi.Text) ? ediMetaTitulo.Text : "";
            //InterfaceAPI apiR = RestService.For<InterfaceAPI>("https://fcm.googleapis.com/fcm");
       
            //Notificacao notificacao = new Notificacao() { 
            //    priority = "high", 
            //    to = $"/topics/{botao.Tag}", 
            //    dados = new Dados(id, ediTitulo.Text, corpo, editImgNotifi.Text) 
            //};

            //string json = JsonConvert.SerializeObject(notificacao).ToString();
            //Debug.WriteLine(json);
            //await apiR.SendNotificacao(json);
            //botao.Visibility = Visibility.Collapsed;

        }

        private void stackBotoes_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btCloseNotifi_Click(object sender, RoutedEventArgs e)
        {
            brBotoesNotificacao.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region ENVIAR POST
        private void btPublicar_Click(object sender, RoutedEventArgs e)
        {

            if (ediTitulo.Texto.Length > 30) {
                PublicarAtualizar();
                return;
            }
      
            var mensagem = MessageBox.Show("Titulo muito curto. Deseja publicar assim mesmo?", "Titulo curto - "+ ediTitulo.Texto.Length, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (mensagem == MessageBoxResult.Yes)
            {
                PublicarAtualizar();
            }
        }

        async void PublicarAtualizar()
        {
            try
            {
                await SalvarPost(false);
                if (ValidarForm())
                {
                    ShowHideLoading(true);

                    if (btPublicar.Content.ToString().Equals("Publicar"))
                    {
                        EnviaPost();
                    }
                    else
                    {
                        AtualizarPost();
                    }

                }

            }
            catch (Exception ex)
            {
                ShowHideLoading(false);
                Mensagem(false, ex.Message);
            }
        }

        private async void EnviaPost()
        {
            try
            {

                postAtual = postMotado();
                string json = ModelToJson(postAtual);
                Debug.WriteLine("\n" + json);
                var request = await apiRestBarear.Postar(json);
                Post resultado = JsonConvert.DeserializeObject<Post>(request);
                if (resultado != null && resultado.id > 0)
                {                 
                    Mensagem(true, "Post publicado");
                    btPublicar.Content = "Atualizar";
                    IDPost = resultado.id;
                    tbSlugTitle.Text = resultado.link;
                    MainWindow.BT_POSTAR.IsEnabled = true;
                    MainWindow.BT_POSTAR.Content = "Novo Post";
                    stackVer.Visibility = Visibility.Visible;
                    btStory.Visibility = Visibility.Visible;
                    btSelectPublicar.Visibility = Visibility.Collapsed;
                    stackStatus.Visibility = Visibility.Collapsed;
                    postConteudo = "";
                    dataPost = "";
                    TarefasAposPost(true);
                }
                else
                {
                    Mensagem(false, "Erro ao publicar");
                    var valid = await new RestAPI().IsValidToken();
                    if(valid)
                    {
                        apiRestBarear = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
                        {
                            AuthorizationHeaderValueGetter = () =>
                                Task.FromResult(configuracoes.getToken())
                        });
                    }
                }

            }
            catch (ApiException ex)
            {
                AlertMensagem.instance.Show(ex.Message, "Erro ao publicar");
            }
            catch (Exception ex) {
                Mensagem(false, "Erro ao publicar -> "+ ex.Message);
                Debug.WriteLine(ex.Message);
            }
            ShowHideLoading(false);
        }

        private async void AtualizarPost()
        {
            if (string.IsNullOrEmpty(postConteudo)) return;

            try
            {
                postAtual = postMotado();
                string json = ModelToJson(postAtual);         
                var request = await apiRestBarear.Atualizar(json, IDPost);
                Post resultado = JsonConvert.DeserializeObject<Post>(request);

                if (resultado != null && resultado.id > 0)
                {
                    Mensagem(true, "Post atualizado");
                    //TarefasAposPost(false);
                }
                else
                {
                    Mensagem(false, "Erro ao atualizar");
                    var valid = await new RestAPI().IsValidToken();
                    if (valid)
                    {
                        apiRestBarear = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
                        {
                            AuthorizationHeaderValueGetter = () =>
                                Task.FromResult(configuracoes.getToken())
                        });
                    }
                }
            }
            catch (ApiException ex)
            {
                AlertMensagem.instance.Show(ex.Message, "Erro ao atualizar");
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                Mensagem(false, "Erro ao atualizar -> " + ex.Message);
            }
            ShowHideLoading(false);
        }
        private bool ValidarForm()
        {
            if (string.IsNullOrEmpty(ediTitulo.Texto))
            {
                ediTitulo.Focus();
                Mensagem(false, "Digite titulo");
                return false;
            }
            if (string.IsNullOrEmpty(ediResumo.Texto))
            {
                ediResumo.Focus();
                Mensagem(false, "Digite resumo");
                return false;
            }
            if (string.IsNullOrEmpty(postConteudo))
            {
                Mensagem(false, "Digite conteudo");
                return false;
            }
            if (string.IsNullOrEmpty(CategoriasCheck.CatList))
            {
                Mensagem(false, "Selecione uma categoria");
                return false;
            }

            return true;
        }

        private async void btSalvarPost_Click(object sender, RoutedEventArgs e)
        {
           await SalvarPost();
        }

    
       
        async Task SalvarPost(bool sTela = true)
        {

            try
            {
                bool visual = btHtml.Content.Equals("HTML (H2)") ? true : false;
                postConteudo = await editorPost.getConteudo();
                if (postConteudo != "")
                {
     
                    postAtual = postMotado(true);
                    configuracoes.setSalvoPost(postAtual);
                    if (sTela && editorPost.TIPO==1)
                    {
                        string tela = await editorPost.getTela();
                        if (tela != "")
                        {
                            configLayout["editor_tamanho"] = Convert.ToInt32(tela);
                            configuracoes.setLayout(configLayout);
                        }
                       
                    }
                    Mensagem(true, "Post salvo com sucesso");

                }
                else
                {
                    Mensagem(false, "Erro ao salvar post");
                }
               
            }
            catch(FormatException ex)
            {
                Debug.WriteLine($"FormatException:{ex.Message}");
                Mensagem(false, "Erro ao salvar post");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro em SalvarPost:{ex.Message}");
                Mensagem(false, "Erro ao salvar post");
            }

        }

        private void btHtml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                bool html = btHtml.Content.ToString().Equals("HTML (F2)");
                btHtml.Content = html ? "VISUAL (F2)" : "HTML (F2)";
                editorPost.AlteraViewEditor(html);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        private PostModel postMotado(bool salve = false)
        {
            PostModel postModel = new PostModel();
            var camposWithVal = new Dictionary<string, object>();
            foreach(string camp in listKeyCampo)
            {
                string valor = getValueCampo(camp);
                if (valor != "")
                {
                    camposWithVal.Add(camp, valor);
                }
            }
            if (camposWithVal.Count>0)
            {
                postModel.Campos = camposWithVal;
            }

            postModel.Autor = selecaoUsuario.Id.ToString();
            postModel.Title = ediTitulo.Texto.Trim();
            postModel.Excerpt = ediResumo.Texto.Trim();
            postModel.Content = postConteudo.Trim();
            postModel.Categories = CategoriasCheck.CatList;
            postModel.Status = statusPost;
            // postModel.Date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            postModel.Tags = string.Join(",",taxanomyList["post_tag"].ToArray());
            var taxsWithVal = new Dictionary<string, object>();
            foreach (var dict in taxanomyList)
            {
                if (dict.Value.Count>0)
                {

                    if(salve)
                    {
                       
                        if (dict.Key == "post_tag")
                        {
                            string val = getValueTaxs("post_tag");
                            if(val!="") taxsWithVal.Add(dict.Key, val);
                        }
                        else
                        {
                            string val = string.Join(",", dict.Value.ToArray());
                            taxsWithVal.Add(dict.Key, val);
                        }
                    }
                    else
                    {
                        if (dict.Key != "post_tag")
                        {
                            string val = string.Join(",", dict.Value.ToArray());
                            taxsWithVal.Add(dict.Key, val);
                        }
                    }
                   
                }
            }
            if (taxsWithVal.Count>0)
            {
                postModel.Terms = taxsWithVal;
            }
            if (!string.IsNullOrEmpty(dataPost)) postModel.Date = dataPost.Trim();

            return postModel;
        }


        #endregion

        private string FormataCidadeSlug(string s)
        {
            string formatada = "";
            var araryCidade = s.Split(",");
            foreach (string item in araryCidade) {
                if (string.IsNullOrEmpty(formatada))
                {
                    formatada = Ferramentas.ToUrlSlug(item.Trim());
                }
                else
                {
                    formatada += ","+Ferramentas.ToUrlSlug(item.Trim());
                }
            }
            return formatada;
        }

        
        private bool ValidaMetas()
        {
            //if (!string.IsNullOrEmpty(ediEdital.Text))
            //{
            //    return true;
            //}
            //else if (!string.IsNullOrEmpty(keywords))
            //{
            //    return true;
            //}
            //else if (!string.IsNullOrEmpty(ediImg.Text))
            //{
            //    return true;
            //}
            return false;
        }

       
        
        private string ModelToJson(PostModel data)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            var extraList = JsonConvert.SerializeObject(data, settings).ToString();
            return extraList;
        }


        private async void getPost(long id)
        {        
            try {
            ShowHideLoading(true);
            var post = await apiRestBarear.getPost(id);
                //Debug.WriteLine(Ferramentas.ObjetoToJson(post));

             setPost(post);

             this.Dispatcher.Invoke(new Action(() => {
                    btStory.Visibility = Visibility.Visible;
                    stackVer.Visibility = Visibility.Visible;
                   btSelectPublicar.Visibility = Visibility.Collapsed;
                   stackStatus.Visibility = Visibility.Collapsed;
                    tbSlugTitle.Text = string.Format("{0}/{1}", Constants.SITE, post.Slug);
             }));
               
            }
            catch (Exception ex)
            {
                AlertMensagem.instance.Show("Erro ao pegar post");
                ShowHideLoading(false);
            }
           
        }

        private void setPost(PostView post)
        {
            postConteudo = post.Content;
            postAtual = new PostModel()
            {
                Title = post.Title,
                Excerpt = post.Resumo,

            };
            this.Dispatcher.Invoke(new Action(() => {
                ediTitulo.Texto = post.Title;
                ediResumo.Texto = post.Resumo;
                ShowHideLoading(false);
                if(post.Categoria != null)
                {
                    string[] cats = post.Categoria.Split(",");
                    categoriasCheck.SetCats(cats);
                }
                if (post.Tag != null)
                {
                    if (post.Tag.Contains(","))
                    {
                        var list = post.Tag.Split(",");
                        taxanomyList["post_tag"] = list.ToList();
                    }
                    else
                    {
                        taxanomyList["post_tag"] = new List<string>()
                        {
                            post.Tag
                        };
                    }
                   
                }
                setTerms(post.Terms);
                setMetas(post.Metas);
               if(postConteudo !=null) editorPost.setConteudo(postConteudo.Trim());
                setImagem(post.imagem_destaque);

              //  Debug.WriteLine(JsonConvert.SerializeObject(post));
            
            }));
        }

        private void setImagem(string url)
        {
            if (url != null)
            {
                imageContainer.PADRAO = true;
                imageContainer.SetImageOnline(url);
            }
            else
            {
                if (!imageContainer.PADRAO && imageContainer.CAMPO != "")
                {
                    string valor = getValueCampo(imageContainer.CAMPO);
                    imageContainer.SetImageOnline(valor);
                }
            }
        }

        private void HideScrollbars(WebBrowser wb)
        {
            const string script = "document.body.style.overflow ='hidden'";
            wb.InvokeScript("execScript", new Object[] { script, "JavaScript" });
        }

        private void ShowHideLoading(bool show)
        {
           
            if (show)
            {
                GridFundo.Visibility = Visibility.Visible;
                loadEnviandoDados.IsIndeterminate = true;
                loadEnviandoDados.Visibility = Visibility.Visible;
      
            }
            else
            {
                GridFundo.Visibility = Visibility.Collapsed;
                loadEnviandoDados.IsIndeterminate = false;
                loadEnviandoDados.Visibility = Visibility.Collapsed;
    
            }
        }

        private void comboUser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboUser.SelectedItem != null)
            {
                selecaoUsuario = comboUser.SelectedItem as Usuario;
                configuracoes.setUsuario(selecaoUsuario.Id);
            }
          
        }

        
        private void boxKeywords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ediTitulo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ediTitulo.Texto)) {
            try
            {
                tbSlugTitle.Text = string.Format("{0}/{1}", Constants.SITE, Ferramentas.ToUrlSlug(ediTitulo.Texto));
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            }
        }

        #region MENSAGEM

        private void Mensagem(bool sucesso, string msg)
        {
            mensagemToast.HomeMensagem(sucesso, msg);
        }

        #endregion

       
        private void btClearTags_Click(object sender, RoutedEventArgs e)
        {
            ClearTerms(1);
        }

        private void btClearCargos_Click(object sender, RoutedEventArgs e)
        {
            ClearTerms(2);
        }

        private void btClearCidades_Click(object sender, RoutedEventArgs e)
        {
            ClearTerms(3);
        }

        private void btClearKeyWords_Click(object sender, RoutedEventArgs e)
        {
            ClearTerms(4);
        }

        private void ClearTerms(int id)
        {
            //if (id == 1)
            //{
            //    ediTags.Text = "";
            //    tbTags.Text = "";
            //    tags = "";
            //    listTags.Clear();
            //}
            //else if (id == 2)
            //{
            //    terms.Cargos = null;
            //    ediCargos.Text = "";
            //    tbCargos.Text = "";
            //}
            //else if (id == 3)
            //{
            //    terms.Cidade = null;
            //    ediCidades.Text = "";
            //    tbCidades.Text = "";
            //}
            //else if (id == 4)
            //{
               

            //}
        }

        private void btEditarStatus_Click(object sender, RoutedEventArgs e)
        {
           
 
        }

        private void btRestautarPost_Click(object sender, RoutedEventArgs e)
        {
       
            //return;
            try {
                var postM = configuracoes.getSalvoPost();
                if (postM == null) return;
                var postV = new PostView();
                postV.Content = postM.Content;
                postV.Title = postM.Title;
                postV.Author = postM.Autor;
                postV.Resumo = postM.Excerpt;
                postV.Categoria = postM.Categories;
                postV.Tag = postM.Tags;
                postV.Meta = postM.Campos;
                postV.Metas = postV.metasToMeta();
            
                setMetas(postV.Metas);
             
                if (postM.Terms !=null)
                {
                    postV.Terms = postM.Terms;
                }
                      
                setPost(postV);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            

        }

       

        private void btVerPost_Click(object sender, RoutedEventArgs e)
        {
            string url = $"{Constants.SITE}/?p=" + IDPost;
            WNavegador wNavegador = new WNavegador(url, false);
            wNavegador.ShowDialog();
        }


        private async void ediTags_LostFocus(object sender, RoutedEventArgs e)
        {
            // SalveContente();
            // Send(VirtualKeyCode.F2, VirtualKeyCode.NONAME);
            //  getTopicos();
            //  Debug.WriteLine($"Pai:{Ferramentas.ToUrlSlug(getPai(true))}");
            //  Debug.WriteLine($"Sub:{getPai(false)}");
       
 
        }

        private async void ContentControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {

                await SalvarPost();
            }
            else if (e.Key == Key.F2)
            {
                btHtml_Click(btHtml, null);
            }
            else if (e.Key == Key.F3)
            {
                btBusca_Click(btBusca, null);
            }
            else if (e.Key == Key.F4)
            {
                WUpload pop = new WUpload(true);
              //  pop.ShowDialog();
            }
            else if (e.Key == Key.F5)
            {
                
            }
            else if (e.Key == Key.F6)
            {
                if (!btPublicar.Content.Equals("Publicar"))
                {
                    string url = $"{Constants.SITE}/?p=" + IDPost;
                    WNavegador wNavegador = new WNavegador(url, false);
                    wNavegador.ShowDialog();
                }
          
            }
        }


        private void btBusca_Click(object sender, RoutedEventArgs e)
        {
            WinContainer pop = new WinContainer(new BuscaPosts());
            pop.ShowDialog();
        }


        private void TextBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        private async void btClearCacher_Click(object sender, RoutedEventArgs e)
        {
            await setCachePost();
        }

        #region Atalho Flutuante
        private void btVerHtmlPost_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(postConteudo)) return;
            WNavegador navegador = new WNavegador(postMotado());
            navegador.ShowDialog();

        }

        private void btGerarTkTwitter_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login(2);
            login.ShowDialog();
        }

        private void btGerarToken_Click(object sender, RoutedEventArgs e)
        {
            Login login = new Login(1);
            login.ShowDialog();
        }

        private void brBotoesAcao_MouseEnter(object sender, MouseEventArgs e)
        {
            brBotoesAcao.Opacity = 1;
            stackBtsDesc.Visibility = Visibility.Visible;
        }

        private void brBotoesAcao_MouseLeave(object sender, MouseEventArgs e)
        {
            brBotoesAcao.Opacity = 0.2;
            foreach(TextBlock text in stackBtsDesc.Children)
            {
                text.Visibility = Visibility.Hidden;
            }
            stackBtsDesc.Visibility = Visibility.Collapsed;
        }

        #region BOTOES HIDEN
        private void btHiden_MouseEnter(object sender, MouseEventArgs e)
        {
            Button bt = sender as Button;
            if (bt == null) return;
            string tag = bt.Tag.ToString();
            foreach (TextBlock text in stackBtsDesc.Children)
            {
                text.Visibility = Visibility.Hidden;
            }
            if (tag == "restaure")
            {
                TextBlock view = FindElemento.FindTextBlock(stackBtsDesc, "desc_restaure");
                view.Visibility = Visibility.Visible;
            }
            else
            {
                TextBlock view = FindElemento.FindTextBlock(stackBtsDesc, "desc_view");
                view.Visibility = Visibility.Visible;
            }
        }

        #endregion
        #endregion

        private void btUpload_Click(object sender, RoutedEventArgs e)
        {
           // new WUpload(true).ShowDialog();
        }
  

        private void btStory_Click(object sender, RoutedEventArgs e)
        {
            if (postAtual == null) return;
           // var post = postAtual.toView(IDPost, tbSlugTitle.Text, ediImg.Text);
           // new WinContainer(post).Show();
        }

        private async void btUpdateUsers_Click(object sender, RoutedEventArgs e)
        {
            comboUser.ItemsSource = null;
            var usuarios = await apiRestBarear.getUsuarios();
            List<Usuario> lista = new List<Usuario>();
            foreach (var usario in usuarios)
            {
                lista.Add(new Usuario()
                {
                    Id = usario.id,
                    Nome = usario.name
                });

            }
            if (lista.Count>0)
            {
                configuracoes.setUsuarios(lista);
                comboUser.ItemsSource = lista;
            }
           
        }

        private void Container_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
                imageContainer.SetapImage(files[0], true);
            }
            else if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string draggedFileUrl = (string)e.Data.GetData(DataFormats.Text, false);
                imageContainer.SetapImage(draggedFileUrl, false);
            }
        }

        #region GRID CONTROLLER
  
        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            GridSplitter spliter = (GridSplitter)sender;
            if (spliter == null) return;
            string tag = spliter.Tag.ToString();
            try
            {
                if (tag == "centro")
                {
                    int wd = Convert.ToInt32(colunaCentro.Width.Value);
                    configLayout["centro"] = wd;
                    containerSidebar.Width = wd - 5;
                }
                else
                {
                    configLayout["cats"] = Convert.ToInt32(rowCats.Height.Value);
                    
                }
               
                configuracoes.setLayout(configLayout);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }        
        }


        #endregion



        private void edit_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ediTitulo.Texto.Length > 2)
            {
                tbSlugTitle.Text = Ferramentas.ToUrlSlug(ediTitulo.Texto);
            }
            else
            {
                tbSlugTitle.Text = "";
            }
        }


        #region STATUS POST
        private void btSelectPublicar_Click(object sender, RoutedEventArgs e)
        {
            gridStatus.Visibility = Visibility.Visible;
        }

        private void btStatus_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            if (item == null) return;
            string tag = item.Tag.ToString();
            if(tag== "programar")
            {
                if (stackDataPost.IsVisible)
                {
                    if (ediDataPost.Text == "") return;
                    gridStatus.Visibility = Visibility.Collapsed;
                    stackDataPost.Visibility = Visibility.Collapsed;
                    statusPost = "future";
                    dataPost = Ferramentas.invertData(ediDataPost.Text)+"T"+ediHoraPost.Text;
                    PublicarAtualizar();

                }
                else
                {
                    if (ediDataPost.Text == "")
                    {
                        DateTime dt = DateTime.Now.AddHours(1);
                        ediDataPost.Text = dt.ToString("dd/MM/yyyy");
                        ediHoraPost.Text = dt.ToString("HH:mm:ss");
                    }
                    stackDataPost.Visibility = Visibility.Visible;
                }
            }
            else if (tag == "salvar")
            {
                gridStatus.Visibility = Visibility.Collapsed;
                statusPost = "draft";
                PublicarAtualizar();
            }
            else if (tag == "cancelar")
            {
                gridStatus.Visibility = Visibility.Collapsed;
            }
        }
        #endregion
    }

}
