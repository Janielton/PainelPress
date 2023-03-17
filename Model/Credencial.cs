using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PainelPress.Model
{
    public class Credencial
    {
        [JsonProperty("ConsumerKey")]
        public string ConsumerKey { get; set; }
        [JsonProperty("ConsumerSecret")]
        public string ConsumerSecret { get; set; }
        [JsonProperty("AccessToken")]
        public string AccessToken { get; set; }
        [JsonProperty("AccessTokenSecret")]
        public string AccessTokenSecret { get; set; }
        [JsonProperty("BearerToken")]
        public string BearerToken { get; set; }
    }
}
