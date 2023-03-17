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
using System.Windows.Shapes;
using CefSharp;
using CefSharp.Wpf;
using PainelPress.Classes;
using PainelPress.Model;

namespace PainelPress
{
    /// <summary>
    /// Lógica interna para WNavegador.xaml
    /// </summary>
    public partial class WNavegador : Window
    {
        ChromiumWebBrowser browser;
        PostModel postModel;
        public WNavegador()
        {
            InitializeComponent();
        }

        public WNavegador(string url, bool js)
        {
            InitializeComponent();
            StartNavegador(url, js);
        }

        public WNavegador(PostModel post)
        {
            postModel = post;
            InitializeComponent();
            btMaximizar.Visibility = Visibility.Collapsed;
            btCopyUrl.Visibility = Visibility.Collapsed;
            btAbrirLink.Visibility = Visibility.Collapsed;
            this.Width = 800;
            browser = new ChromiumWebBrowser("about:blank")
            {
                BrowserSettings = { DefaultEncoding = "UTF-8", WebGl = CefState.Disabled }
            };

            stackBrowser.Children.Add(browser);
            if (browser.IsInitialized) {
                browser.LoadingStateChanged += browser_LoadingStateChanged;      
            }
            
        }

        private string getUsuario(string id) {
            foreach (var item in Usuario.listaUsuarios()) {
                if (item.Id.ToString().Equals(id)) return item.Nome;
            }
            return "Nome";
        }

        private void StartNavegador(string url, bool js)
        {
            if (js)
            {
                browser = new ChromiumWebBrowser(url)
                {
                    BrowserSettings = { DefaultEncoding = "UTF-8", WebGl = CefState.Disabled }
                };
                browser.MenuHandler = new CustomMenuWeb();
                browser.LoadingStateChanged += browser_LoadingStateChanged;
                stackBrowser.Children.Add(browser);
            }
            else
            {
                browser = new ChromiumWebBrowser(url)
                {
                    BrowserSettings = { DefaultEncoding = "UTF-8", WebGl = CefState.Disabled, Javascript = CefState.Disabled }
                };
                browser.MenuHandler = new CustomMenuWeb();
                browser.LoadingStateChanged += browser_LoadingStateChanged;
                stackBrowser.Children.Add(browser);
            }

           
        }

        private void browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {

            if (!e.IsLoading)
            {
                this.Dispatcher.Invoke(new Action(() => {
                    progresso.Visibility = Visibility.Collapsed;
                    progresso.IsIndeterminate = false;
                }));
                if (postModel != null) SetHtmlPost();


            }
        }

        private void SetHtmlPost() {
            string html = Properties.Resources.postHtml;
            string post = html.Replace("%titulo%", postModel.Title).Replace("%conteudo%", postModel.Content).Replace("%data%", DateTime.Now.ToString("dd/MM/yyyy HH:ss")).Replace("%usuario%", getUsuario(postModel.Autor));
            browser.LoadHtml(post);
            postModel = null;
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btMaximizar_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
               WindowState = WindowState.Maximized;
            }
            else
            {
               WindowState = WindowState.Normal;
            }
           
        }
        private void Janela_MouseMove(object sender, MouseButtonEventArgs e)
        {
            e.Handled = false;
            try
            {
                DragMove();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        private void btCopyUrl_Click(object sender, RoutedEventArgs e)
        {       
            Debug.WriteLine(browser.Address);
            Clipboard.SetText(browser.Address);
        }

        private void btAbrirLink_Click(object sender, RoutedEventArgs e)
        {           
            try
            {
                var ps = new ProcessStartInfo(browser.Address)
                {
                    UseShellExecute = true
                    
                };
                Process.Start(ps);
            }
            catch (Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }
    }
}
