using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using PainelPress.Model;
using PainelPress.Model.Conf;

namespace PainelPress.Classes
{

    public interface InterfaceAPI
    {
      

        #region POST
        //Posts
        [Get("/wp-json/wp/v2/posts")]
       // [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        [Headers("Authorization: Bearer")]
        Task<List<PostSimples>> GetPosts();

        [Get("/wp-json/wp/v2/search?search={palavra}&per_page=15&subtype=post")]
        [Headers("Authorization: Bearer")]
        Task<List<PostSimples>> GetPosts(string palavra);

        [Get("/api?rota=post&id={id}")]
        [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<PostView> getPost(long id);

        [Post("/wp-json/wp/v2/posts")]
        [Headers("Content-Type:  application/json", "accept:  application/json", "Authorization: Bearer")]
        Task<Post> Postar([Body] string post);

        [Post("/wp-json/wp/v2/posts/{id}")]
        [Headers("Content-Type: application/json", "accept: application/json", "Authorization: Bearer")]
        Task<PostSimples> Atualizar([Body] string post, int id);
        #endregion

        #region Taxonomy/Categorias/Tags
        //Taxonomy
        [Get("/api?rota=taxonomy&param={isparam}")]
        [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<List<Taxonomy>> getTaxonomy(string isparam);

        //Categorias
        [Get("/api?rota=categorias")]
        [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<string> getCategorias();

        [Post("/wp-json/wp/v2/tags")]
        [Headers("Content-Type: application/json", "accept: application/json", "Authorization: Bearer")]
        Task<Tag> CriarTag([Body] string post);
        #endregion


        #region CONFIG
        //Campos
        [Get("/wp-json/painel-api/campos")]
        [Headers("Authorization: Bearer")]
        Task<List<Campos>> getCampos();

        [Post("/wp-json/painel-api/campos")]
        [Headers("Authorization: Bearer")]
        Task<RequisicaoBol> setCampos(List<Campos> campos);

        //Config
        [Get("/wp-json/painel-api/config")]
        [Headers("Authorization: Bearer")]
        Task<ConfigPlugin> getConfig();

        [Post("/wp-json/painel-api/config")]
        [Headers("Authorization: Bearer")]
        Task<RequisicaoBol> setConfig(ConfigPlugin config);

        #endregion

        //CLEAR
        [Get("/api?rota=cacher")]
        [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<RequisicaoBol> ClearChache([Body(BodySerializationMethod.UrlEncoded)] Cacher config);

       
        //Topicos
        [Get("/app/?rota=inscritos&filtro=varios&ids={ids}")]
        [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<List<Inscritos>> GetInscritos(string ids);

        [Get("/app/?rota=inscritos&filtro=all")]
        [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<List<Inscritos>> GetInscritosALL();

        #region TOKEN/LOGIN

        [Post("/wp-json/painel-press/v1/token/validate")]
        [Headers("Authorization: Bearer")]
        Task<RequisisaoToken> VerificaToken();

        [Post("/wp-json/painel-press/v1/token")]
        Task<Token> Login([Body(BodySerializationMethod.UrlEncoded)] User user);

        #endregion

        [Post("/edital/admin/admin-ajax.php")]
        Task<RequisicaoUrl> GerarEdital([Body(BodySerializationMethod.UrlEncoded)] UrlEdital dados);

        // Notificações
        [Post("/send")]
        [Headers("Content-Type: application/json", "accept: application/json", "Authorization: key=AAAA7T4j51w:APA91bGvjx3STYfBnEYIY7vZFE1-bEAIBv4PoqXEKGRbReOVIpEoC8MO0a_pAR0z9Y2YKR5bnfJps67MuRuR0Xx2tzKvy-3Rg3hKntFlpTUgDTuaBfIJ7-dS9Fvkx1dHl1w7WSAc1XYs")]
        Task<string> SendNotificacao([Body] string dados);

      

        #region DELETE
        [Delete("/wp-json/wp/v2/posts/{id}")]
        [Headers("Authorization: Bearer")]
        Task<PostSimples> Delete(int id);
        #endregion

        #region UPLOAD no-use
        [Multipart]
        [Put("/imagens/api/index.php?nome={param}")]
        [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<RequisicaoBol> UploadBrasao(string param, [AliasAs("binary")] ByteArrayPart bytes);

        #endregion

    }

    public class RequisisaoToken
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("data")]
        public DataToken Data { get; set; }
    }

    public class DataToken
    {
        [JsonProperty("status")]
        public int Status { get; set; }
    }

    public class User
    { 
        [AliasAs("username")]
        public string username { get; set; }

        [AliasAs("password")]
        public string password { get; set; }
    }

    public class Cacher
    {
        [AliasAs("tipo")]
        public string tipo { get; set; }

        [AliasAs("slug")]
        public string slug { get; set; }
    }

    public class UrlEdital
    {
        [AliasAs("action")]
        public string action { get; set; }
        [AliasAs("url")]
        public string url { get; set; }
        [AliasAs("keyword")]
        public string keyword { get; set; }
        [AliasAs("nonce")]
        public string nonce { get; set; }
    }

    public class RequisicaoUrl
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("statusCode")]
        public int StatusCode { get; set; }

        [JsonProperty("shorturl")]
        public string Shorturl { get; set; }
    }

    public class RequisicaoBol
    {
        [JsonProperty("sucesso")]
        public bool Sucesso { get; set; }

        [JsonProperty("mensagem")]
        public string Mensagem { get; set; }

    }

    public class RequisicaoMeta
    {
        [JsonProperty("result")]
        public int result { get; set; }

        [JsonProperty("reason")]
        public string reason { get; set; }

    }

    public class RequisicaoServerCache
    {
        [JsonProperty("metadata")]
        public RequisicaoMeta metadata { get; set; }

    }

    public class VerificaImgCidade
    {
        [JsonProperty("sucesso")]
        public bool Sucesso { get; set; }

        [JsonProperty("cidades")]
        public string Cidades { get; set; }

    }
}

