using PainelPress.Classes;
using PainelPress.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        List<Categoria> categorias = new List<Categoria>();
        int[] catPai = { 71, 76, 58, 60, 87 };
        public static string CatList;

        public CategoriasCheck()
        {
            InitializeComponent();
            // Setap();
        }

        public CategoriasCheck(string[] cats)
        {
            InitializeComponent();
            listCats.Clear();
            SetCats(cats);
                   
        }

        private void Setap()
        {
            string cats = config.Default.categorias;
            if (cats != "")
            {
                categorias = Ferramentas.JsonToObjeto(cats, 2) as List<Categoria>;
                new Ferramentas().GetXamlCats(categorias);              
            }
        }

       

        private void SetCats(string[] cats)
        {    
 
            try
            {
                foreach (string item in cats)
                {
                    CheckBox checkbox = (CheckBox)this.FindName("ct" + item);
                    checkbox.IsChecked = true;
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
            int tagId = Convert.ToInt32(rb.Tag.ToString());
            if (catPai.Contains(tagId))
            {
                ShowHideSubCat(true, tagId);
            }
                AltetarTag(true, tagId.ToString());
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
            int tagId = Convert.ToInt32(rb.Tag.ToString());
            if (catPai.Contains(tagId))
            {
 
                ShowHideSubCat(false, tagId);
            }

                AltetarTag(false, tagId.ToString());
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

            Debug.WriteLine(CatList);
        }
        private void ShowHideSubCat(bool show, int cat)
        {
            if (cat == 71)
            {
                stackcentrooeste.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (cat == 76)
            {
                stacknorte.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (cat == 58)
            {
                stacknordeste.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (cat == 60)
            {
                stacksudeste.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            }
            else if (cat == 87)
            {
                stacksul.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
