using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows;

namespace PainelPress.Classes.Controller
{
    public static class JanelasControl
    {
        public static void Esconder(string tag)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Tag == null) continue;
                if (window.Tag.ToString() == tag)
                {
                    window.Hide();
                }
            }
        }

        public static void Mostrar(string tag)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Tag == null) continue;
                if (window.Tag.ToString()== tag)
                {
                    window.Show();
                }
            }
        }

        public static void Close(string tag)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Tag == null) continue;
                if (window.Tag.ToString() == tag)
                {
                    window.Close();
                }
            }
        }

        public static WindowCollection getAllJanelas() => Application.Current.Windows;
    }
}
