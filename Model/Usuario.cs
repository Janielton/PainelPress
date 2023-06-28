using Newtonsoft.Json;
using PainelPress.Elementos;
using System;
using System.Collections.Generic;
using System.Text;
using config = PainelPress.Properties.Settings;

namespace PainelPress.Model
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        
        public Usuario()
        {

        }

        public static List<Usuario> listaUsuarios()
        {
            try
            {

                return JsonConvert.DeserializeObject<List<Usuario>>(config.Default.usuarios);
            }
            catch(Exception e)
            {
                AlertMensagem.instance.Show(e.Message,"Erro ao pegar usuarios");
            }
            return new List<Usuario>();
        }

        public static List<Usuario> listaUsuariosAnterior()
        {
            List<Usuario> lista = new List<Usuario>();
            lista.Add(new Usuario { Id = 1, Nome = "Dimas Devan", Username = "Dimas Devan", Password = "" });
            lista.Add(new Usuario { Id = 2, Nome = "Joice Ferreira", Username = "Joice", Password = "" });

            return lista;
        }

        public static Usuario getUsuario(string id)
        {
            try
            {
                int ID = Convert.ToInt32(id);
                List<Usuario> lista = listaUsuarios();
                foreach (var item in lista)
                {
                    if (item.Id == ID) return item;
                }
            }catch (Exception ex)
            {
                return null;
            }
            return null;

        }
    }
}
