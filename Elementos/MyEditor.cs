using CefSharp;
using FontAwesome.WPF;
using Google.Protobuf.Reflection;
using HarfBuzzSharp;
using PainelPress.Classes;
using PainelPress.Paginas;
using System;
using System.Diagnostics;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;

namespace PainelPress.Elementos
{
    public class MyEditor : Grid
    {
        RichTextBox editor;
        WebBrowser browser;
        StackPanel stackheaders;
        string html_template = "<html lang=\"pt-BR\"><head><meta charset=\"UTF-8\"><title>Editor PainelPress</title><style>body{font-size:18px}</style></head><body>{conteudo}</body></html>";
        public MyEditor()
        {

            editor = getEditor();
            editor.SpellCheck.IsEnabled = true;
            browser = new WebBrowser() { Visibility = Visibility.Collapsed};
            browser.Language = XmlLanguage.GetLanguage("pt-BR");

            StackPanel stackBT = getStackPanel(true);
            stackheaders = getStackPanel(false);

            this.Children.Add(editor);
            this.Children.Add(getStackPanel(true));
            this.Children.Add(stackheaders);
            this.Children.Add(browser);

        }

        private StackPanel getStackPanel(bool principal)
        {
            StackPanel stackBT = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 5, 4, 0)
            };
            if (principal)
            {
                stackBT.Children.Add(getButton("b"));
                stackBT.Children.Add(getButton("i"));
                stackBT.Children.Add(getButton("ul", FontAwesomeIcon.ListUl));
                stackBT.Children.Add(getButton("link", FontAwesomeIcon.Link));
                stackBT.Children.Add(getButton("titulo", FontAwesomeIcon.Header));
                stackBT.Children.Add(getButton("blockquote", FontAwesomeIcon.QuoteLeft));
                stackBT.Children.Add(getButton("style_centro", FontAwesomeIcon.AlignCenter));
                stackBT.Children.Add(getButton("style_direito", FontAwesomeIcon.AlignRight));
            }
            else
            {
                stackBT.Visibility = Visibility.Collapsed;
                stackBT.Margin = new Thickness(0, 5, 40, 0);
                stackBT.Children.Add(getButton("h1"));
                stackBT.Children.Add(getButton("h2"));
                stackBT.Children.Add(getButton("h3"));
                stackBT.Children.Add(getButton("h4"));
                stackBT.Children.Add(getButton("h5"));
            }
            return stackBT;
        }

        private RichTextBox getEditor()
        {
            ContextMenu context = new ContextMenu();
         //   context.Items.Add(getItem("Parágrafo", "p"));
            context.Items.Add(getItem("Link", "link"));
            context.Items.Add(getItem("Negrito", "strong"));
            context.Items.Add(getItem("Lista", "ul"));

            Style noSpaceStyle = new Style(typeof(Paragraph));
            noSpaceStyle.Setters.Add(new Setter(Paragraph.MarginProperty, new Thickness(5,5,5,12)));
      
            return new RichTextBox() {
                SelectionBrush = CorImage.GetCorPadrao(),
                Language = XmlLanguage.GetLanguage("pt-BR"),
                ContextMenu = context,
                FontSize = 19,
                Background = Cores.fundo,
                Foreground = Cores.texto,
                Padding = new Thickness(0, 0, 40, 5),
                Resources = new ResourceDictionary(){
                {typeof(Paragraph), noSpaceStyle }
                }
            };
        }

        private Button getButton(string tag, FontAwesomeIcon icon = 0)
        {
            Button bt = new Button() {
                Tag = tag,
                Margin = new Thickness(0,0,0,3),
                FontWeight = FontWeights.Bold,
                Padding = new Thickness(0),
                Width = 30, Height = 30,
                FontFamily = new FontFamily("Tahoma"),
                Background = Cores.botoes,
                Foreground = CorImage.GetCor("#FF7D7D7D"),
            };
            bt.Click += (s, e) =>
            {
                Button item = s as Button;
                if (item != null)
                {
                    string tag = item.Tag.ToString();
                    interpreteClick(tag);
                }
            };
            if (tag == "i") bt.FontStyle = FontStyles.Italic;
            if (icon != 0)
            {
                bt.Content = new ImageAwesome() { Icon = icon, Height = 19, Foreground = CorImage.GetCor("#FF7D7D7D") };
            }
            else
            {
                bt.Content = tag.ToUpper();
            }
            return bt;

        }

        private MenuItem getItem(string nome, string tag)
        {
            MenuItem menu = new MenuItem()
            {
                Header = nome,
                Tag = tag
            };
            menu.Click += (sender, e) =>
            {
                MenuItem item = sender as MenuItem;
                if(item != null)
                {
                    string tag = item.Tag.ToString();
                    interpreteClick(tag);
                }
            };
            return menu;
        }

        public void AlteraView(bool html)
        {
            if(html)
            {
                browser.Visibility = Visibility.Visible;
                editor.Visibility = Visibility.Collapsed;
                browser.NavigateToString(html_template.Replace("{conteudo}", GetTextEditor()));
            }
            else
            {
                browser.Visibility = Visibility.Collapsed;
                editor.Visibility = Visibility.Visible;
                browser.Navigate("about:blank");

            }
        }

        public void SetTextEditor(string text)
        {
            editor.Document.Blocks.Clear();
            text = text.Replace("</p><p>", "\r\n").Replace("<p>", "").Replace("</p>", "");
            editor.AppendText(text);
        }

        public string GetTextEditor()
        {
           string text = new TextRange(editor.Document.ContentStart, editor.Document.ContentEnd).Text.Trim();
            return fTexto(text);
        }

        #region BOTOES
        private void interpreteClick(string tag)
        {
        
            if (stackheaders.IsVisible && !tag.Equals("titulo")) stackheaders.Visibility = Visibility.Collapsed;
            if (editor.Selection.Text.Length <= 0) return;
            if (tag.Equals("link"))
            {
                AddLink(Clipboard.GetText());
            }
            else if (tag.Equals("ul"))
            {
                string texto = GetTextEditor();
                string newText = texto.Replace(editor.Selection.Text, formatList(editor.Selection.Text));
                SetTextEditor(newText);
            }
            else if (tag.Contains("style"))
            {
                AddStylo(tag.Replace("style_", ""));
            }
            else if (tag.Equals("titulo"))
            {
                
                if (stackheaders.IsVisible)
                {
                    stackheaders.Visibility = Visibility.Collapsed;
                }
                else
                {
                    stackheaders.Visibility = Visibility.Visible;        
                }
            }
            else
            {      
                AddTag(tag);
            }
    
        }

        private void AddTag(string tag)
        {
            if (editor.Selection.Text.Length <= 0) return;

            var selectionStart = editor.Selection.Start;
            var selectionEnd = editor.Selection.End;
            bool spaceEnd = editor.Selection.Text.EndsWith(" ");
            bool spaceStart = editor.Selection.Text.StartsWith(" ");

            if (spaceStart)
            {
                selectionStart.DeleteTextInRun(1);
                selectionStart.InsertTextInRun(" <" + tag + ">");
            }
            else
            {
                selectionStart.InsertTextInRun("<" + tag + ">");
            }

            if (spaceEnd)
            {
                selectionEnd.DeleteTextInRun(-1);
                selectionEnd.InsertTextInRun("</" + tag + "> ");
            }
            else
            {
                selectionEnd.InsertTextInRun("</" + tag + ">");
            }

            // editConteudo.Selection.Select(editConteudo.CaretPosition.DocumentEnd, editConteudo.CaretPosition.DocumentEnd);
        }
        
        private void AddStylo(string style)
        {
            if (editor.Selection.Text.Length <= 0) return;
            string tag = "";
            if (style == "centro")
            {
                tag = "text-align:center;";
            }
            else if (style == "direito")
            {
                tag = "text-align:right;";
            }
            var selectionStart = editor.Selection.Start;
            var selectionEnd = editor.Selection.End;

            selectionStart.InsertTextInRun("<p style=\"" + tag + "\">");
            selectionEnd.InsertTextInRun("</p>");

            // editConteudo.Selection.Select(editConteudo.CaretPosition.DocumentEnd, editConteudo.CaretPosition.DocumentEnd);
        }

        private void AddLink(string link)
        {
            if (link.Length == 0 || !link.StartsWith("http")) return;
            if (editor.Selection.Text.Length <= 0) return;
            var selectionStart = editor.Selection.Start;
            var selectionEnd = editor.Selection.End;

            selectionStart.InsertTextInRun("<a href=\"" + link + "\">");
            selectionEnd.InsertTextInRun("</a>");

            //  editConteudo.Selection.Select(editConteudo.CaretPosition.DocumentEnd, editConteudo.CaretPosition.DocumentEnd);
        }

        private string fTexto(string text)
        {
            int pontos = text.Split("\r\n").Length - 1;
            if (pontos > 0)
            {
                return "<p>" + text.Replace("\r\n", "</p><p>").Replace("<p></p>", "") + "</p>";
            }
            else
            {
                pontos = text.Split("\n").Length - 1;
                if (pontos > 0)
                {
                    return "<p>" + text.Replace("\n", "</p><p>").Replace("<p></p>", "").Replace("\n", "") + "</p>";
                }
            }
            return text;
        }

        private string formatList(string text)
        {
            var items = text.Split("\r\n");
            string hmtl = "";
            foreach (var item in items)
            {
                hmtl += $"<li>{item}</li>";
            }
            return $"<ul>{hmtl}</ul>";
        }

        #endregion

    }
}