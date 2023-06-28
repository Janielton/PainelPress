using CefSharp;
using HarfBuzzSharp;
using OxyPlot;
using PainelPress.Classes;
using PainelPress.Elementos;
using PainelPress.Model;
using System;
using System.Collections.Generic;
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
    /// Interação lógica para Editor.xam
    /// </summary>
    public partial class Editor : ContentControl
    {
        public int TIPO = 1;
        bool Add = true;
        private string postConteudo = "";
        private bool carregado = false;
        static int sizeTela = 0;
        public Editor(int t = 1, bool ad = true)
        {
            InitializeComponent();
            TIPO = t;
            Add = ad;
            if (TIPO == 1)
            {
                // browEditor.Visibility = Visibility.Visible;
                if(Add) ShowHideLoading(true);
                browEditor.LoadUrl($"{Constants.SITE}/editor");
            }
            else
            {
                sharpEditor.Visibility = Visibility.Visible;
                MyEditor myeditor = new MyEditor();
                sharpEditor.Children.Add(myeditor);
            }
        }

        public void setTela(int sz)
        {
            sizeTela = sz;
        }

        #region BROWSER EDITOR
        public async Task<string> getConteudo()
        {
            var script = @"(function() { return getContent(); })();";
            var task = await browEditor.EvaluateScriptAsync(script);
            dynamic result = task.Result;
            string str = result.ToString() as string;
            return str;
        }

        public async Task<string> getTela()
        {
           string script = @"(function() { return document.querySelector('#myeditor_ifr').style.height; })();";
            var task = await ExecuteJSReturn(script);
            dynamic result = task.Result;
            string str = result.ToString();
            return Convert.ToString(str).Replace("px", "");
        }

        public void setConteudo(string valor)
        {
            // Debug.WriteLine("setConteudo => " + valor);
            postConteudo = valor;
            var script = @"(function() { setContent('" + valor.Replace("\n", " ").Replace("\r", " ") + "') })();";
            ExecuteJS(script);
            if (!string.IsNullOrEmpty(valor))
            {
                // verificaConteudo();
            }
        }

        public async void verificaConteudo()
        {
            var script = @"(function() { return getContent(); })();";
            var task = await browEditor.EvaluateScriptAsync(script);
            dynamic result = task.Result;
            string str = result.ToString() as string;

            if (string.IsNullOrEmpty(str))
            {
               // setConteudo(postConteudo);
            }
        }


        private void browEditor_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
   
            if (TIPO == 1 && !e.IsLoading)
            {
                setTela();
                this.Dispatcher.Invoke(new Action(() => {
                    ShowHideLoading(false);
                    browEditor.Visibility = Visibility.Visible;
                    carregado = true;
                    if (postConteudo != "")
                    {
                        setConteudo(postConteudo);
                    }
                }));
            }
        }
        private void setTela()
        {
            try
            {
                if (sizeTela > 0)
                {
                    string script = string.Format("document.querySelector('#myeditor_ifr').style.height = '{0}px'", sizeTela);
                    ExecuteJS(script);
                }
            }
            catch { }

        }
        private void ShowHideLoading(bool show)
        {

            if (show)
            {
                loading.Visibility = Visibility.Visible;
            }
            else
            {
                loading.Visibility = Visibility.Collapsed;
               // loading.Children.Clear();
            }
        }
        private async void browEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            
            postConteudo = await getConteudo();

        }

        public void ExecuteJS(string script)
        {
            browEditor.ExecuteScriptAsync(script);
        }
        public async Task<JavascriptResponse> ExecuteJSReturn(string script)
        {
            return await browEditor.EvaluateScriptAsync(script);
        }

        #endregion

    }
}
