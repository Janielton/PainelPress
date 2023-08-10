using FontAwesome.WPF;
using PainelPress.Classes;
using PainelPress.Elementos;
using PainelPress.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
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
using System.Xml;
using XamlRadialProgressBar;

namespace PainelPress.Paginas
{
    /// <summary>
    /// Interação lógica para LeitorFeed.xam
    /// </summary>
    public partial class LeitorFeed : ContentControl
    {
        FeedPost postSelecao;
        LeitorRSS leitor = new LeitorRSS();
        CtlDadosSites ctlDados = new CtlDadosSites();
        public static List<Site> sites = new List<Site>();
        public static bool trabalhandoBack = false;
        Site siteSelecao;
        
        public LeitorFeed()
        {
            InitializeComponent();
            _ = getPostsInSites();
        }
        public LeitorFeed(bool background)
        {
            if (!background)
            {
                InitializeComponent();
                _ = getPostsInSites();
            }
        }

        public LeitorFeed(List<FeedPost> posts)
        {
            InitializeComponent();
            bdProgresso.Visibility = Visibility.Collapsed;
            listaItems.ItemsSource = posts;
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

        private async Task<bool> getPostsInSites()
        {
            SetProgresso();
            sites = await ctlDados.getSites();
            listaSites.ItemsSource = sites;
            List<FeedPost> posts = new List<FeedPost>();
            foreach (Site site in sites)
            {
         
                XmlDocument doc = new XmlDocument();
                string xml = await getHtmlALL(site.feed);
                doc.LoadXml(xml);
                if (IsUpdate(doc, site.atualizacao, site.id))
                {
                    List<FeedPost> postsTemp = await leitor.getPosts(site.feed, site.atualizacao);
                    posts.AddRange(postsTemp);
                   
                }
            }
            posts = posts.OrderByDescending(x => x.data).ToList();
            listaItems.ItemsSource = posts;
            SetProgresso(false);
            return true;
        }

        public async Task<List<FeedPost>> getPostsBackground()
        {

            List<FeedPost> posts = new List<FeedPost>();
            foreach (Site site in sites)
            {

                XmlDocument doc = new XmlDocument();
                string xml = await getHtmlALL(site.feed);
                doc.LoadXml(xml);
                if (IsUpdate(doc, site.atualizacao, site.id))
                {
                    List<FeedPost> postsTemp = await leitor.getPosts(site.feed, site.atualizacao);
                    posts.AddRange(postsTemp);

                }
            }
            posts = posts.OrderByDescending(x => x.data).ToList();
            return posts;
        }

        private bool IsUpdate(XmlDocument xml, string at, int id = 0)
        {
            try
            {
            
                XmlNode channel = xml.GetElementsByTagName("channel")[0];
                string data = channel["lastBuildDate"].InnerText;
                bool up = at != data;
                if(up && id>0)
                {
                    ctlDados.UpdateDateSite(id, data);
                }
                return up;
            }
            catch (XmlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        }

        private async Task<string> getHtmlALL(string url)
        {
            var wc = new WebClient();
            wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.85 Safari/537.36");
            wc.Encoding = Encoding.UTF8;
            string pagina = await wc.DownloadStringTaskAsync(new Uri(url));
            return pagina;
        }

        private void listaItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listaItems.SelectedItem == null) return;
            postSelecao = listaItems.SelectedItem as FeedPost;
            if (postSelecao.url.Length > 5)
            {
                new WNavegador(postSelecao.url, true).ShowDialog();
            }
        }
        private void listaSites_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listaSites.SelectedItem == null) return;
            siteSelecao = listaSites.SelectedItem as Site;
            btApagar.IsEnabled = true;
           
        }
        private async void bt_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as Button;
            if (item == null) return;
            string tag = item.Tag.ToString();
            if (tag == "add")
            {
                if (bdFeedAdd.IsVisible)
                {
                    bdFeedAdd.Visibility = Visibility.Collapsed;
                    btAdd.Content = new ImageAwesome()
                    {
                        Icon = FontAwesomeIcon.PlusCircle,
                        Foreground = CorImage.GetCor("#FF88B365"),
                        Width = 30
                    };
                    if (editFeed.Texto.Length > 10)
                    {

                        try
                        {
                            Site site = await leitor.getInfo(editFeed.Texto);
                            if (site != null)
                            {
                                ctlDados.AdicionarSite(site);
                                sites = await ctlDados.getSites();
                            }
                            else
                            {
                                AlertMensagem.instance.Show("Erro ao cadastrar feed");
                            }
                    
                        }
                        catch { }
                    }
     
                }
                else
                {
                    bdFeedAdd.Visibility = Visibility.Visible;
                    btAdd.Content = new ImageAwesome()
                    {
                        Icon = FontAwesomeIcon.Check,
                        Foreground = CorImage.GetCor("#FF88B365"),
                        Width = 30
                    };
                }
            }
            else if(tag == "sites")
            {
                if (bdFeedList.IsVisible)
                {
                    bdFeedList.Visibility = Visibility.Collapsed;
                    btSites.Content = new ImageAwesome()
                    {
                        Icon = FontAwesomeIcon.Sitemap,
                        Foreground = CorImage.GetCor("#FF88B365"),
                        Width = 30
                    };
                    btApagar.IsEnabled = false;

                }
                else
                {
                    bdFeedList.Visibility = Visibility.Visible;
                    btSites.Content = new ImageAwesome()
                    {
                        Icon = FontAwesomeIcon.Close,
                        Foreground = CorImage.GetCor("#FF88B365"),
                        Width = 30
                    };
                }
            }
            else if (tag == "apagar")
            {

            }
            else if (tag == "")
            {
                if (siteSelecao != null)
                {
                    try
                    {
                        ctlDados.DeleteSite(siteSelecao.id);
                    }
                    catch { }
                   
                }
            }
        }


    }

    public class LeitorRSS{
        public async Task<List<FeedPost>> getPosts(string url, string at)
        {
            List<FeedPost> list = new List<FeedPost>();
            FeedPost post;
            try
            {
                XmlDocument doc = new XmlDocument();
                string xml = await getHtmlALL(url);
                doc.LoadXml(xml);
                foreach (XmlNode item in doc.GetElementsByTagName("item"))
                {
                    string data = item["pubDate"].InnerText;
                    if (IsNovo(data,  at)){
                        list.Add(new FeedPost()
                        {
                            titulo = item["title"].InnerText,
                            url = item["link"].InnerText,
                            data = Convert.ToDateTime(data)
                        });
                    }
                  
                }
   
            }
            catch (XmlException ex)
            {
                Debug.WriteLine(ex.Message);
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return list;
        }

        public async Task<Site> getInfo(string url)
        {
       
            try
            {
                XmlDocument doc = new XmlDocument();
                string xml = await getHtmlALL(url);
                doc.LoadXml(xml);
                XmlNode channel = doc.GetElementsByTagName("channel")[0];
                return new Site() { 
                    nome = channel["title"].InnerText,
                    feed = url,
                    url = channel["link"].InnerText,
                    atualizacao = channel["lastBuildDate"].InnerText,
                };
            }
            catch (XmlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return null;
        }

        private bool IsNovo(string dataUrl, string at)
        {
  
            try
            {
                DateTime data = Convert.ToDateTime(dataUrl);
                DateTime dataAt = Convert.ToDateTime(at);
                return data > dataAt;
            }
            catch (XmlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return false;
        } 

        private async Task<string> getHtmlALL(string url)
        {
            var wc = new WebClient();
            wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.85 Safari/537.36");
            wc.Encoding = Encoding.UTF8;
            string pagina = await wc.DownloadStringTaskAsync(new Uri(url));
            return pagina;
        }



        private static string FormatDateFeed(string data)
        {
            string newData;
            if (data.Contains("-"))
            {
                newData = data.Substring(0, data.IndexOf("-")).Trim();
            }
            else if (data.Contains("+"))
            {
                newData = data.Substring(0, data.IndexOf("+")).Trim();
            }
            else
            {
                newData = data;
            }
            return newData;
        }
    }

    public class Site
    {
        public int id { get; set; }
        public string url { get; set; }

        public string nome { get; set; }

        public string feed { get; set; }

        public string atualizacao { get; set; }
    }

    public class FeedPost
    {
        public string titulo { get; set; }

        public DateTime data { get; set; }

        public string url { get; set; }

        public string dataHuman
        {
            get
            {
                return Ferramentas.HumanTime(data);
            }
        }

        public string nome
        {
            get
            {
                foreach (var site in LeitorFeed.sites)
                {
                    if (url.Contains(site.url))
                    {
                        return site.nome;
                    }
                }
                return "";
            }
        }
    }

}
