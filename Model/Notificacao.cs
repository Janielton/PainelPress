using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PainelPress.Model
{

    public class Notificacao
    {
      
        [JsonProperty("data")]
        public Dados dados { get; set; }

        [JsonProperty("to")]
        public string to { get; set; }

        [JsonProperty("priority")]
        public string priority { get; set; }
       
    }

    public class Dados
    {
        public Dados(long id, string titulo, string corpo, string img)
        {
            Id = id;
            Titulo = titulo;
            Corpo = corpo;
            Img = img;
        }
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("titulo")]
        public string Titulo { get; set; }

        [JsonProperty("corpo")]
        public string Corpo { get; set; }

        [JsonProperty("img")]
        public string Img { get; set; }
    }
}
