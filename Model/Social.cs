using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using config = PainelPress.Properties.Settings;

namespace PainelPress.Model
{
    public class Social
    {
        [JsonProperty("ativado")]
        public bool ativado { get; set; }

        [JsonProperty("pagina")]
        public string pagina { get; set; }

        [JsonProperty("token")]
        public string token { get; set; }

        public Social facebook()
        {
            return JsonConvert.DeserializeObject<Social>(config.Default.facebook);
        }

        public bool twitter()
        {
            return config.Default.twitter;
        }

        public void setTwiter(bool ativado)
        {
            config.Default.Upgrade();
            config.Default.twitter = ativado;
            config.Default.Save();

        }
        public void setFacebook(Social social)
        {
            string json = JsonConvert.SerializeObject(social);
            if (json.Length > 5)
            {
                config.Default.Upgrade();
                config.Default.facebook = json;
                config.Default.Save();
            }
        }

        public void sairTwiter()
        {
            config.Default.Upgrade();
            config.Default.credenciais = "";
            config.Default.twitter = false;
            config.Default.Save();
        }

        public string getTwiterCredencial()
        {
            return config.Default.credenciais;
        }
    }
}
