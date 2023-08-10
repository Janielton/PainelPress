using Newtonsoft.Json;
using Notification.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
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
using PainelPress.Classes;
using PainelPress.Classes.Controller;
using PainelPress.Model;
using XamlRadialProgressBar;
using PainelPress.Elementos;

namespace PainelPress.Paginas
{
    /// <summary>
    /// Interação lógica para PaginasSites.xam
    /// </summary>
    public partial class PaginasSites : ContentControl
    {
        Configuracoes conf = new Configuracoes();
        CtlDadosPagina controllerDados;
        Pagina siteSelecao;
        WebScrap webScrap;
        public List<Pagina> listaAtual = new List<Pagina>();
        List<string> erros = new List<string>();
        public static bool Verificando = false;
        bool background = false;
        string request = "GET";
        
        public PaginasSites()
        {
            InitializeComponent();
            webScrap = new WebScrap();
            controllerDados = new CtlDadosPagina();
            LoadPaginas();
        }

        public PaginasSites(bool tarefa)
        {
            webScrap = new WebScrap();
        }

        public PaginasSites(List<Pagina> list)
        {
            InitializeComponent();
            webScrap = new WebScrap();
            controllerDados = new CtlDadosPagina();
            LoadPaginas(list);
        }


        private async void LoadPaginas()
        {
            ResetForm();
            progresso.Visibility = Visibility.Visible;
            listaAtual = await controllerDados.getPaginas();
            listaItems.ItemsSource = listaAtual;
            progresso.Visibility = Visibility.Collapsed;
        }

        private async void LoadPaginas(List<Pagina> list)
        {
            btVerificar.Content = "Confirmar";
            listaItems.ItemsSource = list;
            listaAtual = list;
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

        private void ResetForm()
        {
            ediNome.Texto = "";
            ediUrl.Texto = "";
            editRequest.Texto = "";
            btAdicionar.Content = "Adicionar";
        }
        private void listaItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listaItems.SelectedItem == null) return;
            siteSelecao = listaItems.SelectedItem as Pagina;
        }
       

        private void btDelete_Click(object sender, RoutedEventArgs e)
        {
            if(siteSelecao!=null)
            {
                controllerDados.DeletePagina(siteSelecao.id_pagina);
            }
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            ResetForm();

        }

        private void btAdicionar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(ediNome.Texto) || string.IsNullOrEmpty(ediUrl.Texto)) return;
            if (btAdicionar.Content.Equals("Adicionar"))
            {
                Salvar(ediUrl.Texto);
            }
            else
            {
                if (siteSelecao == null) return;
                Salvar(ediUrl.Texto, siteSelecao.id_pagina);
            }
          
        }

        private async void Salvar(string url,int id=0)
        {
            Pagina page = getPagina();
            bool acao;
            if (id == 0)
            {
                acao = await controllerDados.AdicionarPagina(page);
            }
            else
            {
                page.id_pagina = id;
                acao = await controllerDados.UpdatePagina(page);
            }
            if(acao)
            {
                LoadPaginas();
            }
            else
            {
                AlertMensagem.instance.Show("Erro ao adicionar pagina");
            }
    
        }

        private Pagina getPagina()
        {

            return new Pagina
            {
                descricao = ediNome.Texto,
                url = ediUrl.Texto,
                request = request,
                html = "0",
                parametro= editRequest.Texto
            };
        }

        private void listaItems_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (siteSelecao == null) return;
            Ferramentas.OpenUrl(siteSelecao.url);
        }

        private async void btVerificar_Click(object sender, RoutedEventArgs e)
        {
            if (btVerificar.Content.Equals("Verificar"))
            {
                if (listaAtual.Count == 0) return;
                progresso.Visibility = Visibility.Visible;
                progresso.IsIndeterminate = false;
                BackgroundWorker worker = new BackgroundWorker();
                worker.RunWorkerCompleted += worker_RunWorkerCompleted;
                worker.WorkerReportsProgress = true;
                worker.DoWork += worker_DoWork;
                worker.ProgressChanged += worker_ProgressChanged;
                worker.RunWorkerAsync(argument: JsonConvert.SerializeObject(listaAtual));
            }
            else
            {
                SetProgresso();
                var acao = await controllerDados.AtualizarHtml(listaAtual);
                if (acao)
                {
                    LoadPaginas();
                    btVerificar.Content = "Verificar";
                }
                else
                {
                    AlertMensagem.instance.Show("Erro ao atualizar html");
                }
                SetProgresso(false);

            }
           // progressText.Visibility = Visibility.Visible;

        }

        public async Task<List<Pagina>> VerificaPaginas(List<Pagina> pages)
        {
            
            listaAtual = pages;
            var lista = new List<Pagina>();
            background = true;
            try
            {
     
                foreach (var item in pages)
                {
                    try
                    {
                        if (await webScrap.VerificaHtml(item.html, item.url, item.request, item.parametro))
                        {
                            item.html = WebScrap.HTML_AGORA;
                            lista.Add(item);
                        }
                    }

                    catch (Exception ex)
                    {
                        Debug.WriteLine("VerificaPaginas: {0}",ex.Message);
                        erros.Add(item.descricao);
                        continue;
                    }
                   await Task.Delay(10);
                }
            }
            catch { MessageBox.Show("Erro ao verificar paginas"); }
            background = false;
            return lista;
        }


        #region TRABALHO BACKGROUND

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progresso.Value = e.ProgressPercentage;
            //  progressText.Text = (string)e.UserState;
            //  tbOrganizadoraAtual.Text = OrgAtual;
            //  tbQuandidade.Text = ListaOrganizadora.Count.ToString();
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            Verificando = true;
            string json = (string)e.Argument;
            var worker = sender as BackgroundWorker;
            var list = JsonConvert.DeserializeObject<List<Pagina>>(json);
            worker.ReportProgress(0, "Carregando...");
            if (list.Count <= 0)
            {
                worker.ReportProgress(100, "Carregado 100%");
                return;
            }
            var lista = new List<Pagina>();
            int porcent = 0;
            int size = 100 / list.Count;
            foreach (var item in list)
            {
                try
                {
                    worker.ReportProgress(porcent, string.Format("Carregando {0}%", porcent));
                    porcent += size;
                    if (webScrap.VerificaHtml(item.html, item.url, item.request, item.parametro).Result)
                    {
                        item.html = WebScrap.HTML_AGORA;
                        lista.Add(item);
                    }
                }
                catch (Exception ex)
                {
                    erros.Add(item.descricao);
                    continue;
                }
                Task.Delay(10);
            }
            e.Result = lista;
            worker.ReportProgress(100, "Carregado 100%");
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var list = e.Result as List<Pagina>;
            if (background)
            {
                ShowPaginasInList(list);
                Verificando = false;
                return;
            }

            progresso.Visibility = Visibility.Collapsed;

            if (list.Count > 0)
            {
                LoadPaginas(list);
            }
            else
            {
                ShowMensagem(false, "Nenhuma alteração");
                listaItems.ItemsSource = null;
            }
            Verificando = false;

        }

        #endregion


        private void ShowMensagem(bool sucesso, string mensagem)
        {

            var notificationManager = new NotificationManager();
            if (sucesso)
            {
                notificationManager.Show("Alerta", mensagem, NotificationType.Success, onClick: () => Chamar("ok"));
            }
            else
            {
                notificationManager.Show("Alerta", mensagem, NotificationType.Error);
            }
            
        }

        private void Chamar(string param)
        {
            App.Current.MainWindow.Focus();
            Debug.WriteLine(param);
        }

        private void ShowPaginasInList(List<Pagina> list)
        {
            try
            {
                if (list.Count > 0)
                {
                    WinContainer winContainer = new WinContainer(new PaginasSites(list));
                    winContainer.Show();
                }
            }
            catch { }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            if (item == null) return;
            InterpletClick(item.Tag.ToString());
        }

        private async void InterpletClick(string tag)
        {
            if (tag == "editar")
            {
                if (siteSelecao != null)
                {
                    btAdicionar.Content = "Atualizar";
                    ediNome.Texto = siteSelecao.descricao;
                    ediUrl.Texto = siteSelecao.url;
                }
            }
            else if (tag == "delete")
            {
                if (siteSelecao != null)
                {
                    controllerDados.DeletePagina(siteSelecao.id_pagina);
                    LoadPaginas();
                }
            }
            else if (tag == "view")
            {
                if (siteSelecao != null)
                {
                    Ferramentas.OpenUrl(siteSelecao.url);
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rd = sender as RadioButton;
            if(rd == null || editRequest==null) return;
            string tag = rd.Tag.ToString();
            if (tag == "post")
            {
                request = "POST";
                editRequest.Visibility = Visibility.Visible;
            }
            else
            {
                request = "GET";
                editRequest.Visibility = Visibility.Hidden;
            }
        }

        private void editBusca_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (editBusca.Texto.Length>0)
            {
                buscar(editBusca.Texto);
            }

        }

        private async void buscar(string s)
        {

            listaAtual = await controllerDados.getPaginas(s);
            listaItems.ItemsSource = listaAtual;
        }
    }
}
