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
   
            baseData = new MainBase(Constants.PATHDB);
        }

        public async Task<bool> AdicionarPagina(Pagina page)
        {
            try
            {
                var dados = new Dictionary<string, object>
                {
                 {"nome", page.descricao},
                 {"url", page.url},
                 {"html", page.html},
                 {"request", page.request},
                 {"parametro", page.parametro}
                };
                string qr = "INSERT INTO paginas (descricao, url, html, request, parametro) VALUES (:nome,:url,:html,:request, :parametro);";
                await baseData.execQuery(qr, dados);
                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
            return false;
        }

        public async Task<bool> UpdatePagina(Pagina page)
        {

            try
            {
                var dados = new Dictionary<string, object>
                {
                 {"id", page.id_pagina},
                 {"nome", page.descricao},
                 {"url", page.url},
                 {"html", page.html},
                 {"request", page.request},
                 {"parametro", page.parametro}
                };
                string qr = "UPDATE paginas SET descricao = :nome, url = :url, request = :request, parametro = :parametro WHERE id_pagina = :id;";
                await baseData.execQuery(qr, dados);
                return true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
            return false;
        }

        public bool VerificaPagina(string html, int id)
        {

            try
            {
                var dados = new Dictionary<string, object>
                {
                 {"id", id}
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
                    request = item["request"].ToString(),
                    parametro = item["parametro"].ToString(),
                });
            }
            return listData;
        }


    }

    public class Pagina
    {
        public int id_pagina { get; set; }
        public string descricao { get; set; }
        public string url { get; set; }
        public string request { get; set; }
        public string html { get; set; }
        public string parametro { get; set; }
    }

}
