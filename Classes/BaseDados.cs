using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using PainelPress.Model;
using DataBase;

namespace PainelPress.Classes
{
    public class BaseDados
    {
       // string pathDB = @"C:\WPanel\BaseDados.sqlite";


      

        public BaseDados()
        {
            bool start = !File.Exists(Constants.PATHDB);
            if (start)
            {
                StartDB();
            }
        }

        private async void StartDB()
        {
            MainBase db = new MainBase(Constants.PATHDB);
            string sites = "CREATE TABLE IF NOT EXISTS sites (id_site INTEGER PRIMARY KEY AUTOINCREMENT, nome TEXT, url TEXT, feed TEXT, atualizacao TEXT, UNIQUE(url))";
            await db.execQuery(sites);
            string paginas = "CREATE TABLE IF NOT EXISTS paginas (id_pagina INTEGER PRIMARY KEY AUTOINCREMENT, descricao TEXT, url TEXT, html TEXT, request TEXT, parametro TEXT, UNIQUE(url))";
            await db.execQuery(paginas);
        }


        #region INSERT

        public async Task SalvarTaxonomy(string tipo, List<Taxonomy> lista)
        {
            try{
               
                //var cmd = new SqliteCommand();
                //cmd.Connection = conexaoDB;
                //conexaoDB.Open();
                //if (!Reset(tipo, cmd)) return;
                //foreach (var item in lista)
                //{
                //    cmd.CommandText = string.Format("INSERT INTO {0} (term_id, nome, slug) VALUES (@Id, @Nome, @Slug);", tipo);
                //    cmd.Parameters.Add("@Id", SqliteType.Text, 10).Value = item.TermId;
                //    cmd.Parameters.Add("@Nome", SqliteType.Text, 50).Value = item.Name;
                //    cmd.Parameters.Add("@Slug", SqliteType.Text, 50).Value = item.Slug;
                //    await cmd.ExecuteNonQueryAsync();
                //    cmd.Parameters.Clear();
                //    await Task.Delay(1);
                //}
     
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }
            finally
            {
                //conexaoDB.Close();
            }
            
        }

        public bool Reset(string table, SqliteCommand cmd)
        {

            try
            {
                cmd.CommandText = $"DELETE FROM {table};";
                cmd.ExecuteNonQuery();
                cmd.CommandText = $"UPDATE sqlite_sequence SET seq=0 WHERE name='{table}'";
                cmd.ExecuteNonQuery();
                cmd.CommandText = "VACUUM";
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Reset=>" + e.Message);
                return false;
            }

            return true;
        }

        public async Task SalvarTopicos(List<Inscritos> lista)
        {
            try
            {
       
                //var cmd = new SqliteCommand();
                //cmd.Connection = conexaoDB;
                //conexaoDB.Open();
                //foreach (var item in lista)
                //{
                //    cmd.CommandText = string.Format("INSERT INTO {0} (topico, nome) VALUES (@Id, @Nome);", "topicos");
                //    cmd.Parameters.Add("@Id", SqliteType.Text, 10).Value = item.Topico;
                //    cmd.Parameters.Add("@Nome", SqliteType.Text, 50).Value = item.Nome;
                //    await cmd.ExecuteNonQueryAsync();
                //    cmd.Parameters.Clear();
                //    await Task.Delay(1);
                //}

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }
            finally
            {
                //conexaoDB.Close();
            }

        }

        public async Task SalvarTag(Tag tag)
        {
            try
            {
                //var cmd = new SqliteCommand();
                //cmd.Connection = conexaoDB;
                //conexaoDB.Open();
                //cmd.CommandText = string.Format("INSERT INTO {0} (term_id, nome, slug) VALUES (@Id, @Nome, @Slug);", "tags");
                //cmd.Parameters.Add("@Id", SqliteType.Text, 10).Value = tag.Id;
                //cmd.Parameters.Add("@Nome", SqliteType.Text, 50).Value = tag.Name;
                //cmd.Parameters.Add("@Slug", SqliteType.Text, 50).Value = tag.Slug;
                //await cmd.ExecuteNonQueryAsync();
                //cmd.Parameters.Clear();
                //await Task.Delay(1);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }
            finally
            {
                //conexaoDB.Close();
            }

        }

        #endregion

        #region Listas

        public async Task<List<Taxonomy>> ListaTaxnomy(string palavra, int tipo)
        {
            List<Taxonomy> dtResultado = new List<Taxonomy>();
            //try
            //{
            //    conexaoDB.Open();
            //    string query = string.Format("SELECT * FROM {0} WHERE nome LIKE '%{1}%' LIMIT 10",Constants.taxonomy[tipo], palavra);

            //    var sqCommand = new SqliteCommand(query, conexaoDB);
            //    using (var reader = await sqCommand.ExecuteReaderAsync())
            //    {
            //        Taxonomy taxonomytem = null;
            //        while (await reader.ReadAsync())
            //        {
            //            taxonomytem = new Taxonomy();
            //            taxonomytem.TermId = reader.GetString(1);
            //            taxonomytem.Name = reader.GetString(2);
            //            taxonomytem.Slug = reader.GetString(3);
            //            dtResultado.Add(taxonomytem);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine("ERRO DB" +ex.Message);
            //}
            //finally
            //{
            //    conexaoDB.Close();
            //}

            return dtResultado;
        }

        public async Task<List<Taxonomy>> ListaCidades(string palavra)
        {
            List<Taxonomy> dtResultado = new List<Taxonomy>();
            //try
            //{
            //    conexaoDB.Open();
            //    string query = string.Format("SELECT * FROM cidades WHERE nome LIKE '%{0}%' LIMIT 10", palavra);

            //    var sqCommand = new SqliteCommand(query, conexaoDB);
            //    using (var reader = await sqCommand.ExecuteReaderAsync())
            //    {
            //        Taxonomy taxonomytem = null;
            //        while (await reader.ReadAsync())
            //        {
            //            taxonomytem = new Taxonomy();
            //            taxonomytem.TermId = reader.GetString(1);
            //            taxonomytem.Name = reader.GetString(2);
            //            taxonomytem.Slug = reader.GetString(3);
            //            dtResultado.Add(taxonomytem);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine("ERRO DB" + ex.Message);
            //}
            //finally
            //{
            //    conexaoDB.Close();
            //}

            return dtResultado;
        }
        public async Task<List<Taxonomy>> ListaCargos(string palavra)
        {
            List<Taxonomy> dtResultado = new List<Taxonomy>();
            //try
            //{
            //    conexaoDB.Open();
            //    string query = string.Format("SELECT * FROM cargos WHERE nome LIKE '%{0}%' LIMIT 10", palavra);

            //    var sqCommand = new SqliteCommand(query, conexaoDB);
            //    using (var reader = await sqCommand.ExecuteReaderAsync())
            //    {
            //        Taxonomy taxonomytem = null;
            //        while (await reader.ReadAsync())
            //        {
            //            taxonomytem = new Taxonomy();
            //            taxonomytem.TermId = reader.GetString(1);
            //            taxonomytem.Name = reader.GetString(2);
            //            taxonomytem.Slug = reader.GetString(3);
            //            dtResultado.Add(taxonomytem);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine("ERRO DB" + ex.Message);
            //}
            //finally
            //{
            //    conexaoDB.Close();
            //}

            return dtResultado;
        }

        public async Task<List<Taxonomy>> ListaKeyWords(string palavra)
        {
            List<Taxonomy> dtResultado = new List<Taxonomy>();
            //try
            //{
            //    conexaoDB.Open();
            //    string query = string.Format("SELECT * FROM cidades WHERE nome LIKE '%{0}%' LIMIT 10", palavra);

            //    var sqCommand = new SqliteCommand(query, conexaoDB);
            //    using (var reader = await sqCommand.ExecuteReaderAsync())
            //    {
            //        Taxonomy taxonomytem = null;
            //        while (await reader.ReadAsync())
            //        {
            //            taxonomytem = new Taxonomy();
            //            taxonomytem.TermId = reader.GetString(1);
            //            taxonomytem.Name = reader.GetString(2);
            //            taxonomytem.Slug = reader.GetString(3);
            //            dtResultado.Add(taxonomytem);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine("ERRO DB" + ex.Message);
            //}
            //finally
            //{
            //    conexaoDB.Close();
            //}

            return dtResultado;
        }

       


        #endregion

        #region GetItem

        public Taxonomy getTag(string palavra)
        {
            Taxonomy dtResultado = null;
            //try
            //{
            //    conexaoDB.Open();
            //    string query = string.Format($"SELECT * FROM {Constants.taxonomy[0]} WHERE nome LIKE '{0}' LIMIT 10", palavra);

            //    var sqCommand = new SqliteCommand(query, conexaoDB);
            //    using (var reader = sqCommand.ExecuteReader())
            //    {
            //        Taxonomy taxonomytem = null;
            //        if (reader.Read())
            //        {
            //            taxonomytem = new Taxonomy();
            //            taxonomytem.TermId = reader.GetString(1);
            //            taxonomytem.Name = reader.GetString(2);
            //            taxonomytem.Slug = reader.GetString(3);
            //            dtResultado = taxonomytem;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine("ERRO DB" + ex.Message);
            //}
            //finally
            //{
            //    conexaoDB.Close();
            //}

            return dtResultado;
        }

        public Taxonomy getTagById(string id)
        {
            Taxonomy dtResultado = null;
            //try
            //{
            //    conexaoDB.Open();
            //    string query = string.Format("SELECT * FROM {0} WHERE term_id = {1} LIMIT 1", Constants.taxonomy[0], id);

            //    var sqCommand = new SqliteCommand(query, conexaoDB);
            //    using (var reader = sqCommand.ExecuteReader())
            //    {
            //        if (reader.Read())
            //        {     
            //            dtResultado = new Taxonomy();
            //            dtResultado.TermId = reader.GetString(1);
            //            dtResultado.Name = reader.GetString(2);
            //            dtResultado.Slug = reader.GetString(3);
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Debug.WriteLine("ERRO DB" + ex.Message);
            //}
            //finally
            //{
            //    conexaoDB.Close();
            //}

            return dtResultado;
        }

        #endregion


    }
}
