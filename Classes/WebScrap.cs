using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using PainelPress.Classes.Controller;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using config = PainelPress.Properties.Settings;
using CefSharp.DevTools.IO;
using System.Net.Http;

namespace PainelPress.Classes
{
   public class WebScrap
    {
        public static string HTML_AGORA;

        
        public async Task<string> getPrecoApostila(string url)
        {
            try
            {
                string html = await getHtmlALL(url, false,"GET");
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                var div = htmlDocument.DocumentNode.SelectSingleNode(".//div[@class='col-12 col-xl-9 mt-1']");
                htmlDocument.LoadHtml(div.OuterHtml);
                var valor = htmlDocument.DocumentNode.SelectSingleNode(".//p[@class='special-price 8']");
                if (valor != null)
                {
                    return valor.InnerText.Trim();
                }     
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "PrecoApostila");
            }
            return null;
        }

        #region Metodos

        private void SalvarPosts(int site, string html)
        {
            config.Default.Upgrade();
            if (site == 1)
            {
                config.Default.postsSite1 = html;
            }
            else if (site == 2)
            {
                config.Default.postsSite2 = html;
            }
            else if (site == 3)
            {
                config.Default.postsSite3 = html;
            }
            config.Default.Save();
        }

        public async Task<bool> VerificaHtml(string html, string url, string request, string param)
        {
            try
            {
                HTML_AGORA = await getHtmlALL(url, true, request, param);
                HTML_AGORA = HTML_AGORA.Trim();
                if (string.IsNullOrEmpty(HTML_AGORA)) return false;
                if (Convert.ToInt32(HTML_AGORA) > Convert.ToInt32(html))
                {
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                Debug.WriteLine("VerificaHtml: {0}", ex.Message);
            }
            return false;
        }

        public async Task<string> getHtmlALL(string url, bool length, string request, string param = "")
        {
            try
            {
                var wc = new WebClient();
                wc.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.85 Safari/537.36");
                wc.Encoding = Encoding.UTF8;
                if (request == "GET")
                {
                    string pagina = await wc.DownloadStringTaskAsync(new Uri(url));
                    if (length) return pagina.Length.ToString();
                    return pagina;
                }
                else
                {
                    if (param == "") param = "teste=1";
                    wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                    string pagina = await wc.UploadStringTaskAsync(url, request, param);
                    if (length) return pagina.Length.ToString();
                    return pagina;
                }
            }
            catch(HttpRequestException ex)
            {
                Debug.WriteLine("getHtmlALL: {0}",ex.Message);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("getHtmlALL2: {0}", ex.Message);
            }
            return "0";

        }

        public async Task<string> getTituloPagina(string url)
        {
            try{
                string pagina = await getHtmlALL(url,false, "GET");
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(pagina);
                var titulo = htmlDocument.DocumentNode.SelectSingleNode("//head/title").InnerText;
                return titulo;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return "Titulo Indisponivel";
            }
        }

        #endregion


    }
}
