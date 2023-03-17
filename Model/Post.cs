using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PainelPress.Model
{
    public class Post
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("title")]
        public TituloModel Title { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("link")]
        public string Link { get; set; }

        public Post() { }

        public Post(string titulo, int id, DateTime date) {
            this.Title.raw = titulo;
            this.Id = id;
            this.Date = date;
        }

        [Newtonsoft.Json.JsonIgnore]
        public string getData
        {
            get
            {
                return Date.ToString("dd/MM/yyyy HH:mm");
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public string getAuthor
        {
            get
            {
                return Usuario.getUsuario(Author).Nome;
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public int getID
        {
            get
            {
                return Convert.ToInt32(Id);
            }
        }

        public class TituloModel{
            [JsonProperty("raw")]
            public string raw { get; set; }
            [JsonProperty("rendered")]
            public string rendered { get; set; }

        }

    }
}
