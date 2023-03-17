using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PainelPress.Model
{
    public class Inscritos
    {
        [JsonPropertyName("topico")]
        public int Topico { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("inscritos")]
        public int Quantidade { get; set; }
    }
}
