using Newtonsoft.Json;
using Notification.Wpf;
using PainelPress.Classes.Controller;
using PainelPress.Paginas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PainelPress.Classes
{
    public class TarefasProgramadas
    {
        MainWindow janela;
        CtlDadosSites ctlDados = new CtlDadosSites();
        CtlDadosPagina controllerDados = new CtlDadosPagina();
        public TarefasProgramadas(MainWindow janela) {
            this.janela = janela;
        }
        #region PostsFeeds
        public async Task<bool> getPostsFeeds()
        {
            try
            {

                List<Site> sites = await ctlDados.getSites();
                if (sites.Count > 0)
                {
                    if (LeitorFeed.trabalhandoBack) return true;
                    janela.Dispatcher.Invoke(new Action(async () => {
                        LeitorFeed.sites = sites;
                        LeitorFeed leitor = new LeitorFeed(true);
                        LeitorFeed.trabalhandoBack = true;
                        List<FeedPost> posts = await leitor.getPostsBackground();
                        if (posts.Count > 0)
                        {
                            ShowNotification(posts);
                        }
                        LeitorFeed.trabalhandoBack = false;
                        Debug.WriteLine("Tarefas Feed: {0}", posts.Count);
                    }));
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex) { Debug.WriteLine("getPostsFeeds: " + ex.Message); }
            return true;
        }

        public void ShowNotification(List<FeedPost> posts)
        {
            try
            {
                TimeSpan time = TimeSpan.FromSeconds(30.0);
                var json = JsonConvert.SerializeObject(posts);
                var array = posts.Select(x => x.titulo).Take(6).ToArray();
                var notificationManager = new NotificationManager();
                string mensagem = string.Join("\n\n", array);
                notificationManager.Show($"{posts.Count} novas publicações nos seus feeds", mensagem, NotificationType.Information,"", time, onClick: () => Clique(json));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private void Clique(string posts)
        {
            List<FeedPost> list = JsonConvert.DeserializeObject<List<FeedPost>>(posts);
            new WinContainer(new LeitorFeed(list)).Show();
        }
        #endregion


        #region Paginas

        public async Task<bool> getPaginasUpdate()
        {
            try
            {
                List<Pagina> pages = await controllerDados.getPaginas();
                if (pages.Count == 0)
                {
                    return false;
                }
                janela.Dispatcher.Invoke(new Action(async () => {
                    PaginasSites paginas = new PaginasSites(true);
                    List<Pagina> list = await paginas.VerificaPaginas(pages);
                    if (list.Count > 0)
                    {
                        ShowNotification(list);
                    }
                    Debug.WriteLine("Tarefas Pagina: {0}", list.Count);
                }));
            }
            catch (Exception ex) { Debug.WriteLine("getPaginasUpdate: " + ex.Message); }
            return true;
        }

        public void ShowNotification(List<Pagina> posts)
        {
            try
            {
                TimeSpan time = TimeSpan.FromSeconds(30.0);
                var json = JsonConvert.SerializeObject(posts);
                var array = posts.Select(x => x.descricao).Take(6).ToArray();
                var notificationManager = new NotificationManager();
                string mensagem = string.Join("\n\n", array);
                notificationManager.Show($"{posts.Count} página(s) atualizadas(s)", mensagem, NotificationType.Notification, "", time, onClick: () => CliquePage(json));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private void CliquePage(string posts)
        {
            JanelasControl.Close("acompanhapaginas");
            List<Pagina> list = JsonConvert.DeserializeObject<List<Pagina>>(posts);
            new WinContainer(new PaginasSites(list)).Show();
        }

        #endregion

    }
}
