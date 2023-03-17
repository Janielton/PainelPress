using FtpLibrary;
using Newtonsoft.Json;
using PainelPress.Classes;
using PainelPress.Model.Conf;
using Refit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using config = PainelPress.Properties.Settings;

namespace PainelPress.ViewModel
{
    public class ConfigViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        InterfaceAPI apiRest;
        MensagemToast mensagem = new MensagemToast();
        List<Campos> listCampos = new();
        ConfigPlugin configPlugin = new();
        public ConfigViewModel() {
            Setap();
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void Setap()
        {
            apiRest = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(config.Default.Token)
            });
            listCampos = await apiRest.getCampos();
            configPlugin = await apiRest.getConfig();
            JsonCampos = JsonConvert.SerializeObject(listCampos);
            JsonConfig = JsonConvert.SerializeObject(configPlugin);
            IsVisibleLoading = Visibility.Collapsed;
        }
        
        string _jsonConfig = "";
        public string JsonConfig
        {
            get { return _jsonConfig; }
            set
            {
                _jsonConfig = value;
                OnPropertyChanged("JsonConfig");
            }
        }

        string _jsonCampos = "";
        public string JsonCampos
        {
            get { return _jsonCampos; }
            set
            {
                _jsonCampos = value;
                OnPropertyChanged("JsonCampos");
            }
        }

        Visibility _IsVisibleLoading = Visibility.Visible;
        public Visibility IsVisibleLoading
        {
            get { return _IsVisibleLoading; }
            set
            {
                _IsVisibleLoading = value;
                OnPropertyChanged("IsVisibleLoading");
            }
        }
    }
}
