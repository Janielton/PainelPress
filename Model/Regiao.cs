using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PainelPress.Model
{
   public class Regiao
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("cat_id")]
        public int CatId { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        public Regiao(int id, string nome) {
            Id = id;
            Nome = nome;
        }
    }
}
