using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PainelPress.Classes
{

    public class Terms
    {
        [JsonProperty(PropertyName = "tag")]
        public string Tag { get; set; }

        [JsonProperty(PropertyName = "concurso")]
        public string Cidade { get; set; }

        [JsonProperty(PropertyName = "vagas")]
        public string Cargos { get; set; }
    }

    public class Meta
    {
        [JsonProperty(PropertyName = "questoes")]
        public string Questoes { get; set; }

        [JsonProperty(PropertyName = "edital")]
        public string Edital { get; set; }

        [JsonProperty(PropertyName = "inscricao")]
        public string Inscricao { get; set; }
      
        [JsonProperty(PropertyName = "provas")]
        public string Provas { get; set; }
       
        [JsonProperty(PropertyName = "salario")]  
        public string Salario { get; set; }

        [JsonProperty(PropertyName = "vagas")]
        public string Vagas { get; set; }

        [JsonProperty(PropertyName = "imagem")]
        public string Imagem { get; set; }
    }


    public class PostModel
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }

         [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }

         [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
        
        [JsonProperty(PropertyName = "author")]
        public string Autor { get; set; }

        [JsonProperty(PropertyName = "categories")]
        public string Categories { get; set; }

         [JsonProperty(PropertyName = "tags")]
        public string Tags { get; set; }

         [JsonProperty(PropertyName = "excerpt")]
        public string Excerpt { get; set; }
        
        [JsonProperty(PropertyName = "date")]
        public string Date { get; set; }

        [JsonProperty(PropertyName = "terms")]
        public Terms Terms { get; set; }

        [JsonProperty(PropertyName = "meta")]
        public Meta Meta { get; set; }
    }
}
