using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PainelPress.Classes
{
    public class CorImage
    {
        public static Brush GetCor(string codigo)
        {
            var bc = new BrushConverter();
            Brush cor = (Brush)bc.ConvertFrom(codigo);
            return cor;
        }


        public static Brush GetCorPadrao()
        {
            var bc = new BrushConverter();
            Brush cor = (Brush)bc.ConvertFrom("#FF673AB7");
            return cor;
        }

        public static Brush GetCorIcone()
        {
            var bc = new BrushConverter();
            Brush cor = (Brush)bc.ConvertFrom("#FF673AB7");
            return cor;
        }
        public static Brush GetCorVerde()
        {
            var bc = new BrushConverter();
            Brush cor = (Brush)bc.ConvertFrom("#FF96C561");
            return cor;
        }
        public static SolidColorBrush Transparente() => new SolidColorBrush(Colors.Transparent);

        public static BitmapImage GetImagemProjeto(string name)
        {
            string path = "pack://application:,,,/Imagem/" + name + ".png";
            BitmapImage imagem = new BitmapImage(new Uri(path, UriKind.Absolute));
            return imagem;
        }


    }
}
