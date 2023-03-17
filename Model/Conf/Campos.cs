using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PainelPress.Model.Conf
{
    public class Campos
    {
        public string nome { get; set; }
        public DadosCampos dados { get; set; }
    }
    public class DadosCampos
    {
        public string type { get; set; }
        public string description { get; set; }
        public bool single { get; set; }
        public bool show_in_rest { get; set; }
    }

}
