using PainelPress.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PainelPress.Elementos
{
    public class EditText : Grid
    {
        TextBlock textLegenda = new TextBlock();
        TextBlock textCount = new TextBlock();
        TextBox textBox = new TextBox();

        public bool ShowCount
        {
            get => textCount.IsVisible;
            set => textCount.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
        }

        public string Legenda
        {
            get => textLegenda.Text;
            set => textLegenda.Text = value;
        }
        public double FontLegenda
        {
            get => textLegenda.FontSize;
            set => textLegenda.FontSize = value;
        }

        public double FontSize
        {
            get => textBox.FontSize;
            set => textBox.FontSize = value;
        }

        public string Count
        {
            set => textCount.Text = value;
        }

        public bool QuebraLinha
        {
            get
            {
                return textBox.TextWrapping == TextWrapping.Wrap;
            }
            set
            {
                if (value)
                {
                    textBox.TextWrapping = TextWrapping.Wrap;
                    Altura = double.NaN;
                }
                else
                {
                    textBox.TextWrapping = TextWrapping.NoWrap;
                }       
            }
        }

        public string Texto
        {
            get
            {
                return textBox.Text;
            }
            set
            {
                textBox.Text = value;
                if (value != null)
                {
                    textCount.Text = value.Length.ToString();
                } 
            }
        }

        public double Altura
        {
            get
            {
                return this.Height;
            }
            set
            {
                this.Height = value;
                textBox.Height = value;
            }
        }

        public TextChangedEventHandler TextoChanged
        {
            set
            {
                textBox.TextChanged += value;
            }
        }

        public EditText()
        {
           
            Setap();
        }

        private void Setap()
        {
             this.Altura = 47;
             textLegenda = getTextBlock(true);
             textCount = getTextBlock(false);
             textBox = getTextBox();
             this.Children.Add(getBorder());
             this.Children.Add(textBox);
             this.Children.Add(textLegenda);
             this.Children.Add(textCount);
        }

        private Border getBorder() => new Border()
        {
            CornerRadius = new CornerRadius(8),
            Background = CorImage.GetCor("#FFFFFF")
        };

        private TextBlock getTextBlock(bool legenda)
        {
            TextBlock block = new TextBlock();
            if (legenda)
            {
                block.Margin = new Thickness(3, 1, 0, 4);
                block.Padding = new Thickness(2, 0, 0, 0);
                block.VerticalAlignment = VerticalAlignment.Top;
                block.HorizontalAlignment = HorizontalAlignment.Left;
                block.Foreground = new SolidColorBrush(Colors.Gray);
                block.FontSize = 13;
                block.MouseLeftButtonDown += Block_MouseLeftButtonDown;
            }
            else
            {
                block.Text = "0";
                block.Visibility = Visibility.Collapsed;
                block.Margin = new Thickness(0);
                block.Padding = new Thickness(4, 0, 4, 0);
                block.VerticalAlignment = VerticalAlignment.Top;
                block.HorizontalAlignment = HorizontalAlignment.Right;
                block.Foreground = new SolidColorBrush(Colors.White);
                block.Background = CorImage.GetCor("#FF5CD130");
                block.FontSize = 13;
            }
            return block;
        }

        private void Block_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            textBox.Focus();
        }

        private TextBox getTextBox()
        {
            TextBox box = new TextBox();
            box.FontSize = 19;
            box.Padding = new Thickness(0, 1, 0, 0);
            box.Margin = new Thickness(7, 15, 7, 0);
            box.BorderThickness = new Thickness(0);
            box.VerticalAlignment = VerticalAlignment.Stretch;
            box.SpellCheck.IsEnabled = true; 
            box.TextChanged += edit_TextChanged;
            return box;
        }

        private void edit_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ShowCount)
            {
                var textBox = (TextBox)sender;
                if (textBox != null)
                {
                    Count = textBox.Text.Length.ToString();
                }
            }
            
        }
    }
}
