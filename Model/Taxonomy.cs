using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace PainelPress.Model
{
    public class Taxonomy
    {
        public static int Tag = 0;
        public static int Cargos = 1;
        public static int Cidade = 2;

        [JsonPropertyName("term_id")]
        public string TermId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }
    }
}
