using CefSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PainelPress.Classes
{
    public class CustomMenuWeb : IContextMenuHandler
    {
        public void OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
        {
            // Remove any existent option using the Clear method of the model
            //
            //model.Clear();

            // You can add a separator in case that there are more items on the list
            if (model.Count > 0)
            {
                model.AddSeparator();
            }
            if (!string.IsNullOrEmpty(parameters.SelectionText))
            {
                model.AddItem(CefMenuCommand.Find, "Buscar '" + parameters.SelectionText + "'");
            }

            if (!string.IsNullOrEmpty(parameters.LinkUrl))
            {
                model.AddItem((CefMenuCommand)26504, "Abrir no navegador");
                model.AddItem((CefMenuCommand)26505, "Copiar Link");
            }
   
            model.AddSeparator();
            // Add a new item to the list using the AddItem method of the model
            model.AddItem((CefMenuCommand)26501, "Show DevTools");
            model.AddItem((CefMenuCommand)26502, "Close DevTools");

            // Add a separator
            model.AddSeparator();

            // Add another example item
            model.AddItem((CefMenuCommand)26503, "Cancelar");
        }

        public bool OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
        {
            // React to the first ID (show dev tools method)
            if (commandId == (CefMenuCommand)26504)
            {
                try
                {
                    if (!string.IsNullOrEmpty(parameters.LinkUrl)) {
                        var ps = new ProcessStartInfo(parameters.LinkUrl)
                        {
                            UseShellExecute = true

                        };
                        Process.Start(ps);
                    }
                    
                }
                catch (Exception other)
                {
                    MessageBox.Show(other.Message);
                }
                return true;
            }
            if (commandId == (CefMenuCommand)26501)
            {
                browser.GetHost().ShowDevTools();
                return true;
            }
            // React to the second ID (show dev tools method)
            if (commandId == (CefMenuCommand)26502)
            {
                browser.GetHost().CloseDevTools();
                return true;
            }

            // React to the third ID (Display alert message)
            if (commandId == (CefMenuCommand)26503)
            {
               
                return true;
            }

            if (commandId == CefMenuCommand.Find)
            {
                           
                if (!string.IsNullOrEmpty(parameters.SelectionText))
                {
                    string searchAddress = "https://www.google.com/search?q=" + parameters.SelectionText;
                    var ps = new ProcessStartInfo(searchAddress)
                    {
                        UseShellExecute = true

                    };
                    Process.Start(ps);
                }

                return true;
            }
            if (commandId == (CefMenuCommand)26505)
            {

                Clipboard.SetText(parameters.LinkUrl);
                return true;
            }
            // Any new item should be handled through a new if statement


            // Return false should ignore the selected option of the user !
            return false;
        }

        public void OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {

        }

        public bool RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
        {
            return false;
        }
    }
}
