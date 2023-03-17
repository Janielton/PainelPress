using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PainelPress.Model
{
    public class EmailNew
    {
        [JsonProperty("email")]
        public string email { get; set; }

        [JsonProperty("assunto")]
        public string assunto { get; set; }

        [JsonProperty("corpo")]
        public string corpo { get; set; }
    }
}
