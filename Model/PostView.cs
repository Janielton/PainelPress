using Newtonsoft.Json;
using PainelPress.Classes;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PainelPress.Model
{
    public class PostView
    {
        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonProperty("post_author")]
        [JsonPropertyName("post_author")]
        public string Author { get; set; }

        [JsonProperty("post_title")]
        [JsonPropertyName("post_title")]
        public string Title { get; set; }

        [JsonProperty("post_content")]
        [JsonPropertyName("post_content")]
        public string Content { get; set; }

        [JsonProperty("post_excerpt")]
        [JsonPropertyName("post_excerpt")]
        public string Resumo { get; set; }

        [JsonProperty("post_date")]
        [JsonPropertyName("post_date")]
        public string Date { get; set; }

        [JsonProperty("post_status")]
        [JsonPropertyName("post_status")]
        public string Status { get; set; }

        [JsonProperty("post_name")]
        [JsonPropertyName("post_name")]
        public string Slug { get; set; }

        [JsonProperty("categoria")]
        [JsonPropertyName("categoria")]
        public string Categoria { get; set; }

    
        [JsonPropertyName("tag")]
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("imagem_destaque")]
        [JsonProperty("imagem_destaque")]
        public string imagem_destaque { get; set; }

        [JsonProperty("metas")]
        [JsonPropertyName("metas")]
        public List<MetaGetPost> Metas { get; set; }

        [JsonProperty("terms")]
        [JsonPropertyName("terms")]
        public object Terms { get; set; }

        [JsonProperty("meta")]
        [JsonPropertyName("meta")]
        public Dictionary<string, object> Meta { get; set; }

        public List<MetaGetPost> metasToMeta()
        {
            List<MetaGetPost> meta = new List<MetaGetPost>();
            if (Meta == null) return meta;
            foreach(var met in Meta)
            {
                meta.Add(new MetaGetPost { MetaKey = met.Key, MetaValue = met.Value.ToString() });
            }
            return meta;
        }
    }

    public class MetaGetPost
    {
        [JsonPropertyName("meta_key")]
        public string MetaKey { get; set; }

        [JsonPropertyName("meta_value")]
        public string MetaValue { get; set; }
    }


}
