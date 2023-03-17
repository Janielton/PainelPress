using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PainelPress.Model
{
    public class ERegistro
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("nome")]
        public string Nome { get; set; }
       
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("status")]
        public int Status { get; set; }
        [JsonProperty("regiao")]
        public int Regiao { get; set; }
        [JsonProperty("inscricao_data")]
        public string InscricaoData { get; set; }

        public ERegistro(string nome, string email, string data, int st, int rg)
        {
            Nome = nome;
            Email = email;
            InscricaoData = data;
            Status = st;
            Regiao = rg;
        }

        public ERegistro()
        {
        }
    }
}
