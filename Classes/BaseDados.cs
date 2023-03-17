using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using PainelPress.Model;

namespace PainelPress.Classes
{
    public class BaseDados
    {
       // string pathDB = @"C:\WPanel\BaseDados.sqlite";
        public static readonly SqliteConnection conexaoDB = new SqliteConnection("Data Source=" + App.BasePasta + "");

       

        public BaseDados()
        {

        }

        #region START
        public bool ExisteTabela()
        {
            try
            {
                if (File.Exists(App.BasePasta))
                {
                    string sql = "SELECT seq FROM sqlite_sequence WHERE name = 'config'";
                    var cmd = new SqliteCommand(sql, conexaoDB);
                    conexaoDB.Open();
                    int temp = (int)cmd.ExecuteScalar();
                    Debug.WriteLine(temp);
                   
                    if (temp > 0)
                    {
                        return true;
                    }

                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Erro ao verificar tabela - " + e.Message, "Ocorreu um erro");
               
                return false;
            }
            finally
            {
                conexaoDB.Close();
            }

            return false;

        }

        public async void CriaTabela()
        {

            if (!ExisteTabela())
            {
                try
                {
                    conexaoDB.Open();

                    var cmd = new SqliteCommand();
                    cmd.Connection = conexaoDB;
                    foreach(string tax in Constants.taxonomy)
                    {
                        cmd.CommandText = $"CREATE TABLE IF NOT EXISTS {tax} (id INTEGER PRIMARY KEY AUTOINCREMENT, term_id TEXT, nome TEXT, slug TEXT);";

                        await cmd.ExecuteScalarAsync();

                    }
                    
                    cmd.CommandText = "INSERT INTO sqlite_sequence (name, seq) VALUES ('config',1)";
                    await cmd.ExecuteScalarAsync();

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    conexaoDB.Close();
                }

            }

        }

        public async void AddTabela(string nome, string param)
        {
            try
            {

                var cmd = new SqliteCommand();
                cmd.Connection = conexaoDB;
                cmd.CommandText = string.Format("CREATE TABLE IF NOT EXISTS {0} ({1});", nome, param);
                conexaoDB.Open();
                await cmd.ExecuteScalarAsync();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                conexaoDB.Close();
            }
        }

        public bool ClearTabela(int tipo)
        {
            try
            {
                string tabela = tipo == 1 ? "tags" : tipo == 2 ? "cidades" : "cargos";
                conexaoDB.Open();
                var cmd = new SqliteCommand();
                cmd.Connection = conexaoDB;
                cmd.CommandText = string.Format("DELETE FROM {0}", tabela);
                cmd.ExecuteNonQuery();
                cmd.CommandText = string.Format("UPDATE sqlite_sequence SET seq = 0 WHERE name = '{0}'", tabela);
                cmd.ExecuteNonQuery();
                cmd.CommandText = string.Format("VACUUM");
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                conexaoDB.Close();
            }
            return true;
        }

        #endregion

        #region INSERT

        public async Task SalvarTaxonomy(string tipo, List<Taxonomy> lista)
        {
            try{
               
                var cmd = new SqliteCommand();
                cmd.Connection = conexaoDB;
                conexaoDB.Open();
                if (!Reset(tipo, cmd)) return;
                foreach (var item in lista)
                {
                    cmd.CommandText = string.Format("INSERT INTO {0} (term_id, nome, slug) VALUES (@Id, @Nome, @Slug);", tipo);
                    cmd.Parameters.Add("@Id", SqliteType.Text, 10).Value = item.TermId;
                    cmd.Parameters.Add("@Nome", SqliteType.Text, 50).Value = item.Name;
                    cmd.Parameters.Add("@Slug", SqliteType.Text, 50).Value = item.Slug;
                    await cmd.ExecuteNonQueryAsync();
                    cmd.Parameters.Clear();
                    await Task.Delay(1);
                }
     
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }
            finally
            {
                conexaoDB.Close();
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
       
                var cmd = new SqliteCommand();
                cmd.Connection = conexaoDB;
                conexaoDB.Open();
                foreach (var item in lista)
                {
                    cmd.CommandText = string.Format("INSERT INTO {0} (topico, nome) VALUES (@Id, @Nome);", "topicos");
                    cmd.Parameters.Add("@Id", SqliteType.Text, 10).Value = item.Topico;
                    cmd.Parameters.Add("@Nome", SqliteType.Text, 50).Value = item.Nome;
                    await cmd.ExecuteNonQueryAsync();
                    cmd.Parameters.Clear();
                    await Task.Delay(1);
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }
            finally
            {
                conexaoDB.Close();
            }

        }

        public async Task SalvarTag(Tag tag)
        {
            try
            {
                var cmd = new SqliteCommand();
                cmd.Connection = conexaoDB;
                conexaoDB.Open();
                cmd.CommandText = string.Format("INSERT INTO {0} (term_id, nome, slug) VALUES (@Id, @Nome, @Slug);", "tags");
                cmd.Parameters.Add("@Id", SqliteType.Text, 10).Value = tag.Id;
                cmd.Parameters.Add("@Nome", SqliteType.Text, 50).Value = tag.Name;
                cmd.Parameters.Add("@Slug", SqliteType.Text, 50).Value = tag.Slug;
                await cmd.ExecuteNonQueryAsync();
                cmd.Parameters.Clear();
                await Task.Delay(1);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }
            finally
            {
                conexaoDB.Close();
            }

        }

        #endregion

        #region Listas

        public async Task<List<Taxonomy>> ListaTaxnomy(string palavra, int tipo)
        {
            List<Taxonomy> dtResultado = new List<Taxonomy>();
            try
            {
                conexaoDB.Open();
                string query = string.Format("SELECT * FROM {0} WHERE nome LIKE '%{1}%' LIMIT 10",Constants.taxonomy[tipo], palavra);

                var sqCommand = new SqliteCommand(query, conexaoDB);
                using (var reader = await sqCommand.ExecuteReaderAsync())
                {
                    Taxonomy taxonomytem = null;
                    while (await reader.ReadAsync())
                    {
                        taxonomytem = new Taxonomy();
                        taxonomytem.TermId = reader.GetString(1);
                        taxonomytem.Name = reader.GetString(2);
                        taxonomytem.Slug = reader.GetString(3);
                        dtResultado.Add(taxonomytem);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO DB" +ex.Message);
            }
            finally
            {
                conexaoDB.Close();
            }

            return dtResultado;
        }

        public async Task<List<Taxonomy>> ListaCidades(string palavra)
        {
            List<Taxonomy> dtResultado = new List<Taxonomy>();
            try
            {
                conexaoDB.Open();
                string query = string.Format("SELECT * FROM cidades WHERE nome LIKE '%{0}%' LIMIT 10", palavra);

                var sqCommand = new SqliteCommand(query, conexaoDB);
                using (var reader = await sqCommand.ExecuteReaderAsync())
                {
                    Taxonomy taxonomytem = null;
                    while (await reader.ReadAsync())
                    {
                        taxonomytem = new Taxonomy();
                        taxonomytem.TermId = reader.GetString(1);
                        taxonomytem.Name = reader.GetString(2);
                        taxonomytem.Slug = reader.GetString(3);
                        dtResultado.Add(taxonomytem);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO DB" + ex.Message);
            }
            finally
            {
                conexaoDB.Close();
            }

            return dtResultado;
        }
        public async Task<List<Taxonomy>> ListaCargos(string palavra)
        {
            List<Taxonomy> dtResultado = new List<Taxonomy>();
            try
            {
                conexaoDB.Open();
                string query = string.Format("SELECT * FROM cargos WHERE nome LIKE '%{0}%' LIMIT 10", palavra);

                var sqCommand = new SqliteCommand(query, conexaoDB);
                using (var reader = await sqCommand.ExecuteReaderAsync())
                {
                    Taxonomy taxonomytem = null;
                    while (await reader.ReadAsync())
                    {
                        taxonomytem = new Taxonomy();
                        taxonomytem.TermId = reader.GetString(1);
                        taxonomytem.Name = reader.GetString(2);
                        taxonomytem.Slug = reader.GetString(3);
                        dtResultado.Add(taxonomytem);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO DB" + ex.Message);
            }
            finally
            {
                conexaoDB.Close();
            }

            return dtResultado;
        }

        public async Task<List<Taxonomy>> ListaKeyWords(string palavra)
        {
            List<Taxonomy> dtResultado = new List<Taxonomy>();
            try
            {
                conexaoDB.Open();
                string query = string.Format("SELECT * FROM cidades WHERE nome LIKE '%{0}%' LIMIT 10", palavra);

                var sqCommand = new SqliteCommand(query, conexaoDB);
                using (var reader = await sqCommand.ExecuteReaderAsync())
                {
                    Taxonomy taxonomytem = null;
                    while (await reader.ReadAsync())
                    {
                        taxonomytem = new Taxonomy();
                        taxonomytem.TermId = reader.GetString(1);
                        taxonomytem.Name = reader.GetString(2);
                        taxonomytem.Slug = reader.GetString(3);
                        dtResultado.Add(taxonomytem);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO DB" + ex.Message);
            }
            finally
            {
                conexaoDB.Close();
            }

            return dtResultado;
        }

        public async Task<List<Inscritos>> ListaInscritos(string ids)
        {
            List<Inscritos> dtResultado = new List<Inscritos>();
            try
            {
                conexaoDB.Open();
                string query = string.Format("SELECT topico, nome FROM topicos WHERE topico IN ({0})", ids);

                var sqCommand = new SqliteCommand(query, conexaoDB);
                using (var reader = await sqCommand.ExecuteReaderAsync())
                {
                    Inscritos taxonomytem = null;
                    while (await reader.ReadAsync())
                    {
                        taxonomytem = new Inscritos();
                        taxonomytem.Topico = reader.GetInt32(0);
                        taxonomytem.Nome = reader.GetString(1);
                        dtResultado.Add(taxonomytem);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO DB" + ex.Message);
            }
            finally
            {
                conexaoDB.Close();
            }

            return dtResultado;
        }


        #endregion

        #region GetItem

        public Taxonomy getTag(string palavra)
        {
            Taxonomy dtResultado = null;
            try
            {
                conexaoDB.Open();
                string query = string.Format($"SELECT * FROM {Constants.taxonomy[0]} WHERE nome LIKE '{0}' LIMIT 10", palavra);

                var sqCommand = new SqliteCommand(query, conexaoDB);
                using (var reader = sqCommand.ExecuteReader())
                {
                    Taxonomy taxonomytem = null;
                    if (reader.Read())
                    {
                        taxonomytem = new Taxonomy();
                        taxonomytem.TermId = reader.GetString(1);
                        taxonomytem.Name = reader.GetString(2);
                        taxonomytem.Slug = reader.GetString(3);
                        dtResultado = taxonomytem;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO DB" + ex.Message);
            }
            finally
            {
                conexaoDB.Close();
            }

            return dtResultado;
        }

        public Taxonomy getTagById(string id)
        {
            Taxonomy dtResultado = null;
            try
            {
                conexaoDB.Open();
                string query = string.Format("SELECT * FROM {0} WHERE term_id = {1} LIMIT 1", Constants.taxonomy[0], id);

                var sqCommand = new SqliteCommand(query, conexaoDB);
                using (var reader = sqCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {     
                        dtResultado = new Taxonomy();
                        dtResultado.TermId = reader.GetString(1);
                        dtResultado.Name = reader.GetString(2);
                        dtResultado.Slug = reader.GetString(3);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO DB" + ex.Message);
            }
            finally
            {
                conexaoDB.Close();
            }

            return dtResultado;
        }

        #endregion


        #region EMAIL

        public bool IsStarted()
        {

            try
            {
                var cmd = new SqliteCommand();
                cmd.Connection = conexaoDB;
                cmd.CommandText = string.Format("SELECT COUNT(id) FROM registros;");
                conexaoDB.Open();
                long operacao = (long) cmd.ExecuteScalar();
               // conexaoDB.Close();
                if (operacao > 0) return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                conexaoDB.Close();
            }
            return false;

        }

        public long getQuantidade(int regiao)
        {
            string where = regiao > 0 ? string.Format(" WHERE regiao = {0}",regiao) : "";
            long quantidade = 0;
            try
            {
                var cmd = new SqliteCommand();
                cmd.Connection = conexaoDB;
                conexaoDB.Open();
                cmd.CommandText = string.Format("SELECT COUNT(id) FROM registros{0};", where);

                quantidade = (long) cmd.ExecuteScalar();
         
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                conexaoDB.Close();            
            }
            return quantidade;
        }

        public bool AddRegistro(Registros item)
        {
            try
            {
                var cmd = new SqliteCommand();
                cmd.Connection = conexaoDB;
                conexaoDB.Open();
                cmd.CommandText = string.Format("INSERT INTO {0} (id_registro, nome, email, status, regiao, inscricao_data) VALUES (@IdRegistro, @Nome, @Email, @Status, @Regiao, @InscricaoData);", "registros");
                cmd.Parameters.Add("@IdRegistro", SqliteType.Integer, 6).Value = item.Id;
                cmd.Parameters.Add("@Nome", SqliteType.Text, 50).Value = item.Nome;
                cmd.Parameters.Add("@Email", SqliteType.Text, 50).Value = item.Email;
                cmd.Parameters.Add("@InscricaoData", SqliteType.Text, 20).Value = item.InscricaoData;
                cmd.Parameters.Add("@Status", SqliteType.Integer, 1).Value = item.Status;
                cmd.Parameters.Add("@Regiao", SqliteType.Integer, 2).Value = item.Regiao;

                int operacao = cmd.ExecuteNonQuery();
                if (operacao > 0) return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                conexaoDB.Close();
            }
            return false;

        }

        public bool AlterarStatus(string email, int status)
        {
            try
            {
                var cmd = new SqliteCommand();
                cmd.Connection = conexaoDB;
                conexaoDB.Open();
                cmd.CommandText = string.Format("UPDATE {0} SET status = @Status WHERE email = @Email;", "registros");
                cmd.Parameters.Add("@Email", SqliteType.Text, 50).Value = email;
                cmd.Parameters.Add("@Status", SqliteType.Integer, 1).Value = status;
       
                int operacao = cmd.ExecuteNonQuery();
                if (operacao > 0) return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                conexaoDB.Close();
            }
            return false;

        }

        public async Task ApagarTabelaRegistros()
        {
            try
            {
                var cmd = new SqliteCommand();
                cmd.Connection = conexaoDB;
                cmd.CommandText = "DROP TABLE `registros`";
               await conexaoDB.OpenAsync();
                await cmd.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
               await conexaoDB.CloseAsync();
            }

        }

        public async Task ApagarRegistrosTabela(int regiao)
        {
            try
            {
                var cmd = new SqliteCommand();
                cmd.Connection = conexaoDB;
                cmd.CommandText = string.Format("DELETE FROM `registros` WHERE regiao = {0}", regiao);
                await conexaoDB.OpenAsync();
                await cmd.ExecuteScalarAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
              //  await conexaoDB.CloseAsync();
            }

        }

        public async Task CriaTabelaRegistros()
        {    
                try
                {
                    var cmd = new SqliteCommand();
                    cmd.Connection = conexaoDB;
                    cmd.CommandText = "CREATE TABLE IF NOT EXISTS 'registros' (id INTEGER PRIMARY KEY AUTOINCREMENT, id_registro INTEGER UNIQUE, nome TEXT, email TEXT, status INTEGER, regiao INTEGER, inscricao_data TEXT);";
                await conexaoDB.OpenAsync();
                await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
               // await conexaoDB.CloseAsync();
                }

        }

        public bool ApagarRegistro(int id)
        {
            try
            {
                var cmd = new SqliteCommand();
                cmd.Connection = conexaoDB;
                cmd.CommandText = string.Format("DELETE FROM 'registros' WHERE id_registro = {0};", id);
                conexaoDB.Open();
                int operacao = cmd.ExecuteNonQuery();
                if (operacao > 0) return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                conexaoDB.Close();
            }
            return false;

        }
       
      

        public async Task<int> Sincronize(List<Registros> listItems, int regiao)
        {
            int size = 0;
            try
            {
                var cmd = new SqliteCommand();
                cmd.Connection = conexaoDB;
                await ApagarRegistrosTabela(regiao);


                foreach (var item in listItems)
                {

                    cmd.CommandText = string.Format("INSERT INTO {0} (id_registro, nome, email, status, regiao, inscricao_data) VALUES (@IdRegistro, @Nome, @Email, @Status, @Regiao, @InscricaoData);", "registros");
                    cmd.Parameters.Add("@IdRegistro", SqliteType.Integer, 6).Value = item.Id;
                    cmd.Parameters.Add("@Nome", SqliteType.Text, 50).Value = item.Nome;
                    cmd.Parameters.Add("@Email", SqliteType.Text, 50).Value = item.Email;
                    cmd.Parameters.Add("@InscricaoData", SqliteType.Text, 20).Value = item.InscricaoData;
                    cmd.Parameters.Add("@Status", SqliteType.Integer, 1).Value = item.Status;
                    cmd.Parameters.Add("@Regiao", SqliteType.Integer, 2).Value = item.Regiao;

                    await cmd.ExecuteNonQueryAsync();
                    cmd.Parameters.Clear();
                    size++;
                    await Task.Delay(1);
                }
             
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                await conexaoDB.CloseAsync();

            }
            return size;
        }


        public async Task<RegistroCompleto> ListaRegistros(int regiao)
        {
            List<Registros> dtResultado = new List<Registros>();
            int size = 0;
            try
            {
                conexaoDB.Open();
      
                string query = string.Format("SELECT * FROM registros WHERE regiao = {0} ORDER BY id_registro DESC LIMIT 50", regiao);

                var sqCommand = new SqliteCommand(query, conexaoDB);
                using (var reader = await sqCommand.ExecuteReaderAsync())
                {
                    Registros item = null;
                    while (await reader.ReadAsync())
                    {
                        item = new Registros();
                        item.Id = reader.GetInt32(1);
                        item.Nome = reader.GetString(2);
                        item.Email = reader.GetString(3);
                        item.Status = reader.GetInt32(4);
                        item.Regiao = reader.GetInt32(5);
                        item.InscricaoData = reader.GetString(6);
                        dtResultado.Add(item);
                    }
                }
                string query2 = string.Format("SELECT COUNT(id) as quantidade FROM registros WHERE regiao = {0}", regiao);
                sqCommand.CommandText = query2;
                var sizeObject = await sqCommand.ExecuteScalarAsync();
                size = Convert.ToInt32(sizeObject.ToString());
       
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO DB" + ex.Message);
            }
            finally
            {
                conexaoDB.Close();
            }
           
            return new RegistroCompleto(dtResultado, size);
        }

        public async Task<RegistroCompleto> ListaRegistros(string palavra)
        {
            List<Registros> dtResultado = new List<Registros>();
            int size = 0;
            try
            {
                conexaoDB.Open();
                string query = string.Format("SELECT * FROM registros WHERE email LIKE '%{0}%' LIMIT 50", palavra);

                var sqCommand = new SqliteCommand(query, conexaoDB);
                using (var reader = await sqCommand.ExecuteReaderAsync())
                {
                    Registros item = null;
                    while (await reader.ReadAsync())
                    {
                        item = new Registros();
                        item.Id = reader.GetInt32(1);
                        item.Nome = reader.GetString(2);
                        item.Email = reader.GetString(3);
                        item.Status = reader.GetInt32(4);
                        item.Regiao = reader.GetInt32(5);
                        item.InscricaoData = reader.GetString(6);
                        dtResultado.Add(item);
                    }
                }
                string query2 = string.Format("SELECT COUNT(id) as quantidade FROM registros WHERE email LIKE '%{0}%'", palavra);
                sqCommand.CommandText = query2;
                var sizeObject = await sqCommand.ExecuteScalarAsync();
                size = Convert.ToInt32(sizeObject.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO DB" + ex.Message);
            }
            finally
            {
                conexaoDB.Close();
            }

            return new RegistroCompleto(dtResultado, size);
        }

        public async Task<RegistroCompleto> ListaRegistros(int regiao, string palavra)
        {
            List<Registros> dtResultado = new List<Registros>();
            int size = 0;
            try
            {
                conexaoDB.Open();
                string query = string.Format("SELECT * FROM registros WHERE regiao = {0} AND email LIKE '%{1}%' LIMIT 50", regiao, palavra);

                var sqCommand = new SqliteCommand(query, conexaoDB);
                using (var reader = await sqCommand.ExecuteReaderAsync())
                {
                    Registros item = null;
                    while (await reader.ReadAsync())
                    {
                        item = new Registros();
                        item.Id = reader.GetInt32(1);
                        item.Nome = reader.GetString(2);
                        item.Email = reader.GetString(3);
                        item.Status = reader.GetInt32(4);
                        item.Regiao = reader.GetInt32(5);
                        item.InscricaoData = reader.GetString(6);
                        dtResultado.Add(item);
                    }
                }
                string query2 = string.Format("SELECT COUNT(id) as quantidade FROM registros WHERE regiao = {0} AND email LIKE '%{1}%'", regiao, palavra);
                sqCommand.CommandText = query2;
                var sizeObject = await sqCommand.ExecuteScalarAsync();
                size = Convert.ToInt32(sizeObject.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO DB" + ex.Message);
            }
            finally
            {
                conexaoDB.Close();
            }

            return new RegistroCompleto(dtResultado, size);
        }

        public async Task<List<string>> ListaEmail(int regiao)
        {
            List<string> dtResultado = new List<string>();
            try
            {
                conexaoDB.Open();

                string query = string.Format("SELECT email FROM registros WHERE regiao = {0}", regiao);

                var sqCommand = new SqliteCommand(query, conexaoDB);
                using (var reader = await sqCommand.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        dtResultado.Add(reader.GetString(0));
                    }
                }

            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERRO DB: {0}", ex.Message);
            }
            finally
            {
                conexaoDB.Close();
            }

            return dtResultado;
        }

        public string getRegiaoName(int regiao)
        {
            try
            {
                conexaoDB.Open();

                string query = string.Format("SELECT nome FROM registros WHERE regiao = {0}", regiao);
                var sqCommand = new SqliteCommand(query, conexaoDB);
                string nome = (string) sqCommand.ExecuteScalar();
                return nome;

            }
            catch (Exception ex)
            {
                Debug.WriteLine("getRegiaoName: {0}", ex.Message);
            }
            finally
            {
                conexaoDB.Close();
            }

            return "Sem Nome";
        }

        #endregion
    }
}
