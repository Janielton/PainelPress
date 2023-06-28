using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PainelPress.Model
{
    public class Post
    {
      
        public int id { get; set; }
        public string author { get; set; }   
        public TituloModel title { get; set; }
        public string date { get; set; }
        public string status { get; set; }
        public string link { get; set; }


        public Post() { }

        public Post(string titulo, int id, string date) {
            this.title.raw = titulo;
            this.id = id;
            this.date = date;
        }

        [Newtonsoft.Json.JsonIgnore]
        public string getData
        {
            get
            {
                return Convert.ToDateTime(date).ToString("dd/MM/yyyy HH:mm");
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public string getAuthor
        {
            get
            {
                return Usuario.getUsuario(author).Nome;
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public int getID
        {
            get
            {
                return Convert.ToInt32(id);
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        public string getTitulo
        {
            get
            {
                return title.raw;
            }
        }
        [Newtonsoft.Json.JsonIgnore]
        public string setTitulo
        {
            set
            {
                title.raw = value;
                title.rendered = value;
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
