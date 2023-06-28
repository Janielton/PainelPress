using Google.Protobuf.Reflection;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace PainelPress.Elementos
{
    public class MyEditor : RichTextBox
    {
        public MyEditor()
        {
            ContextMenu context = new ContextMenu();
            context.Items.Add(getItem("Parágrafo","p"));
            context.Items.Add(getItem("Link", "link"));
            context.Items.Add(getItem("Negrito", "strong"));
            this.ContextMenu = context;
            this.FontSize = 17;
            this.Padding = new Thickness(5,5,70,5);
            this.Language = XmlLanguage.GetLanguage("pt-BR");
            SpellCheck.IsEnabled = true;
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

        private void interpreteClick(string tag)
        {
            Debug.WriteLine(tag);
        }

    }
}