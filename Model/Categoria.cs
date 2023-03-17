using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PainelPress.Model
{
    public class Categoria
    {
        [JsonProperty("term_id")]
        public int TermId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("term_group")]
        public int TermGroup { get; set; }

        [JsonProperty("term_taxonomy_id")]
        public int TermTaxonomyId { get; set; }

        [JsonProperty("taxonomy")]
        public string Taxonomy { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("parent")]
        public int Parent { get; set; }

        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("filter")]
        public string Filter { get; set; }

        [JsonProperty("cat_ID")]
        public int CatID { get; set; }

        [JsonProperty("category_count")]
        public int CategoryCount { get; set; }

        [JsonProperty("category_description")]
        public string CategoryDescription { get; set; }

        [JsonProperty("cat_name")]
        public string CatName { get; set; }

        [JsonProperty("category_nicename")]
        public string CategoryNicename { get; set; }

        [JsonProperty("category_parent")]
        public int CategoryParent { get; set; }
    }
}
