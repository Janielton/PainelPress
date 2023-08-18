using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Shapes;
using DataBase;
using Microsoft.Data.Sqlite;
using PainelPress.Model;
using PainelPress.Paginas;

namespace PainelPress.Classes
{
    public class CtlDadosChat
    {
        MainBase baseData;
        public CtlDadosChat() {
            baseData = new MainBase(Constants.PATHDB);
        }

        public async Task<int> AdicionarChat(Chat item)
        {
            int id = 0;
            try
            {
              
               var dados = new Dictionary<string, object>
               {
                 {"titulo", item.titulo},
                 {"data", item.data},
                 {"tipo", item.tipo},
                };
                string qr = "INSERT INTO chats (titulo, data, tipo) VALUES (:titulo,:data,:tipo);";
                id = await baseData.execQuery(qr, dados, true);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
            return id;
        }

        public async void UpdateChat(Chat item)
        {

            try
            {
                var dados = new Dictionary<string, object>
                {
                  {"titulo", item.titulo},
                  {"data", item.data},
                  {"tipo", item.tipo},
                  {"id", item.id}
                };
                string qr = "UPDATE chats SET titulo = :titulo, data = :data, tipo = :tipo WHERE id_chat = :id;";
                await baseData.execQuery(qr, dados); ;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
        }

        public async Task<int> AdicionarChatItem(ChatItem item)
        {
            int idr = 0;
            try
            {
                var dados = new Dictionary<string, object>
               {
                 {"conteudo", item.conteudo},
                 {"data", item.data},
                 {"tipo", item.tipo},
                 {"id", item.id_chat},
                };
                string qr = "INSERT INTO chats_items (conteudo, data, tipo, id_chat) VALUES (:conteudo, :data, :tipo, :id);";
                idr = await baseData.execQuery(qr, dados);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
            return idr;
        }


        public async void DeleteChat(int id)
        {
            try
            {
                var dados = new Dictionary<string, object>
                {
                 {"id", id}
                };
                string qr = "DELETE FROM chats_items WHERE id_chat = :id;DELETE FROM chats WHERE id_chat = :id;";
                await baseData.execQuery(qr, dados);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
        }

        public async Task<List<Chat>> getChats()
        {
            string qr = "SELECT * FROM chats ORDER BY id_chat DESC LIMIT 50";
           // baseData = new MainBase(Constants.PATHDB);
            var list = await baseData.execQueryResult(qr);
            if (list == null) return new List<Chat>();
            return DataTabletoList(list);
        }

        public async Task<List<Chat>> getChats(string nome)
        {
            string qr = $"SELECT * FROM chats WHERE titulo LIKE '%{nome}%' LIMIT 50";
            var list = await baseData.execQueryResult(qr);
            if (list == null) return new List<Chat>();
            return DataTabletoList(list);
        }

        public async Task<List<ChatItem>> getItensChats(int id)
        {
            string qr = $"SELECT * FROM chats_items WHERE id_chat = {id} LIMIT 50";
            // baseData = new MainBase(Constants.PATHDB);
            var list = await baseData.execQueryResult(qr);
            if (list == null) return new List<ChatItem>();
            return DataTabletoListChat(list);
        }


        private List<Chat> DataTabletoList(DataTable data)
        {
            List<Chat> listData = new List<Chat>();
            foreach (DataRow item in data.Rows)
            {

                listData.Add(new Chat
                {
                    id = Convert.ToInt32(item["id_chat"].ToString()),
                    titulo = item["titulo"].ToString(),
                    data = item["data"].ToString(),
                    tipo = Convert.ToInt32(item["tipo"].ToString())
                });
            }
            return listData;
        }

        private List<ChatItem> DataTabletoListChat(DataTable data)
        {
            List<ChatItem> listData = new List<ChatItem>();
            foreach (DataRow item in data.Rows)
            {

                listData.Add(new ChatItem
                {
                    id = Convert.ToInt32(item["id"].ToString()),
                    conteudo = item["conteudo"].ToString(),
                    data = item["data"].ToString(),
                    tipo = Convert.ToInt32(item["tipo"].ToString())
                });
            }
            return listData;
        }

    }

    public class Chat {
        public int id { get; set; }
        public string titulo { get; set; }
        public string data { get; set; }
        public int tipo { get; set; }
    }

    public class ChatItem
    {
        public int id { get; set; }

        public int tipo { get; set; }

        public string conteudo { get; set; }
       
        public string data { get; set; }

        public int id_chat { get; set; }

        public bool isME { get{ return tipo == 1; } }
    }


}
