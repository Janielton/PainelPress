﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PainelPress.Model
{
    public class Tag
    { 
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("description")]
    public string Description { get; set; }

    [JsonProperty("link")]
    public string Link { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("slug")]
    public string Slug { get; set; }

    [JsonProperty("taxonomy")]
    public string Taxonomy { get; set; }
    }
}
