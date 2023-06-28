using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PainelPress.Model
{
    public class CategoriaFull
    {
        public string name { get; set; }
        public string term_id { get; set; }
        public string description { get; set; }
        public List<SubCat> subs { get; set; }
    }

    public class SubCat
    {
        public string name { get; set; }
        public string term_id { get; set; }
        public string description { get; set; }

        public CategoriaFull subsToFull => new CategoriaFull()
        {
            name = name,
            term_id = term_id,
            description = description,
            subs = new List<SubCat>()
        };
      
    }
}
