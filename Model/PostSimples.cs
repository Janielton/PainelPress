using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PainelPress.Model
{
    public class PostSimples
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        [JsonProperty("date")]
        public DateTime? Date { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public string getData
        {
            get
            {  
               if (Date == null) return "";
               return Date?.ToString("dd/MM/yyyy HH:mm");
            }
        }
    }
}
