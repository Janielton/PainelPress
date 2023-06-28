using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Exceptions;
using Tweetinvi.Parameters;
using PainelPress.Model;
using config = PainelPress.Properties.Settings;

namespace PainelPress.Classes
{
    public class TwitterService
    {
        private TwitterClient appClient;
        public static string USER = "";
        public bool API = false;

        public TwitterService()
        {
            
        }
        public async Task<string> Setap()
        {
            try
            {
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto };
                if (config.Default.credenciais == "") return "no";
                var uc = JsonConvert.DeserializeObject<Credencial>(config.Default.credenciais, settings);

                appClient = new TwitterClient(uc.ConsumerKey, uc.ConsumerSecret, uc.AccessToken, uc.AccessTokenSecret);
                var user = await appClient.Users.GetAuthenticatedUserAsync();
               // USER = user.Name;
                return user.Name;

            }
            catch (TwitterException ex)
            {
                Debug.WriteLine($"Erro ao autenticar 1: {ex.Message} - {ex.Content}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erro ao autenticar 2: {ex.Message}");
            }
            return "";
        }

        public async Task<bool> PublicarTweetSimples(string mensagem)
        {
            try
            {
                var tweet = await appClient.Tweets.PublishTweetAsync(mensagem);
                Debug.WriteLine("You published the tweet simples: " + tweet);
                return true;
            }
            catch (TwitterException ex)
            {
                Debug.WriteLine($"Erro ao enviar tweet simples: {ex.Message} - {ex.Content}");
            }
            return false;
        }

        public async Task<bool> PublicarTweetImage(string mensagem, byte[] img)
        {
            try
            {
                var uploadedImage = await appClient.Upload.UploadTweetImageAsync(img);
                var tweetImage = await appClient.Tweets.PublishTweetAsync(new PublishTweetParameters(mensagem){
                    Medias = { uploadedImage }
                });
                Debug.WriteLine("You published the tweet com Imagem : " + tweetImage);
                return true;
            }
            catch (TwitterException ex)
            {
                Debug.WriteLine($"Erro ao enviar tweet com Imagem: {ex.Message} - {ex.Content}");
            }
            return false;
        }

        public async Task<string> AuthTwitter()
        {

            var client = new TwitterClient(Constants.CONSUMER_KEY, Constants.CONSUMER_SECRET);
            var authenticationRequest = await client.Auth.RequestAuthenticationUrlAsync();

            Process.Start(new ProcessStartInfo(authenticationRequest.AuthorizationURL)
            {
                UseShellExecute = true
            });

            try
            {
                Login mensagemAlerta = new Login(2);
                if (mensagemAlerta.ShowDialog() == true)
                {
                    var userCredentials = await client.Auth.RequestCredentialsFromVerifierCodeAsync(mensagemAlerta.Resultado, authenticationRequest);
                    config.Default.Upgrade();
                    config.Default.credenciais = JsonConvert.SerializeObject(userCredentials);
                    config.Default.Save();
                    Debug.WriteLine(config.Default.credenciais);

                    // You can now save those credentials or use them as followed
                    var appClient = new TwitterClient(userCredentials);
                    var user = await appClient.Users.GetAuthenticatedUserAsync();
                    return user.Name;
                };
            
            }catch (TwitterException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;

        }

    }
}
