using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using config = PainelPress.Properties.Settings;

namespace PainelPress.Classes
{
    public class RestAPI
    {

        private readonly string Base = Constants.SITE;
        
        public async Task<string> POST(string url, string dados) {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                if (dados != null)
                {
                    byte[] dataStream = Encoding.UTF8.GetBytes(dados);
                    request.ContentLength = dataStream.Length;
                    Stream newStream = request.GetRequestStream();
                    newStream.Write(dataStream, 0, dataStream.Length);
                    newStream.Close();
                }
                using (var response = await request.GetResponseAsync())
                {
                    using (var stream = response.GetResponseStream())
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                return await reader.ReadToEndAsync();
                            }
                }
                return null;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("POST => "+ex.Message);
            }
            return null;
        }

        public async Task<string> GET(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(Base+"/"+url);
                var myHttpWebRequest = (HttpWebRequest)request;
                myHttpWebRequest.PreAuthenticate = true;
                myHttpWebRequest.Headers.Add("Authorization", "Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=");
                request.Method = "GET";
                var response = await request.GetResponseAsync();
               
                using (var stream = response.GetResponseStream())
                if (stream != null){
                    using (var reader = new StreamReader(stream))
                    {
                       return await reader.ReadToEndAsync();
                    }
                }
                             
                return null;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("GET => " + ex.Message);
            }
            return null;
        }

        public async Task<byte[]> BaixarImagem(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                using (WebResponse response = await request.GetResponseAsync())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        MemoryStream ms = new MemoryStream();
                        stream.CopyTo(ms);
                        byte[] data = ms.ToArray();
                        ms.Close();
                        return data;
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("BaixarImagem => " + ex.Message);
            }
            return null;
        }

        public async Task<byte[]> BaixarDados(string url)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    var data = await client.DownloadDataTaskAsync(new Uri(url));
                    return data;
                }  
            }
            catch (Exception ex)
            {
                Debug.WriteLine("BaixarImagem => " + ex.Message);
            }
            return null;
        }

        public async Task<RequisicaoBol> UploadImagemStrem(string url, byte[] fileStream)
        {
          //  var bytes = Convert.FromBase64String(fileStream);
            var contents = new StreamContent(new MemoryStream(fileStream));
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=");

            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(contents, "file", "file");
                var response = await client.PostAsync(url, formData);
                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }
                string responseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<RequisicaoBol>(responseJson); ;
            }

        }

        public async Task<bool> Pings()
        {
            string data = DateTime.Now.ToString("yyyy-MM");
            string[] urls = {"https://pubsubhubbub.appspot.com", "https://www.google.com/webmasters/tools/ping?sitemap=https://www.confiraconcursos.com.br/sitemap-pt-post-"+data };

            try
            {
                foreach (string url in urls)
                {
                    bool news = url.Contains("pubsubhubbub");
                    WebRequest request = WebRequest.Create(url);
                    if (news) {
                        string dados = $"hub.mode=publish&hub.url=https://www.confiraconcursos.com.br/noticias?feedgn=news";
                        byte[] dataStream = Encoding.UTF8.GetBytes(dados);
                        request.Method = "POST";
                        request.ContentType = "application/x-www-form-urlencoded";
                        request.ContentLength = dataStream.Length;
                        Stream newStream = request.GetRequestStream();
                        newStream.Write(dataStream, 0, dataStream.Length);
                        newStream.Close();
                    }
                    else
                    {
                       request.Method = "GET";
                    }
                    
                    await request.GetResponseAsync();
                }
               
                return true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Pings => " + ex.Message);
                return false;
            }

        }

        public async Task<bool> IsValidToken()
        {
            var retri = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(config.Default.Token)
            });

            try
            {
                var resultado = await retri.VerificaToken();

                if (resultado.Data.Status != 200)
                {
                    Login login = new Login(1);
                    return (bool)login.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Login login = new Login(1);
                return (bool)login.ShowDialog();
            }
            return false;

        }

    }
}
