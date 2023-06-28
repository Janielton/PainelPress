using Newtonsoft.Json;
using PainelPress.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FtpLibrary;
using config = PainelPress.Properties.Settings;
using PainelPress.Paginas;

namespace PainelPress.Classes
{
    public class Configuracoes
    {
        #region SET

        public void setSalvoPost(PostModel obj)
        {
            string json = PostToJson(obj);
            if (json != "")
            {
                config.Default.Upgrade();
                config.Default.PostSalvo = json;
                config.Default.Save();
            }
        }

        public void setToken(string value)
        {
            config.Default.Upgrade();
            config.Default.Token = value;
            config.Default.Save();
        }

        public void setLayout(Dictionary<string, int> obj)
        {
            try
            {
                string json = JsonConvert.SerializeObject(obj);
                config.Default.Upgrade();
                config.Default.layout = json;
                config.Default.Save();
            }
            catch { }
        }

        public void setUsuario(int user)
        {
            config.Default.Upgrade();
            config.Default.usuarioAtual = user;
            config.Default.Save();
        }
        public void setUsuarios(List<Usuario> users)
        {
            try
            {
                config.Default.Upgrade();
                config.Default.usuarios = JsonConvert.SerializeObject(users);
                config.Default.Save();
            }
            catch { }
        }
        

        #endregion

        #region GET

        public List<string> getTaxonomies()
        {
            try
            {
                string taxs = config.Default.taxonomys;
                if (taxs=="") return null;
                return JsonConvert.DeserializeObject<List<string>>(taxs);
            }catch { 
                return null; 
            }
        }

        public List<string> getCampos()
        {
            try
            {
                string camps = config.Default.campos;
                if (camps == "") return null;
                return JsonConvert.DeserializeObject<List<string>>(camps);
            }
            catch
            {
                return null;
            }
        }

        public int getUsuario() => config.Default.usuarioAtual;

        public string getToken() => config.Default.Token;


        public PostModel getSalvoPost(){
            try
            {
                string json = config.Default.PostSalvo;
                if (json == "") return null;
                return JsonConvert.DeserializeObject<PostModel>(json);
            }
            catch
            {
                return null;
            }
        }
       
        public Dictionary<string, int> getLayout()
        {
            if (config.Default.layout == "") return null;
            try
            {
                Dictionary<string, int> lay = JsonConvert.DeserializeObject<Dictionary<string, int>>(config.Default.layout);
                return lay;
            }
            catch
            {
                return null;
            }
        
        }

        #endregion


        #region METODOS

        private string PostToJson(PostModel data)
        {
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
                string extraList = JsonConvert.SerializeObject(data, settings).ToString();
                return extraList;
            }
            catch { }
            return "";
        }

        #endregion

    }


}
