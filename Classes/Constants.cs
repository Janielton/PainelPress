using System;
using System.Collections.Generic;
using System.Text;
using config = PainelPress.Properties.Settings;

namespace PainelPress.Classes
{
    public class Constants
    {
        public readonly static string SITE = config.Default.site;
        public readonly static string PASTA = @"C:\PanelPress\";
        public readonly static string PATH_IMAGEM = "/conteudo";

        public static readonly string CONSUMER_KEY = "iFSYf2gS3OuYl5vrEFvru5mEN";
        public static readonly string CONSUMER_SECRET = "I6Yo3mMMh41LweiJQltxoAfkAMAiHxImhL7q8JxyotgUYjRiiC";
        public static readonly string ACCESS_TOKEN = "137524448-0CMbZtoaNMXG5TdhikueFLplr3gpMh0EmtEliD2s";
        public static readonly string ACCESS_TOKEN_SECRET = "hEooT16DZOu1F0si01Sz3rNYbeXDCBioiRgkKuKRbRWDp";
        public static readonly string BAREAR_TOKEN = "AAAAAAAAAAAAAAAAAAAAAGRCRAEAAAAA154BlmmDS8GrONTwei2mGkEaTC4%3D5ZCMD2htgsVYqGNCGA6Ev4lxQY3q4mpXB3HXPOk3xkSAFZbi6h";

        public readonly static string[] taxonomy = { "post_tag", "vagas", "concurso" };

        public readonly static string urlSendFacebook = "https://graph.facebook.com/v15.0/{idpagina}/feed?message={mensagem}&access_token={token}&link={link}";
    }
}
