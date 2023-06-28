using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PainelPress.Model
{
    public class Story
    {
        [AliasAs("id")]
        public string id { get; set; }

        [AliasAs("titulo")]
        public string titulo { get; set; }

        [AliasAs("resumo")]
        public string resumo { get; set; }
        
        [AliasAs("slug")]
        public string slug { get; set; }
        
        [AliasAs("imagem")]
        public string imagem { get; set; }
       
        [AliasAs("id_post")]
        public string id_post { get; set; }
       
        [AliasAs("data")]
        public string data { get; set; }

    }
}
