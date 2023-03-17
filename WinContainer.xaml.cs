using PainelPress.Paginas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PainelPress
{
    /// <summary>
    /// Lógica interna para WinContainer.xaml
    /// </summary>
    public partial class WinContainer : Window
    {
        public WinContainer()
        {
            InitializeComponent();
        }

        public WinContainer(int tipo)
        {
            InitializeComponent();
            if (tipo == 1)
            {
                this.Title = "Configurações";
                contentContainer.Content = new Configuracao();
            }else if (tipo == 2)
            {

            }
        }
    }
}
