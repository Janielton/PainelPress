using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PainelPress.Model
{
    public class DataView
    {
        [JsonIgnore]
        public int IdItem { get; set; }
        [JsonIgnore]
        public string iTem1 { get; set; }
        [JsonIgnore]
        public string iTem2 { get; set; }
        [JsonIgnore]
        public string iTem3 { get; set; }
        [JsonIgnore]
        public string iTem4 { get; set; }
    }
}
