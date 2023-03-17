using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PainelPress.Model;

namespace PainelPress.Classes
{
    public interface InterfaceEmail
    {
        public readonly static string URL = "https://www.confiraconcursos.com.br";

        #region PUT

        [Put("/newsletter/api")]
        [Headers("Content-Type:  application/json", "Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<RequisicaoBol> AddEmail([Body] string dados);

        #endregion

        #region POST

        [Post("/newsletter/api?acao=send")]
        [Headers("Content-Type:  application/json", "Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<RequisicaoBol> SendEmail([Body] string dados);

        [Post("/newsletter/api")]
        [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<RequisicaoBol> SetStatuEmail([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> dados);

        #endregion

        #region DELETE
        [Delete("/newsletter/api?id_registro={id}&email={email}")]
        [Headers("Content-Type:  application/json", "Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<RequisicaoBol> RemoveEmail(int id, string email);
        #endregion


        #region GET


        [Get("/newsletter/api?acao=show&regiao={rg}")]
        [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<RegistroCompleto> getRegistros(int rg);

        [Get("/newsletter/api?acao=show&regiao={rg}&status=1")]
        [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<RegistroCompleto> getRegistrosAll(int rg);

        [Get("/newsletter/api?acao=show&all=true")]
        [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<RegistroCompleto> getRegistrosAll();

        [Get("/newsletter/api?acao=show&all=true&status=1")]
        [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<RegistroCompleto> getRegistrosAllAtivos();


        [Get("/newsletter/api?acao=show&buscar={s}")]
        [Headers("Content-Type:  application/json", "Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<RegistroCompleto> buscaRegistros(string s);

        [Get("/newsletter/api?acao=show&buscar={s}&regiao={rg}")]
        [Headers("Content-Type:  application/json", "Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<RegistroCompleto> buscaRegistrosbyRegiao(string s, int rg);

        [Get("/newsletter/api?acao=emails&regiao={rg}")]
        [Headers("Content-Type:  application/json", "Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<RegistroCompleto> getEmails(int rg);

        [Get("/newsletter/api?acao=emails&regiao={rg}&status={st}")]
        [Headers("Content-Type:  application/json", "Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<RegistroCompleto> getEmailsByStatus(int st, int rg);

        [Get("/newsletter/api?acao=emails")]
        [Headers("Content-Type:  application/json", "Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<RegistroCompleto> getEmailsAll();

        [Get("/newsletter/api?acao=verifica&regiao={rg}")]
        [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<Quantidade> getQuantidade(int rg);

        [Get("/newsletter/api?acao=show&regiao_email={email}")]
        [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<RegiaoEmail> getRegiaoEmail(string email);

        [Get("/apipainel?ope=14&cat={ct}&data={dt}")]
        [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<string> getPosts(int ct, string dt);

  


        #endregion
    }
    public class Quantidade
    {

        [JsonPropertyName("quantidade")]
        public int valor { get; set; }
    }

    public class RegiaoEmail
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("regiao")]
        public int Regiao { get; set; }
    }



    public class RegistroCompleto
    {
        public RegistroCompleto(List<Registros> registros, int size)
        {
            Registros = registros;
            Size = size;
        }
        [JsonPropertyName("registros")]
        public List<Registros> Registros { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }
    }
    public class Registros
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("regiao")]
        public int Regiao { get; set; }

        [JsonPropertyName("inscricao_data")]
        public string InscricaoData { get; set; }
    }
}
