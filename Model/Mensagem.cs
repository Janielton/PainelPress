using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PainelPress.Model
{
    public class Mensagem
    {
        [JsonProperty("Messages")]
        public Messages Messages { get; set; }

        [JsonProperty("More")]
        public object More { get; set; }
    }
    public class Messages
    {
        [JsonProperty("painel_email")]
        public ObservableCollection<WinNotification> WinNotification { get; set; }
    }
    public class WinNotification
    {
        [JsonProperty("Timetoken")]
        public object Timetoken { get; set; }

        [JsonProperty("Entry")]
        public Entry Entry { get; set; }

        [JsonProperty("Meta")]
        public object Meta { get; set; }

        [JsonProperty("Actions")]
        public object Actions { get; set; }

        [JsonProperty("Uuid")]
        public string Uuid { get; set; }

        [JsonProperty("MessageType")]
        public int MessageType { get; set; }
    }
    public class Entry
    {
        [JsonProperty("Operacao")]
        public string Operacao { get; set; }

        [JsonProperty("Data")]
        public string Data { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }
    }
}
