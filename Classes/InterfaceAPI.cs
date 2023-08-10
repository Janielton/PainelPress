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
using System.Windows.Media.Animation;

namespace PainelPress.Classes
{

    public interface InterfaceAPI
    {


        [Delete("/wp-json/painel-api/remove-media/{id}")]
        [Headers("Authorization: Bearer")]
        Task<RequisicaoBol> DeleteImagemDestaque(int id);

        #region CAMPOS

        [Delete("/wp-json/painel-api/campo/{id}/{nome}")]
        [Headers("Authorization: Bearer")]
        Task<RequisicaoBol> DeleteCampo(long id, string nome);

        [Post("/wp-json/painel-api/campo/{id}")]
        [Headers("Authorization: Bearer")]
        Task<RequisicaoBol> EditarCampo(long id, [Body(BodySerializationMethod.UrlEncoded)] Dictionary<object, string> data);


        //Campos
        [Get("/wp-json/painel-api/campos")]
        [Headers("Authorization: Bearer")]
        Task<List<Campos>> getCampos();

        [Put("/wp-json/painel-api/campos")]
        [Headers("Authorization: Bearer")]
        Task<RequisicaoBol> setCampos(List<Campos> campos);
        #endregion


        #region CAT

        [Get("/wp-json/painel-api/categorias")]
        [Headers("Authorization: Bearer")]
        Task<List<CategoriaFull>> ListaCats();

        [Post("/wp-json/painel-api/categoria")]
        [Headers("Authorization: Bearer")]
        Task<RequisicaoBol> AdicionarCat([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> dados);

        [Put("/wp-json/painel-api/categoria/{id}")]
        [Headers("Authorization: Bearer")]
        Task<RequisicaoBol> EditarCat(string id, [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> dados);

        [Delete("/wp-json/painel-api/categoria/{id}")]
        [Headers("Authorization: Bearer")]
        Task<RequisicaoBol> DeleteCat(string id);

        #endregion

        #region USUARIO

        [Get("/wp-json/wp/v2/users")]
        [Headers("Authorization: Bearer")]
        Task<List<SimplesModel>> getUsuarios();

        #endregion

        #region POST
        //GET
        [Get("/wp-json/painel-api/posts")]
       // [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        [Headers("Authorization: Bearer")]
        Task<List<PostSimples>> GetPosts();

        [Get("/wp-json/painel-api/busca-posts?key={palavra}")]
        [Headers("Authorization: Bearer")]
        Task<List<PostSimples>> GetPosts(string palavra);

        [Get("/wp-json/painel-api/post/{id}")]
        [Headers("Authorization: Bearer")]
        Task<PostView> getPost(long id);

        //POST
        [Post("/wp-json/wp/v2/posts")]
        [Headers("Content-Type:  application/json", "accept:  application/json", "Authorization: Bearer")]
        Task<string> Postar([Body] string post);

        [Post("/wp-json/wp/v2/posts/{id}")]
        [Headers("Content-Type: application/json", "accept: application/json", "Authorization: Bearer")]
        Task<string> Atualizar([Body] string post, int id);

        //DELETE
        [Delete("/wp-json/wp/v2/posts/{id}")]
        [Headers("Authorization: Bearer")]
        Task<string> Delete(int id);
        #endregion

        #region Taxonomy/Categorias/Tags
        //Taxonomy
        [Get("/wp-json/painel-api/taxonomy/{tipo}")]
        [Headers("Authorization: Bearer")]
        Task<List<Taxonomy>> getTaxonomies(string tipo);

        [Post("/wp-json/painel-api/taxonomy/{tipo}")]
        [Headers("Authorization: Bearer")]
        Task<Taxonomy> getTaxonomy(string tipo, [Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> dados);

        //Categorias
        [Get("/api?rota=categorias")]
        [Headers("Authorization: Basic cGFpbmVscHJlc3M6ODU0NTQ1ODhzJDMzNDQ=")]
        Task<string> getCategorias();

        [Post("/wp-json/wp/v2/tags")]
        [Headers("Content-Type: application/json", "accept: application/json", "Authorization: Bearer")]
        Task<Tag> CriarTag([Body] string post);


        [Get("/wp-json/painel-api/taxonomys")]
        [Headers("Authorization: Bearer")]
        Task<List<string>> getCustomTaxonomies();


        [Post("/wp-json/painel-api/busca-taxonomy")]
        [Headers("Authorization: Bearer")]
        Task<List<Taxonomy>> buscaTaxonomies([Body(BodySerializationMethod.UrlEncoded)] Dictionary<string, object> dados);


        #endregion


        #region CONFIG
 
        //Config
        [Get("/wp-json/painel-api/config")]
        [Headers("Authorization: Bearer")]
        Task<ConfigPlugin> getConfig();

        [Put("/wp-json/painel-api/config")]
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



        #region STORY

        [Post("/wp-json/painel-api/story")]
        [Headers("Authorization: Bearer")]
        Task<RequisicaoBol> AddStory([Body(BodySerializationMethod.UrlEncoded)] Story story);

        [Put("/wp-json/painel-api/story")]
        [Headers("Authorization: Bearer")]
        Task<RequisicaoBol> EditStory([Body(BodySerializationMethod.UrlEncoded)] Story story);

        [Get("/wp-json/painel-api/story-post/{id}")]
        [Headers("Authorization: Bearer")]
        Task<Story?> getStoryByPost(string id);


        [Get("/wp-json/painel-api/stories")]
        [Headers("Authorization: Bearer")]
        Task<List<Story>> getStories();

        //DELETE
        [Delete("/wp-json/painel-api/story/{id}")]
        [Headers("Authorization: Bearer")]
        Task<RequisicaoBol> DeleteStory(string id);

        [Post("/wp-json/painel-api/stories")]
        [Headers("Authorization: Bearer")]
        Task<List<Story>> BuscarStory([Body(BodySerializationMethod.UrlEncoded)] [AliasAs("key")]  string key);

        #endregion


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

