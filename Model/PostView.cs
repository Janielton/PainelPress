using Newtonsoft.Json;
using PainelPress.Classes;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PainelPress.Model
{
    public class PostView
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("post_author")]
        public string Author { get; set; }

        [JsonPropertyName("post_title")]
        public string Title { get; set; }

        [JsonPropertyName("post_content")]
        public string Content { get; set; }

        [JsonPropertyName("post_excerpt")]
        public string Resumo { get; set; }

        [JsonPropertyName("post_date")]
        public string Date { get; set; }

        [JsonPropertyName("post_status")]
        public string Status { get; set; }

        [JsonPropertyName("post_name")]
        public string Slug { get; set; }

        [JsonPropertyName("categoria")]
        public string Categoria { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("cidade")]
        public string Cidade { get; set; }

        [JsonPropertyName("cargos")]
        public string Cargos { get; set; }
         
        [JsonPropertyName("metas")]
        public List<MetaGetPost> Metas { get; set; }

        [JsonPropertyName("terms")]
        public Terms Terms { get; set; }

        public Meta Meta { get; set; }

        public List<MetaGetPost> metasToMeta()
        {
            List<MetaGetPost> meta = new List<MetaGetPost>();
            if (Meta == null) return meta;
            if (Meta.Imagem != null)
            {
                meta.Add(new MetaGetPost { MetaKey = "imagem", MetaValue = Meta.Imagem });
            }
            if (Meta.Provas != null)
            {
                meta.Add(new MetaGetPost { MetaKey = "provas", MetaValue = Meta.Provas });
            }
            if (Meta.Salario != null)
            {
                meta.Add(new MetaGetPost { MetaKey = "salario", MetaValue = Meta.Salario });
            }
            if (Meta.Vagas != null)
            {
                meta.Add(new MetaGetPost { MetaKey = "vagas", MetaValue = Meta.Vagas });
            }
            if (Meta.Edital != null)
            {
                meta.Add(new MetaGetPost { MetaKey = "edital", MetaValue = Meta.Edital });
            }
            if (Meta.Questoes != null)
            {
                meta.Add(new MetaGetPost { MetaKey = "questoes", MetaValue = Meta.Questoes });
            }
            if (Meta.Inscricao != null)
            {
                meta.Add(new MetaGetPost { MetaKey = "inscricao", MetaValue = Meta.Inscricao });
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
