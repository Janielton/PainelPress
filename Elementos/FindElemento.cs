using PainelPress.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Controls;

namespace PainelPress.Elementos
{
    public static class FindElemento
    {
        public static IEnumerable<T> FindElements<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield return (T)Enumerable.Empty<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject ithChild = VisualTreeHelper.GetChild(depObj, i);
                if (ithChild == null) continue;
                if (ithChild is T t) yield return t;
                foreach (T childOfChild in FindElements<T>(ithChild)) yield return childOfChild;
            }
        }

        public static StackPanel FindStack(StackPanel el, string tag)
        {
            IEnumerable<StackPanel> container = FindElements<StackPanel>(el).Where(x => x.Tag != null && x.Tag.ToString() == tag);
            if (container.Count() == 0) return null;
            return container.First();
        }

        public static Border FindBorder(StackPanel el, string tag)
        {
            IEnumerable<Border> container = FindElements<Border>(el).Where(x => x.Tag != null && x.Tag.ToString() == tag);
            if(container.Count() == 0) return null;
            return container.First();
        }

     
        public static ListBox FindListBox(StackPanel el, string tag)
        {
            IEnumerable<ListBox> container = FindElements<ListBox>(el).Where(x => x.Tag != null && x.Tag.ToString() == tag);
            if (container.Count() == 0) return null;
            return container.First();
        }

        public static TextBox FindTextBox(StackPanel el, string tag)
        {
            IEnumerable<TextBox> container = FindElements<TextBox>(el).Where(x => x.Tag != null && x.Tag.ToString() == tag);
            if (container.Count() == 0) return null;
            return container.First();
        }

        public static TextBlock FindTextBlock(StackPanel el, string tag)
        {
            IEnumerable<TextBlock> container = FindElements<TextBlock>(el).Where(x => x.Tag != null && x.Tag.ToString() == tag);
            if (container.Count() == 0) return null;
            return container.First();
        }

        public static Button FindButton(StackPanel el, string tag)
        {
            IEnumerable<Button> container = FindElements<Button>(el).Where(x => x.Tag != null && x.Tag.ToString() == tag);
            if (container.Count() == 0) return null;
            return container.First();
        }

        public static CheckBox FindCheckBox(StackPanel el, string tag)
        {
            IEnumerable<CheckBox> container = FindElements<CheckBox>(el).Where(x => x.Tag != null && x.Tag.ToString() == tag);
            if (container.Count() == 0) return null;
            return container.First();
        }
    }
}
