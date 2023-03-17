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
using config = PainelPress.Properties.Settings;

namespace PainelPress.Paginas
{
    /// <summary>
    /// Interação lógica para Postar.xam
    /// </summary>
    public partial class Postar : ContentControl
    {
        int IDPost;
        bool Add = true;
        string statusPost = "publish";
        string tags = "";
        string keywords = "";
        string dataPost = "";
        public static string postConteudo = "";
        public static TextBox _ediImg;
        Usuario selecaoUsuario;
        Terms terms = new Terms();
        Meta meta = new Meta();
        List<string> listTags = new List<string>();
        List<string> listRusumo = new List<string>();
        List<string> listCidades = new List<string>();
        List<string> listCagos = new List<string>();
        BaseDados baseDados = new BaseDados();
        Taxonomy selecaoTag;
        Taxonomy selecaoCidade;
        Taxonomy selecaoCargo;
        DateTime hoje = DateTime.Now;
        private int timerTickCount = 0;
        DispatcherTimer timer;
        int indexMeta = 0;
        InterfaceAPI apiRestBasic;
        InterfaceAPI apiRestBarear;
        TwitterService twitterService;
        RestAPI restAPI = new RestAPI();
        bool facebook = false;
        bool twitter = false;
     
        public Postar()
        {
            InitializeComponent();
           
            browEditor.LoadUrl($"{Constants.SITE}/editor.html");
            apiRestBasic = RestService.For<InterfaceAPI>(Constants.SITE);
            apiRestBarear = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(config.Default.Token)
            });
            Setap();

        }

        public Postar(Post post)
        {
            InitializeComponent();
            browEditor.LoadUrl($"{Constants.SITE}/editor.html");
            apiRestBasic = RestService.For<InterfaceAPI>(Constants.SITE);
            apiRestBarear = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(config.Default.Token)
            });
            SetUpdate(post);
        }

        public Postar(PostSimples post)
        {
            InitializeComponent();
            browEditor.LoadUrl($"{Constants.SITE}/editor.html");
            apiRestBasic = RestService.For<InterfaceAPI>(Constants.SITE);
            apiRestBarear = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(config.Default.Token)
            });
            SetUpdate(post);
        }

        private void Setap()
        {
            List<Usuario> lista = Usuario.listaUsuarios();
         
            comboUser.ItemsSource = lista;
            int i = 0;
            foreach (var user in lista)
            {
                if(user.Id == config.Default.usuarioAtual)
                {
                    comboUser.SelectedIndex = i;
                }
                i++;
            }
            postConteudo = "";
            twitterService = new TwitterService();

            _ediImg = ediImg;

            Social social = new Social();
            facebook = social.facebook().ativado;
            twitter = social.twitter();
        }

        private void SetUpdate(Post post)
        {
            this.IDPost = post.getID;
            Add = false;
            ediTitulo.Text = post.Title.raw;
            btPublicar.Content = "Atualizar";
            MainWindow.BT_POSTAR.Content = "Novo Post";
           
            Setap();
        }

        private void SetUpdate(PostSimples post)
        {
            this.IDPost = post.Id;
            Add = false;
            ediTitulo.Text = post.Title;
            btPublicar.Content = "Atualizar";
            MainWindow.BT_POSTAR.Content = "Novo Post";

            Setap();
        }

        private async void TarefasAposPost(bool post) {
            if (statusPost.Equals("future")) return;
            bdStatusTask.Visibility = Visibility.Visible;
           // tbStatusTask.Text = "Gerando cache do post";
           // await setCachePost();
           // await Task.Delay(100);
            if (post) {
                
                //if (!string.IsNullOrEmpty(tbCidades.Text))
                //{
                //    tbStatusTask.Text = "Limpando cache cidade";
                //    await ClearCacheCidade();
                //}
                //tbStatusTask.Text = "Limpando cache categorias";
                //await ClearCacheCategoria();
                tbStatusTask.Text = "Efetuando pings";
                bool ping = await restAPI.Pings();
                if (!ping) Mensagem(false, "Erro fazer ping");
                tbStatusTask.Text = "Enviando post ao Twitter";
               if(twitter) await SendTwitter();
                tbStatusTask.Text = "Enviando post ao Facebook";
               if(facebook) SendFacebook();
                            
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
            string mensagem = ediTitulo.Text+" "+tbSlugTitle.Text+" #concurso #vaga";
           var send = await twitterService.PublicarTweetSimples(mensagem);
            if (!send) Mensagem(false, "Erro ao enviar tweet");
        }

        private async void SendFacebook()
        {
            WSocial social = new WSocial();
            await social.SendFacebook(ediTitulo.Text,tbSlugTitle.Text);
        }

        private async Task ClearCacheCidade()
        {    
            string s = tbCidades.Text;
            var size = s.Split(",");
            if (size.Length > 9) return;
            string param = s.Contains(",") ? FormataCidadeSlug(s) : Ferramentas.ToUrlSlug(s);

            var cacher = new Cacher();
            cacher.tipo = "concurso";
            cacher.slug = param;
            Ferramentas.PrintObjeto(cacher);
            return;
            var resultado = await apiRestBasic.ClearChache(cacher);
            if (!resultado.Sucesso)
            {
                Mensagem(false, "Erro ao limpar cache");
            }

        }

        private async Task ClearCacheCategoria()
        {
       
      
            string pai = Ferramentas.ToUrlSlug(getPai(true));
            string subs = getPai(false);
            var cacher = new Cacher();
            cacher.tipo = "categoria";
            cacher.slug = subs;
            Ferramentas.PrintObjeto(cacher);
            return;
            var resultado = await apiRestBasic.ClearChache(cacher);
            if (!resultado.Sucesso)
            {
                Mensagem(false, "Erro ao limpar cacher da categoria");
            }

        }

        private string getPai(bool pai) {
            if (string.IsNullOrEmpty(CategoriasCheck.CatList)) return "no";
            var catList = CategoriasCheck.CatList.Split(",");
            Ferramentas ferramentas = new Ferramentas();
            if (pai) {
                foreach (var cat in catList)
                {
                    if (!cat.Equals("7841"))
                    {
                        if (cat.Equals("54") || cat.Equals("53") || cat.Equals("50") || cat.Equals("52") || cat.Equals("51") || cat.Equals("9"))
                        {

                            return ferramentas.getCatNome(Convert.ToInt32(cat));
                        }
                    }
                }
            }
            else
            {
                List<string> cats = new List<string>();
                foreach (var cat in catList)
                {
                   int c = Convert.ToInt32(cat.Trim());
                   if (c == 7841) continue;
                   bool IsSub = c != 54 && c != 53 && c != 50 && c != 52 && c != 51 && c != 9;
                  
                   if (IsSub) cats.Add(Ferramentas.ToUrlSlug(ferramentas.getCatNome(c)));
                }
                if(cats.Count > 0) return string.Join(",", cats);

            }
            
            return "no";
        }

       
        #endregion

        #region Notificaçao Push
        private async Task getTopicos()
        {
          //  var lista = await apiRest.GetInscritosALL();
           // await baseDados.SalvarTopicos(lista);
 
            if (string.IsNullOrEmpty(CategoriasCheck.CatList)) return;
            // var request = await apiRest.GetInscritos(CategoriasCheck.CatList.Replace("7841,", ""));
            var request = await baseDados.ListaInscritos(CategoriasCheck.CatList.Replace("7841,", ""));
            if (request.Count>0) {
                brBotoesNotificacao.Visibility = Visibility.Visible;
                CriarBTnoticao(request);
            } 
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

        private void btPublicar_Click(object sender, RoutedEventArgs e)
        {

            if (ediTitulo.Text.Length > 30) {
                PublicarAtualizar();
                return;
            }
      
            var mensagem = MessageBox.Show("Titulo muito curto. Deseja publicar assim mesmo?", "Titulo curto - "+ ediTitulo.Text.Length, MessageBoxButton.YesNo, MessageBoxImage.Question);

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
               
                string json = ModelToJson(postMotado());
                Debug.WriteLine("\n" + json);
                var resultado = await apiRestBarear.Postar(json);
                if (resultado != null && resultado.Id > 0)
                {                 
                    Mensagem(true, "Post publicado");
                    btPublicar.Content = "Atualizar";
                    IDPost = resultado.Id;
                    tbSlugTitle.Text = resultado.Link;
                    MainWindow.BT_POSTAR.IsEnabled = true;
                    MainWindow.BT_POSTAR.Content = "Novo Post";
                    stackVer.Visibility = Visibility.Visible;
                    stackStatus.Visibility = Visibility.Collapsed;
                    postConteudo = "";
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
                                Task.FromResult(config.Default.Token)
                        });
                    }
                }
            }
            catch (Exception ex) {
                Mensagem(false, "Erro ao publicar -> "+ ex.Message);
            }
            ShowHideLoading(false);
        }

        private async void AtualizarPost()
        {
            if (string.IsNullOrEmpty(postConteudo)) return;

            try
            {
                string json = ModelToJson(postMotado());
                Debug.WriteLine("\n" + json);
                var resultado = await apiRestBarear.Atualizar(json, IDPost);
                if (resultado != null && resultado.Id > 0)
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
                                Task.FromResult(config.Default.Token)
                        });
                    }
                }
            }catch(Exception ex) {
                Debug.WriteLine(ex.Message);
            }
            ShowHideLoading(false);
        }

        

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

        private bool ValidarForm()
        {
            if (string.IsNullOrEmpty(ediTitulo.Text))
            {
                ediTitulo.Focus();
                Mensagem(false, "Digite titulo");
                return false;
            }
            if (string.IsNullOrEmpty(ediResumo.Text))
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

        private void btSalvarPost_Click(object sender, RoutedEventArgs e)
        {
                SalvarPost(true);
        }

        private async Task GetConteudo()
        {
            var script = @"(function() { return getContent(); })();";
            var task = await browEditor.EvaluateScriptAsync(script);
            dynamic result = task.Result;
            string str = result.ToString() as string;
            postConteudo = str;
            Debug.WriteLine(str);

        }

        private void setConteudo(string valor)
        {
            Debug.WriteLine("setConteudo => " + valor);
            var script = @"(function() { setContent('" + valor.Replace("\n"," ") + "') })();";
            browEditor.ExecuteScriptAsync(script);
            if (!string.IsNullOrEmpty(valor))
            {
               // verificaConteudo();
            }
        }

        private async void verificaConteudo()
        {
            var script = @"(function() { return getContent(); })();";
            var task = await browEditor.EvaluateScriptAsync(script);
            dynamic result = task.Result;
            string str = result.ToString() as string;
         
            if (string.IsNullOrEmpty(str)) {
                setConteudo(postConteudo);
            }
        }

        async Task SalvarPost(bool sTela)
        {
            try
            {
                bool visual = btHtml.Content.Equals("HTML") ? true : false;
                var script = @"(function() { return getContent(); })();";
                var task = await browEditor.EvaluateScriptAsync(script);
                dynamic result = task.Result;
                string str = result.ToString() as string;
                postConteudo = str;
                config.Default.Upgrade();
                config.Default.PostSalvo = ModelToJson(postMotado());
                if (sTela) {
                    script = @"(function() { return document.querySelector('#myeditor_ifr').style.height; })();";
                    task = await browEditor.EvaluateScriptAsync(script);
                    result = task.Result;
                    str = result.ToString() as string;
                    config.Default.SizeEditor = str;
                }
                
                config.Default.Save();
                Debug.WriteLine(config.Default.PostSalvo);
                Mensagem(true, "Post salvo com sucesso");
            }
            catch (Exception ex)
            {
                Mensagem(false, ex.Message);
    
            }
         
        }

        private void btHtml_Click(object sender, RoutedEventArgs e)
        {
            try {
             btHtml.Content = btHtml.Content.ToString().Equals("HTML") ? "VISUAL" : "HTML";
             string script = @"(function() { AlteraEditor() })();";
             browEditor.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        private PostModel postMotado()
        {
            PostModel postModel = new PostModel();

            if (ValidaMetas())
            {
                if(!string.IsNullOrEmpty(ediEdital.Text)) meta.Edital = ediEdital.Text;
                if (!string.IsNullOrEmpty(ediInscricao.Text)) meta.Inscricao = ediInscricao.Text;
                if (!string.IsNullOrEmpty(ediImg.Text)) meta.Imagem = ediImg.Text;
                if (!string.IsNullOrEmpty(ediProvas.Text)) meta.Provas = ediProvas.Text;
                if (!string.IsNullOrEmpty(ediQuestao.Text)) meta.Questoes = ediQuestao.Text;
                if (!string.IsNullOrEmpty(ediSalario.Text)) meta.Salario = ediSalario.Text;
                if (!string.IsNullOrEmpty(ediVagas.Text)) meta.Vagas = ediVagas.Text;
                postModel.Meta = meta;
            }
             
           
            postModel.Autor = selecaoUsuario.Id.ToString();
            postModel.Title = ediTitulo.Text.Trim();
            postModel.Excerpt = ediResumo.Text.Trim();
            postModel.Content = postConteudo.Trim();
            postModel.Categories = CategoriasCheck.CatList;
            postModel.Status = statusPost;
           // postModel.Date = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
            postModel.Tags = tags;
            if (terms.Cidade != null || terms.Cargos != null) { 
                postModel.Terms = terms;
            }
            if (!string.IsNullOrEmpty(dataPost)) postModel.Date = dataPost.Trim();


            return postModel;
        }

        private bool ValidaMetas()
        {
            if (!string.IsNullOrEmpty(ediEdital.Text))
            {
                return true;
            }
            else if (!string.IsNullOrEmpty(keywords))
            {
                return true;
            }
            else if (!string.IsNullOrEmpty(ediImg.Text))
            {
                return true;
            }
            return false;
        }

        private void AddTaxs(bool cidade, string name)
        {
            if (cidade)
            {
                if (name.Contains(","))
                {
                    var array = name.Split(",");
                    foreach(var item in array)
                    {
                        string nome = item.Trim();
                        if (!listCidades.Contains(nome) && item.Length > 2)
                        {
                            listCidades.Add(nome);
                        }
                    }
                 
                }
                else
                {
                    string nome = name.Trim();
                    if (!listCidades.Contains(nome))
                    {
                        listCidades.Add(nome);
                    }
                }

               terms.Cidade = string.Join(", ", listCidades.ToArray());

            }
            else
            {
                if (name.Contains(","))
                {
                    var array = name.Split(",");
                    foreach (var item in array)
                    {
                        string nome = item.Trim();
                        if (!listCagos.Contains(nome) && item.Length > 2)
                        {
                            listCagos.Add(nome);
                        }
                    }

                }
                else
                {
                    string nome = name.Trim();
                    if (!listCagos.Contains(nome))
                    {
                        listCagos.Add(nome);
                    }
                }
                terms.Cargos = string.Join(", ", listCagos.ToArray());
            }
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
  
            var post = await apiRestBasic.getPost(id);
            //Debug.WriteLine(Ferramentas.ObjetoToJson(post));
        
             setPost(post);

             this.Dispatcher.Invoke(new Action(() => {
                    stackVer.Visibility = Visibility.Visible;
                    stackStatus.Visibility = Visibility.Collapsed;
                    tbSlugTitle.Text = string.Format("{0}/{1}", Constants.SITE, post.Slug);
             }));
               
            }
            catch (Exception ex)
            {
                Debug.WriteLine("getPost => "+ex.Message);
            }
           
        }

        private void setPost(PostView post)
        {
            postConteudo = post.Content;

            this.Dispatcher.Invoke(new Action(() => {
                ediTitulo.Text = post.Title;
                ediResumo.Text = post.Resumo;
                loadingEditor.IsIndeterminate = false;
                loadingEditor.Visibility = Visibility.Collapsed;
                browEditor.Visibility = Visibility.Visible;
                if(post.Categoria != null)
                {
                    string[] cats = post.Categoria.Split(",");
                    frameCats.Content = new CategoriasCheck(cats);
                }
                SetTerms(post.Terms.Tag, post.Terms.Cidade, post.Terms.Cargos);

                setMetas(post.Metas);
               
                setConteudo(postConteudo.Trim());
            
            }));
        }

        private void setMetas(List<MetaGetPost> metas)
        {
            if (metas == null || metas.Count == 0) return;
            metas.ForEach(met => {
                if (met.MetaKey == "inscricao")
                {
                    ediInscricao.Text = met.MetaValue;
                }
                else if (met.MetaKey == "edital")
                {
                    ediEdital.Text = met.MetaValue;
                }
                else if (met.MetaKey == "provas")
                {
                    ediProvas.Text = met.MetaValue;
                }
                else if (met.MetaKey == "salario")
                {
                    ediSalario.Text = met.MetaValue;
                }
                else if (met.MetaKey == "vagas")
                {
                    ediVagas.Text = met.MetaValue;
                }
                else if (met.MetaKey == "questoes")
                {
                    ediQuestao.Text = met.MetaValue;
                }
                else if (met.MetaKey == "imagem")
                {
                    ediImg.Text = met.MetaValue;
                }
            });
        }

        private void SetTerms(string tgs, string cidades, string cargos)
        {
            if (tgs == null) return;
            string newtags = tgs.Replace(";", ",");
            string[] tagA = newtags.Split(",");
            if (tagA.Length > 1)
            {
                foreach (string item in tagA)
                {
                    var tagBy = baseDados.getTagById(item);
                    listTags.Add(tagBy.TermId);
                

                    if (string.IsNullOrEmpty(tbTags.Text))
                    {
                        tbTags.Text = tagBy.Name;
                    }
                    else
                    {
                        tbTags.Text += ", " + tagBy.Name;
                    }                 
                }
   
            }
            else
            {
                Debug.WriteLine(tgs);
                var tagBy = baseDados.getTagById(tgs);
                if (tagBy == null) return;
                listTags.Add(tgs);
                tbTags.Text = tagBy.Name;

            }

            tags = string.Join(",", listTags.ToArray());

            terms.Cidade = cidades;
            tbCidades.Text = cidades;
            terms.Cargos = cargos;
            tbCargos.Text = cargos;

           if(cidades != null) listCidades.AddRange(cidades.Split(","));
           if(cargos != null) listCagos.AddRange(cargos.Split(","));
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
                config.Default.Upgrade();
                config.Default.usuarioAtual = selecaoUsuario.Id;
                config.Default.Save();
            }
          
        }

        private void ediTags_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.Enter))
            {
                if (ediTags.IsFocused)
                {
                    boxTags.SelectedIndex = 0;                  
                }
                else if (ediCargos.IsFocused)
                {
                    boxCargos.SelectedIndex = 0;
                }
                else if (ediCidades.IsFocused)
                {
                    boxCidades.SelectedIndex = 0;
                }
               
            }else if (e.Key == Key.Enter)
            {
                if (ediTags.IsFocused)
                {
                    selecaoTag = baseDados.getTag(ediTags.Text);
                    if (selecaoTag != null)
                    {

                    listTags.Add(selecaoTag.TermId);
                    tags = string.Join(",", listTags.ToArray());

                    if (string.IsNullOrEmpty(tbTags.Text))
                    {
                        tbTags.Text = selecaoTag.Name;
                    }
                    else
                    {
                        tbTags.Text += ", " + selecaoTag.Name;
                    }
                    ediTags.Text = "";
                    }
                    else
                    {
                        var questao = MessageBox.Show("Essa tag não existe. Deseja cadastrar?", "Tag não existe", MessageBoxButton.YesNo, MessageBoxImage.Question);

                        if (questao == MessageBoxResult.Yes) {
                            ShowHideLoading(true);
                            CriaTag(ediTags.Text);
                        }
                    }
                }
                else if (ediCargos.IsFocused)
                {
                    AddTaxs(false, ediCargos.Text);
                    tbCargos.Text = terms.Cargos;
                    ediCargos.Text = "";
                }
                else if (ediCidades.IsFocused)
                {
                    AddTaxs(true, ediCidades.Text);
                    tbCidades.Text = terms.Cidade;
                    ediCidades.Text = "";
                }
               
            }
            
        }

        private async void CriaTag(string tag)
        {
            try
            {
                string json = "{\"name\":\""+ tag + "\",\"slug\":\""+Ferramentas.ToUrlSlug(tag)+"\"}";
                Debug.WriteLine(json);
               
                var post = await apiRestBarear.CriarTag(json);
                await baseDados.SalvarTag(post);
                ShowHideLoading(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("CriaTag => " + ex.Message);
                ShowHideLoading(false);
            }
        }

        private void boxTags_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (boxTags.SelectedItem != null)
            {
                selecaoTag = boxTags.SelectedItem as Taxonomy;
                listTags.Add(selecaoTag.TermId);
                tags = string.Join(",", listTags.ToArray());
                
                if (string.IsNullOrEmpty(tbTags.Text))
                {
                    tbTags.Text = selecaoTag.Name;
                }
                else
                {
                    tbTags.Text += ", " + selecaoTag.Name;
                }
                ediTags.Text = "";
                boxTags.ItemsSource = null;
            }
        }

        private void ediTags_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ediTags.IsFocused)
            {
                if (!string.IsNullOrEmpty(ediTags.Text))
                {
                    BuscarTax(Taxonomy.Tag, ediTags.Text.Trim());
                }
                else
                {
                    boxTags.ItemsSource = null;
                }
            }else if (ediCargos.IsFocused)
            {
                if (!string.IsNullOrEmpty(ediCargos.Text))
                {
                    BuscarTax(Taxonomy.Cargos, ediCargos.Text.Trim());
                }
                else
                {
                    boxCargos.ItemsSource = null;
                }
            }
            else if (ediCidades.IsFocused)
            {
                if (!string.IsNullOrEmpty(ediCidades.Text))
                {
                    BuscarTax(Taxonomy.Cidade, ediCidades.Text.Trim());
                }
                else
                {
                    boxCidades.ItemsSource = null;
                }
            }

        }

        private async void BuscarTax(int tipo, string palavra)
        {
            if (tipo == Taxonomy.Tag)
            {
                boxTags.ItemsSource = await baseDados.ListaTaxnomy(palavra, tipo);
            }
            else if (tipo == Taxonomy.Cidade)
            {
                boxCidades.ItemsSource = await baseDados.ListaTaxnomy(palavra, tipo);
            }
            else if (tipo == Taxonomy.Cargos)
            {
                boxCargos.ItemsSource = await baseDados.ListaTaxnomy(palavra, tipo);
            }
        }

        private void boxCargos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (boxCargos.SelectedItem != null)
            {
                selecaoCargo = boxCargos.SelectedItem as Taxonomy;
                AddTaxs(false, selecaoCargo.Name);
                tbCargos.Text = terms.Cargos;
                ediCargos.Text = "";
                boxCargos.ItemsSource = null;
            }
        }

        private void boxCidades_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (boxCidades.SelectedItem != null)
            {
                selecaoCidade = boxCidades.SelectedItem as Taxonomy;
                AddTaxs(true, selecaoCidade.Name);
                tbCidades.Text = terms.Cidade;
                ediCidades.Text = "";
                boxCidades.ItemsSource = null;
            }
        }

        private void boxKeywords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ediTitulo_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(ediTitulo.Text)) {
            try
            {
                tbSlugTitle.Text = string.Format("{0}/{1}", Constants.SITE, Ferramentas.ToUrlSlug(ediTitulo.Text));
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            }
        }

        #region MENSAGEM

        private void Mensagem(bool sucesso, string msg)
        {
            if (sucesso)
            {

                MainWindow.lbMensagem.Content = msg;
                MainWindow.brMensagem.Visibility = Visibility.Visible;
                MainWindow.brMensagem.Background = CorImage.GetCor("#FFB3E237");
            }
            else
            {
                MainWindow.lbMensagem.Content = msg;
                MainWindow.brMensagem.Visibility = Visibility.Visible;
                MainWindow.brMensagem.Background = CorImage.GetCor("#FFF74343");
            }

            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 1); // will 'tick' once every second
                timer.Tick += new EventHandler(Timer_Tick);
                timer.Start();
            }
            else
            {
                timer.Start();
            }
        }

        private void LimparMensagem()
        {
            MainWindow.lbMensagem.Content = "";
            MainWindow.brMensagem.Visibility = Visibility.Collapsed;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            if (++timerTickCount == 2)
            {
                timer.Stop();
                LimparMensagem();
                timerTickCount = 0;
            }
        }

        #endregion


        private void browEditor_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
 
            if (!e.IsLoading) {
                setTela();
                if (Add)
                {
                   this.Dispatcher.Invoke(new Action(() => {
                        loadingEditor.IsIndeterminate = false;
                        loadingEditor.Visibility = Visibility.Collapsed;
                        browEditor.Visibility = Visibility.Visible;
                    }));
                   
                }
                else
                {
                    getPost(IDPost);
                }

            }
        }

        private void setTela()
        {
            string script = string.Format("document.querySelector('#myeditor_ifr').style.height = '{0}'", config.Default.SizeEditor);
            browEditor.ExecuteScriptAsync(script);
        }

      
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
            if (id == 1)
            {
                ediTags.Text = "";
                tbTags.Text = "";
                tags = "";
                listTags.Clear();
            }
            else if (id == 2)
            {
                terms.Cargos = null;
                ediCargos.Text = "";
                tbCargos.Text = "";
            }
            else if (id == 3)
            {
                terms.Cidade = null;
                ediCidades.Text = "";
                tbCidades.Text = "";
            }
            else if (id == 4)
            {
               

            }
        }

        private void btEditarStatus_Click(object sender, RoutedEventArgs e)
        {
            if (btEditarStatus.Content.ToString().Equals("OK"))
            {
                ediStatusPost.IsEnabled = false;
                statusPost = "future";
                dataPost = ediStatusPost.Text;
                btEditarStatus.Content = "Editar";
            }
            else
            {
                ediStatusPost.IsEnabled = true;
                ediStatusPost.Text = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                btEditarStatus.Content = "OK";
            }
 
        }

        private void btRestautarPost_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(config.Default.PostSalvo);

            //return;
            try {
                var postM = JsonConvert.DeserializeObject<PostModel>(config.Default.PostSalvo);
                var postV = new PostView();
                postV.Content = postM.Content;
                postV.Title = postM.Title;
                postV.Author = postM.Autor;
                postV.Resumo = postM.Excerpt;
                postV.Categoria = postM.Categories;
                postV.Tag = postM.Tags;
                postV.Meta = postM.Meta;
                postV.Metas = postV.metasToMeta();
            
                setMetas(postV.Metas);

                if (postM.Terms !=null)
                {
                    postV.Terms = postM.Terms;
                    postV.Terms.Tag = postV.Tag;
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


        private async void SalveContente() {
            try
            {
                var script = @"(function() { return getContent(); })();";
                var task = await browEditor.EvaluateScriptAsync(script);
                if (task.Result != null && !string.IsNullOrEmpty(task.Result.ToString()))
                {
                    dynamic result = task.Result;
                    string str = result.ToString() as string;
                    postConteudo = str;
                    return;
                }
                postConteudo = "";
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

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
                WUpload pop = new WUpload(true);
                pop.ShowDialog();
            }
            else if (e.Key == Key.F2)
            {
                ediEdital.Focus();
            }
            else if (e.Key == Key.F3)
            {
              await SalvarPost(true);
            }
            else if (e.Key == Key.F4)
            {
                try
                {
                    btHtml.Content = btHtml.Content.ToString().Equals("HTML") ? "VISUAL" : "HTML";
                    string script = @"(function() { AlteraEditor() })();";
                    browEditor.ExecuteScriptAsync(script);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            else if (e.Key == Key.F5)
            {
                new WPop(4).ShowDialog();
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

        private void browEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            SalveContente();

        }

        private void btBusca_Click(object sender, RoutedEventArgs e)
        {
            WPop pop = new WPop(4);
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
        }

        private void brBotoesAcao_MouseLeave(object sender, MouseEventArgs e)
        {
            brBotoesAcao.Opacity = 0.2;
        }

        #endregion

        private void btUpload_Click(object sender, RoutedEventArgs e)
        {
            new WUpload(true).ShowDialog();
        }

        private void btEdital_Click(object sender, RoutedEventArgs e)
        {
            ediEdital.Focus();
        }
    }

}
