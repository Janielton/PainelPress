using PainelPress.Paginas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PainelPress.Classes
{
    public class Menu
    {
        private static List<Menu> listAll;
        public string nome { get; set; }

        public string tag { get; set; }

        public int tipo { get; set; }
        
        public static List<Menu> ListaItems()
        {
            if (listAll != null) return listAll;
            List<Menu> menus = new List<Menu>()
        {
            new Menu()
            {
                nome = "Upload",
                tipo= 1,
                tag = "upload"
            },
             new Menu()
            {
                nome = "Analytics",
                tipo= 1,
                tag = "analytics"
            },
            new Menu()
            {
                nome = "FTP",
                tipo= 1,
                tag = "ftp"
            },
            new Menu()
            {
                nome = "Paginas",
                tipo= 1,
                tag = "paginas"
            },
            new Menu()
            {
                nome = "Social",
                tipo= 1,
                tag = "social"
            },
            new Menu()
            {
                nome = "Stories",
                tipo= 1,
                tag = "stories"
            },
            new Menu()
            {
                nome = "Backup",
                tipo= 1,
                tag = "backup"
            },
            new Menu()
            {
                nome = "Categorias",
                tipo= 1,
                tag = "categorias"
            },
            new Menu()
            {
                nome = "Leitor Feed",
                tipo= 1,
                tag = "feed"
            },
            new Menu()
            {
                nome = "ChatGPT",
                tipo= 1,
                tag = "gpt"
            },

        };
            listAll = menus;
            return menus;
        }

        public static List<Menu> ListaBusca(string palavra)
        {
            List<Menu> lista = new List<Menu>();
            if (palavra == "")
            {
                return ListaItems();
            }
            try
            {
                var lists = ListaItems().Where(p => p.nome.ToLower().Contains(palavra.ToLower())).ToList();
                lista = lists;

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return lista;
        }
    }
    

}
