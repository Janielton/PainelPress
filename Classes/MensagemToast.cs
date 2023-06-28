using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PainelPress.Classes
{
    public class MensagemToast
    {
        private int timerTickCount = 0;
        DispatcherTimer timer;
        public void HomeMensagem(bool sucesso, string msg)
        {
            if (sucesso)
            {

                MainWindow.lbMensagem.Content = msg;
                MainWindow.brMensagem.Visibility = Visibility.Visible;
                MainWindow.brMensagem.Background = CorImage.GetCor("#FFB3E237");
            }
            else
            {
                MainWindow.lbMensagem.Content = msg;
                MainWindow.brMensagem.Visibility = Visibility.Visible;
                MainWindow.brMensagem.Background = CorImage.GetCor("#FFF74343");
                Application.Current.MainWindow.Focus();
            }

            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 1); // will 'tick' once every second
                timer.Tick += new EventHandler(Timer_Tick);
                timer.Start();
            }
            else
            {
                timer.Start();
            }
        }

        private void LimparMensagem()
        {
            MainWindow.lbMensagem.Content = "";
            MainWindow.brMensagem.Visibility = Visibility.Collapsed;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            if (++timerTickCount == 2)
            {
                timer.Stop();
                LimparMensagem();
                timerTickCount = 0;
            }
        }

        public static MensagemToast instance => new MensagemToast();
    }
}
