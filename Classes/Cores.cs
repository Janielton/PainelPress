using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PainelPress.Classes
{
    public class Cores
    {
        static bool dark = false;
        public static Brush texto => dark ? CorImage.GetCor("#ffffff"): CorImage.GetCor("#333333");
        
        public static Brush fundo => dark ? CorImage.GetCor("#333333") : CorImage.GetCor("#ffffff");

        public static Brush botoes => dark ? CorImage.GetCor("#FFF3F3F3") : CorImage.GetCor("#FFD2D2D2");

    }
}
