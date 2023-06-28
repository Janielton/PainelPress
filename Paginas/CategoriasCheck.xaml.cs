
using PainelPress.Classes;
using PainelPress.Elementos;
using PainelPress.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using config = PainelPress.Properties.Settings;

namespace PainelPress.Paginas
{
    /// <summary>
    /// Interação lógica para CategoriasCheck.xam
    /// </summary>
    public partial class CategoriasCheck : Page
    {
        List<string> listCats = new List<string>();
        List<CategoriaFull> categorias = new List<CategoriaFull>();
        List<string> catsPai = new List<string>();
        public static string CatList;

        public CategoriasCheck()
        {
            InitializeComponent();
            Setap();
        }

        public CategoriasCheck(string[] cats)
        {
            InitializeComponent();
            listCats.Clear();
            Setap();
            SetCats(cats);         
        }


        private void Setap()
        {
            string cats = config.Default.categorias;
            if (cats != "")
            {
                categorias = Ferramentas.JsonToObjeto(cats, 3) as List<CategoriaFull>;
                SetCatsInContainer(categorias);              
            }
        }


        public void SetCatsInContainer(List<CategoriaFull> categorias)
        {
 
            foreach (var cat in categorias)
            {
                StackPanel stack = new StackPanel()
                {
                    Orientation = Orientation.Vertical
                };
                CheckBox checkPai = CategoriaBox(cat, true);
                stack.Children.Add(checkPai);
                if (HasChild(cat))
                {
                    stack.Children.Add(StackComSubs(cat.subs, cat.term_id));
                    catsPai.Add(cat.term_id);
                }
                containerCats.Children.Add(stack);
            }
            
            bool HasChild(CategoriaFull cat)
            {
                return cat.subs.Count > 0;
            }

            CheckBox CategoriaBox(CategoriaFull cat, bool pai)
            {
  
                var check = new CheckBox()
                {
                    Content = cat.name,
                    Tag = cat.term_id,
                    Margin = new Thickness(0,0,0,2)
                };
                this.RegisterName($"cb_{cat.term_id}", check);
                if (pai)
                {
                    check.Style = Resources["CheckBoxStylePai"] as Style;
                    check.FontSize = 20;
                }
                else
                {
                    check.Style = Resources["CheckBoxStyleSub"] as Style;
                    check.FontSize = 16;
                }
                check.Checked += CheckBox_Checked;
                check.Unchecked += CheckBox_Unchecked;
                return check;
            }

           

            StackPanel StackComSubs(List<SubCat> subs, string id)
            {
                StackPanel stack = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    Background = CorImage.GetCor("#FFF5F5F5"),
                    Visibility = Visibility.Collapsed,
                   // Name = $"stack{id}"
                };
                this.RegisterName($"stack{id}", stack);
 
                foreach (SubCat cat in subs)
                {
                    CheckBox checkPai = CategoriaBox(cat.subsToFull, false);
                    stack.Children.Add(checkPai);
                }
                return stack;
            }

        }

        public void SetCats(string[] cats)
        {    
 
            try
            {
                listCats.Clear();
                foreach (string item in cats)
                {
                    string nome = "cb_" + item;
                    CheckBox checkbox = (CheckBox)this.FindName(nome);
                    if(checkbox!= null) checkbox.IsChecked = true;

                }
                
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
               
        }


        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox rb = (CheckBox)sender;
                if (rb == null) return;
                string tagId = rb.Tag.ToString();
                if (catsPai.Contains(tagId))
                {
                    ShowHideSubCat(true, tagId);
                }
                AltetarTag(true, tagId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            try {
                CheckBox rb = (CheckBox)sender;
                if (rb == null) return;
                string tagId = rb.Tag.ToString();
                if (catsPai.Contains(tagId))
                {
 
                ShowHideSubCat(false, tagId);
                }

                AltetarTag(false, tagId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void AltetarTag(bool add, string tag)
        {
  
            if (add)
            {
                listCats.Add(tag);
                CatList = string.Join(",", listCats.ToArray());
            }
            else
            {
                listCats.Remove(tag);
                CatList = string.Join(",", listCats.ToArray());

            }

        }
        private void ShowHideSubCat(bool show, string cat)
        {
            var stack = (StackPanel)containerCats.FindName($"stack{cat}");
            if(stack == null) return;
            stack.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
