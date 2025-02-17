﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using config = PainelPress.Properties.Settings;

namespace PainelPress.Classes
{
    public class Constants
    {
        public static string SITE = config.Default.site;

        public static string SITEMAP = config.Default.sitemapurl;
        public static string SITEMAPNEWS = config.Default.sitemapnews;
        public readonly static string TOKEN = config.Default.Token;
        public readonly static string PASTA = @"C:\PanelPress\";
        public readonly static string PASTA_TEMP = Path.Combine(PASTA, "arquivos_temporarios");
        public readonly static string PATH_IMAGEM = "/conteudo";
        public static string PATHDB = Path.Combine(Constants.PASTA, "BaseDados.sqlite");

        public static readonly string CONSUMER_KEY = "";
        public static readonly string CONSUMER_SECRET = "";
        public static readonly string ACCESS_TOKEN = "";
        public static readonly string ACCESS_TOKEN_SECRET = "";
        public static readonly string BAREAR_TOKEN = "";

        public readonly static string[] taxonomy = { "post_tag", "vagas", "concurso" };

        public readonly static string urlSendFacebook = "https://graph.facebook.com/v15.0/{idpagina}/feed?message={mensagem}&access_token={token}&link={link}";
    }
}
