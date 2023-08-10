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
    public class CtlDadosSites
    {
        MainBase baseData;
        public CtlDadosSites() {
            baseData = new MainBase(Constants.PATHDB);
        }

        public async void AdicionarSite(Site site)
        {
            try
            {
               var dados = new Dictionary<string, object>
               {
                 {"nome", site.nome},
                 {"url", site.url},
                 {"feed",site.feed},
                 {"atualizacao", site.atualizacao}
                };
                string qr = "INSERT INTO sites (nome, url, feed, atualizacao) VALUES (:nome,:url,:feed,:atualizacao);";
                await baseData.execQuery(qr, dados);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
        }

        public async void UpdateSite(Site site)
        {

            try
            {
                var dados = new Dictionary<string, object>
                {
                  {"nome", site.nome},
                  {"url", site.url},
                  {"feed",site.feed},
                  {"atualizacao", site.atualizacao},
                  {"id", site.id}
                };
                string qr = "UPDATE sites SET nome = :nome, url = :url, feed = :feed, atualizacao = :atualizacao WHERE id_site = :id;";
                await baseData.execQuery(qr, dados); ;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
        }

        public async void UpdateDateSite(int id, string data)
        {

            try
            {
                var dados = new Dictionary<string, object>
                {
                  {"atualizacao", data},
                  {"id", id}
                };
                string qr = "UPDATE sites SET atualizacao = :atualizacao WHERE id_site = :id;";

                await baseData.execQuery(qr, dados);
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
        }

        public async void DeleteSite(int id)
        {
            try
            {
                var dados = new Dictionary<string, object>
                {
                 {"id", id}
                };
                string qr = "DELETE FROM sites WHERE id_site = :id;";
                await baseData.execQuery(qr, dados);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
        }

        public async Task<List<Site>> getSites()
        {
            string qr = "SELECT * FROM sites LIMIT 50";
           // baseData = new MainBase(Constants.PATHDB);
            var list = await baseData.execQueryResult(qr);
            if (list == null) return new List<Site>();
            return DataTabletoList(list);
        }

        public async Task<List<Site>> getSites(string nome)
        {
            string qr = $"SELECT * FROM sites WHERE nome LIKE '%{nome}%' LIMIT 50";
            var list = await baseData.execQueryResult(qr);
            if (list == null) return new List<Site>();
            return DataTabletoList(list);
        }

        private List<Site> DataTabletoList(DataTable data)
        {
            List<Site> listData = new List<Site>();
            foreach (DataRow item in data.Rows)
            {

                listData.Add(new Site
                {
                    id = Convert.ToInt32(item["id_site"].ToString()),
                    nome = item["nome"].ToString(),
                    url = item["url"].ToString(),
                    feed = item["feed"].ToString(),
                    atualizacao = item["atualizacao"].ToString(),
                });
            }
            return listData;
        }

    }

}
