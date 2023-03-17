using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
    /// Lógica interna para WLog.xaml
    /// </summary>
    public partial class WLog : Window
    {
        public WLog()
        {
            InitializeComponent();
            SetLog();
        }

        private async void SetLog()
        {
            string file = @"C:\WPanel\logs_painel.txt";
            try
            {
                if (!File.Exists(file))
                {
                    File.Create(file);
                }
                else {
                   string text = await File.ReadAllTextAsync(file);
                   EditorLog.AppendText(text);
                }
             
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private void btClear_Click(object sender, RoutedEventArgs e)
        {
            string file = @"C:\WPanel\logs_painel.txt";
            try
            {
                if (File.Exists(file))
                {
                    File.WriteAllText(file, "");
                    EditorLog.AppendText("");
                } 
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
