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
using DataBase;
using Microsoft.Data.Sqlite;
using PainelPress.Model;
using PainelPress.View;

namespace PainelPress.Classes.Controller
{
    public class CtlDadosPagina
    {
        MainBase baseData;
        public CtlDadosPagina() {
            bool start = !File.Exists(Constants.PATHDB);
            baseData = new MainBase(Constants.PATHDB);
            if (start)
            {
                StartDB();
            }
        }

        public async void AdicionarPagina(string n, string u, string h, int f = 30)
        {
            try
            {
                var dados = new Dictionary<string, object>
               {
                 {"nome", n},
                 {"url", u},
                 {"html", h},
                 {"frequencia", f}
                };
                string qr = "INSERT INTO paginas (descricao,url,html, frequencia) VALUES (:nome,:url,:html,:frequencia);";
                await baseData.execQuery(qr, dados);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
        }

        public async void UpdatePagina(int id, string n, string u, int f = 30)
        {

            try
            {
                var dados = new Dictionary<string, object>
                {
                 {"id", id},
                 {"nome", n},
                 {"url", u},
                 {"frequencia", f}
                };
                string qr = "UPDATE paginas SET descricao = :nome, url = :url, frequencia = :frequencia WHERE id_pagina = :id;";
                await baseData.execQuery(qr, dados); ;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
        }

        public bool VerificaPagina(string html, int id)
        {

            try
            {
                var dados = new Dictionary<string, object>
                {
                 {":id", id}
                };
                string qr = "SELECT html FROM paginas WHERE id_pagina = :id LIMIT 1";
                var htmlAtual = baseData.execQueryResult(qr, dados);
                Ferramentas.PrintObjeto(htmlAtual);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
                return true;
            }
            return false;
        }

        public async Task<bool> AtualizarHtml(List<Pagina> list)
        {
            try
            {
                foreach (Pagina p in list)
                {
                    try
                    {
                        var dados = new Dictionary<string, object>
                        {
                         {"id", p.id_pagina},
                         {"html", p.html}
                        };
                        Ferramentas.PrintObjeto(p);
                        string qr = "UPDATE paginas SET html = :html WHERE id_pagina = :id;";
                        int valor = await baseData.execQuery(qr, dados);
                        if (valor ==0)
                        { 
                            return false;
                        }
                        await Task.Delay(500);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Erro");
                    }
                }

                return true;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("AtualizarHtml => " + ex.Message);
            }

            return false;
        }

        public async void DeletePagina(int id)
        {
            try
            {
                var dados = new Dictionary<string, object>
                {
                 {"id", id}
                };
                string qr = "DELETE FROM paginas WHERE id_pagina = :id;";
                await baseData.execQuery(qr, dados);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
        }

        public async Task<List<Pagina>> getPaginas()
        {
            string qr = "SELECT * FROM paginas LIMIT 50";
            var list = await baseData.execQueryResult(qr);
            if (list == null) return new List<Pagina>();
            return DataTabletoList(list);
        }

        public async Task<List<Pagina>> getPaginasMudanca()
        {
            string qr = "SELECT * FROM paginas LIMIT 50";
            var list = await baseData.execQueryResult(qr);
            if (list == null) return new List<Pagina>();
            return DataTabletoList(list);
        }

        public async Task<List<Pagina>> getPaginas(string nome)
        {
            string qr = $"SELECT * FROM paginas WHERE descricao LIKE '%{nome}%' LIMIT 50";
            var list = await baseData.execQueryResult(qr);
            if (list == null) return new List<Pagina>();
            return DataTabletoList(list);
        }

        private List<Pagina> DataTabletoList(DataTable data)
        {
            List<Pagina> listData = new List<Pagina>();
            foreach (DataRow item in data.Rows)
            {

                listData.Add(new Pagina
                {
                    id_pagina = Convert.ToInt32(item["id_pagina"]),
                    descricao = item["descricao"].ToString(),
                    url = item["url"].ToString(),
                    html = item["html"].ToString(),
                    frequencia = Convert.ToInt32(item["frequencia"]),
                });
            }
            return listData;
        }

        private async void StartDB()
        {
            string sites = "CREATE TABLE IF NOT EXISTS paginas (id_pagina INTEGER PRIMARY KEY AUTOINCREMENT, descricao TEXT, url TEXT, html TEXT, frequencia INTEGER, UNIQUE(url))";
            await baseData.execQuery(sites);
        }

    }

    public class Pagina
    {
        public int id_pagina { get; set; }
        public string descricao { get; set; }
        public string url { get; set; }
        public int frequencia { get; set; }
        public string html { get; set; }
    }

}
