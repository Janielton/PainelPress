using System;
using CefSharp;
using CefSharp.Wpf;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using PainelPress.Classes;
using System.Text;
using CefSharp.Handler;
using System.Diagnostics;

namespace PainelPress
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static string databaseNome = "BaseDados.sqlite";
        public static string BasePasta = Path.Combine(Constants.PASTA, databaseNome);


        protected override void OnStartup(StartupEventArgs e)
        {
            const bool multiThreadedMessageLoop = true;

            try
            {
                IBrowserProcessHandler browserProcessHandler;

                if (multiThreadedMessageLoop)
                {
                    browserProcessHandler = new BrowserProcessHandler();
                }
                //else
                //{
                //    browserProcessHandler = new WpfBrowserProcessHandler(Dispatcher);
                //}

                var settings = new CefSettings();

                settings.MultiThreadedMessageLoop = multiThreadedMessageLoop;
                settings.ExternalMessagePump = !multiThreadedMessageLoop;
                settings.AcceptLanguageList = "pt-BR, pt";
                settings.Locale = "pt-BR";
                Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: browserProcessHandler);
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            base.OnStartup(e);

            //  BaseDados db = new BaseDados();
            // db.AddTabela("topicos", "id INTEGER PRIMARY KEY AUTOINCREMENT, topico INTEGER, nome TEXT);

            if (!File.Exists(BasePasta))
            {
                try
                {
                   if(!Directory.Exists(Constants.PASTA)) Directory.CreateDirectory(Constants.PASTA);
                    BaseDados startDB = new BaseDados();
                    startDB.CriaTabela();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao criar tabela => " + ex.Message);
                }

                return;
            }
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

    }
}
