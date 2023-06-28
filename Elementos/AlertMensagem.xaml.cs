using FontAwesome.WPF;
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

namespace PainelPress.Elementos
{
    /// <summary>
    /// Lógica interna para AlertMensagem.xaml
    /// </summary>
    public partial class AlertMensagem : Window
    {
        public string result = "no";
        public AlertMensagem()
        {
            InitializeComponent();
        }

        private void btClose_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
           // this.Close();
        }

        public void Show(string mensagem)
        {
            TvMensagem.Text = mensagem;
            RowTitulo.Height = new GridLength(35);
            this.ShowDialog();
        }

        public void Show(string mensagem, bool erro)
        {
            TvMensagem.Text = mensagem;
            this.ShowDialog();
        }

        public void Show(string mensagem, string titulo)
        {
            TvMensagem.Text = mensagem;
            Tvtitulo.Text = titulo;
            this.ShowDialog();
        }

        public void Show(string mensagem, string titulo, bool erro)
        {
            TvMensagem.Text = mensagem;
            Tvtitulo.Text = titulo;
            this.ShowDialog();
        }

        public AlertMensagem Confirme(string mensagem, string titulo, int tipo = 0)
        {
            TvMensagemConfirme.Text = mensagem;
            Tvtitulo.Text = titulo;
            containerConfirme.Visibility = Visibility.Visible;
            containerNormal.Visibility = Visibility.Collapsed;
            if(tipo > 0)
            {
                icone.Visibility = Visibility.Visible;
                if (tipo == 2)
                {
                    icone.Icon = FontAwesomeIcon.Warning;
                }
            }
            return this;
        }
        public AlertMensagem Aviso(string mensagem, string titulo, int tipo = 0)
        {
            TvMensagemConfirme.Text = mensagem;
            Tvtitulo.Text = titulo;
            containerConfirme.Visibility = Visibility.Visible;
            containerNormal.Visibility = Visibility.Collapsed;
            btCancelar.Content = "Não";
            btConfirmar.Content = "Sim";
            if (tipo > 0)
            {
                icone.Visibility = Visibility.Visible;
                if (tipo == 2)
                {
                    icone.Icon = FontAwesomeIcon.Warning;
                }
            }
            return this;
        }

        public static AlertMensagem instance
        {
            get { return new AlertMensagem(); }
        }

        private void Janela_MouseMove(object sender, MouseButtonEventArgs e)
        {
            try
            {
                DragMove();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
           /// this.Hide();
        }

        private void btCancelar_Click(object sender, RoutedEventArgs e)
        {
            result = "no";
            this.DialogResult = true;
            this.Close();
        }

        private void btConfirmar_Click(object sender, RoutedEventArgs e)
        {
            result = "sim";
            this.DialogResult = true;
            this.Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && containerConfirme.IsVisible)
            {
                btConfirmar.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            }
        }
    }
}
