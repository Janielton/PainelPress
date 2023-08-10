using ChatGPT.Net.DTO.ChatGPT;
using FontAwesome.WPF;
using IA;
using Newtonsoft.Json;
using PainelPress.Classes;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Globalization;

namespace PainelPress.Paginas
{
    /// <summary>
    /// Interação lógica para ChatPage.xam
    /// </summary>
    public partial class ChatPage : ContentControl
    {
        MainIA mainIA = new MainIA();

        public ChatPage()
        {
            InitializeComponent();

        }

        private void editPergunta_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter && editPergunta.Text != "")
            {
                Request(editPergunta.Text);
                editPergunta.Text = "";
            }
        }

        private async void Request(string msg)
        {
            stackChat.Children.Add(messagemMe(msg));
            string result = getConteudo(); //await mainIA.RequestGPT(msg);
            stackChat.Children.Add(messagemIa(result));
        }

        private string getConteudo()
        {
            try
            {
                string json = File.ReadAllText(Constants.PASTA + "result.json");
                ChatGptResponse obj = JsonConvert.DeserializeObject<ChatGptResponse>(json);
                return obj.Choices[0].Message.Content;
            }catch(Exception ex) { Debug.WriteLine(ex.Message); }
            return "NO";
        }

        private Border messagemMe(string msg)
        {
            Border border = new Border() { 
                CornerRadius = new CornerRadius(4),
                Background = CorImage.GetCor("#FF009FE2"),
                Margin = new Thickness(8, 4, 15, 4),
                Padding = new Thickness(5),
            };

            var stack = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(2, 3, 8, 6),
                HorizontalAlignment = HorizontalAlignment.Left

            };
            stack.Children.Add(new TextBlock()
            {
                Text = msg,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 570,
                FontSize = 15,
                Foreground = Brushes.White
            });
            border.Child = stack;

            return border;
        }

        private Border messagemIa(string msg)
        {
            Border border = new Border()
            {
                CornerRadius = new CornerRadius(8),
                Background = CorImage.GetCor("#FFA2C825"),
                Margin = new Thickness(18, 4, 5, 4)
            };

            StackPanel stack = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(2, 3, 8, 6),
                HorizontalAlignment = HorizontalAlignment.Center

            };
            stack.Children.Add(new TextBox()
            {
                Text = msg,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 570,
                FontSize = 16,
                Foreground = Brushes.White,
                IsReadOnly = true,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0)
            });
            StackPanel stackBase = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Button btCopy = new Button()
            {
                Padding = new Thickness(0),
                Background = Brushes.Transparent,
                Cursor = Cursors.Hand,
                Content = new ImageAwesome() { Icon = FontAwesomeIcon.Copy},
                Tag = "23",
                Width = 30,
                Foreground = CorImage.GetCor("#FF88B365"),
            };
            btCopy.Click += btCopy_Click;
            stackBase.Children.Add(btCopy);
            stack.Children.Add(stackBase);
            border.Child = stack;
            return border;
        }


        private void btCopy_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            if(bt != null)
            {
                string tag = bt.Tag.ToString();
                Debug.WriteLine(tag);
            }
        }

        private void botoes_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            if (bt != null)
            {
                string tag = bt.Tag.ToString();
                if (tag == "send")
                {

                }
                else if(tag == "audio")
                {
                 
                }
            }
        }
       
    }
}
