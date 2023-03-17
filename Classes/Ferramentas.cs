using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;
using Newtonsoft.Json;
using PainelPress.Model;
using config = PainelPress.Properties.Settings;

namespace PainelPress.Classes
{
    public class Ferramentas
    {
        public static string urlUplod = Constants.SITE+ "/api?rota=upload";
        private static readonly Encoding encoding = Encoding.UTF8;
        public static string ToUrlSlug(string value)
        {

            //First to lower case
            value = value.ToLowerInvariant();

            //Remove all accents
           // var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(value);
            value = RemoveDiacritics(value);

            value = Regex.Replace(value, @"[/]", "-", RegexOptions.Compiled);

            //Replace spaces
            value = Regex.Replace(value, @"\s", "-", RegexOptions.Compiled);

            //Remove invalid chars
            value = Regex.Replace(value, @"[^a-z0-9\s-_]", "", RegexOptions.Compiled);

            //Trim dashes from end
            value = value.Trim('-', '_');

            //Replace double occurences of - or _
            value = Regex.Replace(value, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return value;
        }

        static string RemoveDiacritics(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
        
        public async Task<string> UploadBrasao(string param, byte[] img)
        {
     
            string formDataBoundary = string.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;
            var responseStream = await RestAPI(param, "PUT", contentType, img);
            var reader = new StreamReader(responseStream.GetResponseStream(), encoding);
           
            return reader.ReadToEnd();
 
        }

        public void TocarAlerta(int tipo)
        {
            if (tipo == 1)
            {
                System.Media.SystemSounds.Asterisk.Play();
            } 
            else if (tipo == 2) {
                System.Media.SystemSounds.Beep.Play();
            }
            else if (tipo == 3)
            {
                System.Media.SystemSounds.Exclamation.Play();
            }
            else if (tipo == 4)
            {
                System.Media.SystemSounds.Hand.Play();
            }
            else if (tipo == 5)
            {
                System.Media.SystemSounds.Question.Play();
            }
        }

        public async Task<string> UploadImagem(string param, byte[] img)
        {
      
            string formDataBoundary = string.Format("----------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;
            var responseStream = await RestAPI(param, "POST", contentType, img);
            var reader = new StreamReader(responseStream.GetResponseStream(), encoding);
            
            return reader.ReadToEnd();

        }

        public static void PrintObjeto(object obj) {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(obj))
            {
                string name = descriptor.Name;
                object value = descriptor.GetValue(obj);
                Debug.WriteLine("{0}={1}", name, value);
            }
        }

        private async Task<WebResponse> RestAPI(string param, string tipo, string contentType, byte[] formData)
        {
            string url = urlUplod + "&" + param;
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
           Debug.WriteLine(url);
            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            // Set up the request properties.
            request.Method = tipo;
            request.ContentType = contentType;
            request.UserAgent = "Firefox";
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            // You could add authentication here as well if needed:
            // request.PreAuthenticate = true;
            // request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.MutualAuthRequested;
            request.Headers.Add("Authorization", "Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=");

            // Send the form data to the request.
            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }
            var result = await request.GetResponseAsync();

            return result;
        }

        private async Task<WebResponse> RestAPI(string url, string tipo)
        {
         
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
 
            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            // Set up the request properties.
            request.Method = tipo;
            request.UserAgent = "Firefox";

            return await request.GetResponseAsync();
        }

        public static string ObjetoToJson(object ojeto) {
            var jsonTolist = JsonConvert.SerializeObject(ojeto);
            return jsonTolist;
        }

        public static object JsonToObjeto(string json, int tipo)
        {
            var jsonTolist = new object();
            if (tipo == 1)
            {
                jsonTolist = JsonConvert.DeserializeObject<ERegistro>(json);
            }
            else if (tipo == 2) {
                jsonTolist = JsonConvert.DeserializeObject<List<Categoria>>(json);
            }
            return jsonTolist;
        }

        public async Task<string> LerFeed(string url)
        {
            var request = await RestAPI(url, "GET");
            string feedContents = "";
            using (var reader = new StreamReader(request.GetResponseStream()))
            {
                 feedContents = reader.ReadToEnd();
                
            }

            var document = XDocument.Parse(feedContents);

            var posts = (from p in document.Descendants("item")
                         select new
                         {
                             Title = p.Element("title").Value,
                             Link = p.Element("link").Value,
                             Description = p.Element("description").Value,
                             PubDate =
                             DateTime.Parse(p.Element("pubDate").Value)
                         }).ToList();
            foreach (var post in posts)
            {
                Debug.WriteLine(post.Title);
                Debug.WriteLine(post.Link);
                Debug.WriteLine(post.Description);
                Debug.WriteLine(post.PubDate);
            }
            return url;
        }

        public string getCatNome(int id) {
            return "None";
        }


        public async Task<bool> RegistrarLog(string log) {
            string file = @"C:\WPanel\logs_painel.txt";   
            try {
                if (!File.Exists(file))
                {
                    File.Create(file);
                }
                string _log = $"{log} - {DateTime.Now}{Environment.NewLine}";
                //Thread thread = new Thread(async () => await ThreadProc(file, _log));
                //thread.Start();
                await File.AppendAllTextAsync(file, _log);
            }
            catch (Exception ex) {
                Debug.WriteLine(ex.Message);
                return false;
            }    
            return true;
        }

        
        public async Task ThreadProc(string file, string _log)
        {
            await File.AppendAllTextAsync(file, _log);
        }

        public void GetXamlCats(List<Categoria> categorias) {
            Debug.WriteLine("\n==============\n");
            string categoria = "";
            foreach (var cat in categorias)
            {
                if (HasChild(cat.CatID))
                {
                    categoria += StackComSubs(cat);
                }   
            }
            Debug.WriteLine(string.Format("<StackPanel Orientation=\"Vertical\">{0}</StackPanel>", categoria));
            Debug.WriteLine("\n==============\n");
            bool HasChild(int id)
            {
                bool rs = false;
                categorias.ForEach(c =>
                {
                    if (c.Parent == id)
                    {
                        rs = true;
                    }
                });
                return rs;
            }
            string CategoriaBox(Categoria item)
            {
                string template = "<CheckBox Style=\"{DynamicResource CheckBoxStyleSub}\" Checked=\"CheckBox_Checked\" Unchecked=\"CheckBox_Unchecked\" Content=\"{Nome}\" Tag=\"{ID}\" x:Name=\"ct{ID}\" FontSize =\"16\"/>";

                return template.Replace("{Nome}", item.Name).Replace("{ID}", item.CatID.ToString());
            }

             string listToBox(List<Categoria> list)
            {
                string st = "";
                list.ForEach(c =>
                {
                    st += CategoriaBox(c);
                });
                return st;
            }

            string StackComSubs(Categoria item)
            {
                string template = "<StackPanel Orientation=\"Vertical\"><CheckBox Style=\"{DynamicResource CheckBoxStylePai}\" Checked=\"CheckBox_Checked\" Unchecked=\"CheckBox_Unchecked\" Content=\"{Nome}\" Tag=\"{ID}\" x:Name=\"ct{ID}\" FontSize =\"20\" Margin=\"0,0,0,2\"/><StackPanel Orientation=\"Vertical\" Background=\"WhiteSmoke\" Visibility=\"Collapsed\" x:Name=\"stack{slug}\">{ListaCheckBox}</StackPanel></StackPanel>";

                string list = listToBox(getItemsParents(item.CatID));

                return template
                    .Replace("{Nome}", item.Name)
                    .Replace("{ID}", item.CatID.ToString())
                    .Replace("{slug}", item.Slug.Replace("-",""))
                    .Replace("{ListaCheckBox}", list);
            }

            List<Categoria> getItemsParents(int parent)
            {
                List<Categoria> list = new List<Categoria>();
                categorias.ForEach(c =>
                {
                    if (c.Parent == parent)
                    {
                        list.Add(c);
                    }
                });
                return list;
            }
        }

    }
}
