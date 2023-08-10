using FontAwesome.WPF;
using PainelPress.Classes;
using PainelPress.Model;
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

        public WinContainer(ChatPage pagina)
        {
            InitializeComponent();
            Titulo.Text = "ChatGPT";
            this.Tag = "gpt";
            this.Width = 850;
            this.Height = 650;
            contentContainer.Content = pagina;
        }
        public WinContainer(Relatorios pagina)
        {
            InitializeComponent();
            Titulo.Text = "Analytics";
            this.Tag = "telatorios";
            this.Width = 850;
            this.Height = 650;
            contentContainer.Content = pagina;
        }
        public WinContainer(PaginasSites pagina)
        {
            InitializeComponent();
            Titulo.Text = "Acompanhamento de paginas";
            this.Tag = "acompanhapaginas";
            this.Width = 850;
            this.Height = 650;
            contentContainer.Content = pagina;
        }

        public WinContainer(LeitorFeed pagina)
        {
            InitializeComponent();
            Titulo.Text = "Leitor Feed";
            this.Tag = "leitorfeed";
            this.Width = 850;
            this.Height = 650;
            contentContainer.Content = pagina;
        }

        public WinContainer(BuscaPosts pagina)
        {
            InitializeComponent();
            Titulo.Text = "Buscar posts";
            this.Tag = "busca-posts";
            this.Height = 350;
            this.Width = 700;
            btMaximizar.Visibility = Visibility.Collapsed; 
            btMinimizar.Visibility = Visibility.Collapsed;
            contentContainer.Content = pagina;
        }
        public WinContainer(Categorias pagina)
        {
            InitializeComponent();
            Titulo.Text = "Categorias";
            this.Tag = "categorias";
            this.Width = 780;
            this.Height = 580;
            contentContainer.Content = pagina;
        }
        
        public WinContainer(PostView post)
        {
            InitializeComponent();
            Titulo.Text = "Adicionar/editar story";
            this.Tag = "stories-add";
            contentContainer.Content = new Stories(post);
        }
        public WinContainer(Social pagina)
        {
            InitializeComponent();
            Titulo.Text = "Rede Sociais";
            this.Tag = "social";
            this.Width = 900;
            this.Height = 650;
            btMaximizar.Visibility = Visibility.Collapsed;
            contentContainer.Content = pagina;
        }
        
        public WinContainer(Stories pagia)
        {
            InitializeComponent();
            this.Width = 900;
            this.Height = 500;
            this.Tag = "stories";
            Titulo.Text = "Gerenciador de stories";
            contentContainer.Content = pagia;
        }

        public WinContainer(int tipo)
        {
            InitializeComponent();
            if (tipo == 1)
            {
                Titulo.Text = "Configurações";
                this.Width = 1000;
                this.Height = 600;
                this.Tag = "config";
                contentContainer.Content = new Configuracao();
            }else if (tipo == 2)
            {

            }
        }


        private void btCloseWin_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btMaximizar_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                btMaximizar.Content = new ImageAwesome()
                {
                    Icon = FontAwesomeIcon.WindowRestore,
                    Foreground = Brushes.White,
                    Width = 20
                };
            }
            else
            {
                this.WindowState = WindowState.Normal;
                btMaximizar.Content = new ImageAwesome()
                {
                    Icon = FontAwesomeIcon.WindowMaximize,
                    Foreground = Brushes.White,
                    Width = 20
                };
            }

        }

        private void btMinimizar_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

    }
}
