using FtpLibrary;
using ICSharpCode.AvalonEdit.Rendering;
using Newtonsoft.Json;
using OpenQA.Selenium;
using PainelPress.Classes;
using PainelPress.Elementos;
using PainelPress.Model;
using PainelPress.Model.Conf;
using Prism.Commands;
using Refit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        bool startView = false;
        AlertMensagem alert;

        public ConfigViewModel() {
            Setap();
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void Setap()
        {
            alert = AlertMensagem.instance;
            apiRest = RestService.For<InterfaceAPI>(Constants.SITE, new RefitSettings()
            {
                AuthorizationHeaderValueGetter = () =>
                    Task.FromResult(config.Default.Token)
            });

            //Antes
            IsVisibleJsonConfig = Visibility.Collapsed;
            IsVisibleJsonCampos = Visibility.Collapsed;
            JsonBtText = "Json";
            JsonCamposBtText = "Json";
            IsVisibleLoading = Visibility.Visible;
            IsVisibleEdAdd = Visibility.Collapsed;
            editTextSite = config.Default.site; 
            
            //Pós carregamento
            listCampos = await apiRest.getCampos();
            configPlugin = await apiRest.getConfig();
            ModelConfig = configPlugin;
            JsonCampos = JsonConvert.SerializeObject(listCampos);
            JsonConfig = JsonConvert.SerializeObject(configPlugin);
            IsVisibleLoading = Visibility.Collapsed;
            FilhosCampos = listToCampos(listCampos);
            startView = true;

        }

      

        List<StackPanel> _filhosCampos = new();
        public List<StackPanel> FilhosCampos
        {
            get { return _filhosCampos; }
            set
            {
                _filhosCampos = value;
                OnPropertyChanged("FilhosCampos");
            }
        }


        ConfigPlugin _modelConfig = new();
        public ConfigPlugin ModelConfig
        {
            get { return _modelConfig; }
            set
            {
                _modelConfig = value;
                OnPropertyChanged("ModelConfig");
            }
        }

        #region TEXTO

        string _editTextSite = "";
        public string editTextSite
        {
            get { return _editTextSite; }
            set
            {
                _editTextSite = value;
                OnPropertyChanged("editTextSite");
            }
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

        string _jsonBtText = "";
        public string JsonBtText
        {
            get { return _jsonBtText; }
            set
            {
                _jsonBtText = value;
                OnPropertyChanged("JsonBtText");
            }
        }

        string _jsonCamposBtText = "";
        public string JsonCamposBtText
        {
            get { return _jsonCamposBtText; }
            set
            {
                _jsonCamposBtText = value;
                OnPropertyChanged("JsonCamposBtText");
            }
        }

        #endregion

        #region VISIBLE

        

        Visibility _IsVisibleEdAdd = Visibility.Visible;
        public Visibility IsVisibleEdAdd
        {
            get { return _IsVisibleEdAdd; }
            set
            {
                _IsVisibleEdAdd = value;
                OnPropertyChanged("IsVisibleEdAdd");
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

        Visibility _IsVisibleJsonConfig = Visibility.Visible;
        public Visibility IsVisibleJsonConfig
        {
            get { return _IsVisibleJsonConfig; }
            set
            {
                _IsVisibleJsonConfig = value;
                OnPropertyChanged("IsVisibleJsonConfig");
                if (value == Visibility.Visible)
                {
                    IsVisibleStackConfig = Visibility.Collapsed;
                    JsonBtText = "Formulário";
                }
                else
                {
                    IsVisibleStackConfig = Visibility.Visible;
                    JsonBtText = "Json";
                    JsonToObjectConfig();
                }
            }
        }

        Visibility _IsVisibleStackConfig = Visibility.Visible;
        public Visibility IsVisibleStackConfig
        {
            get {
                return _IsVisibleStackConfig;
            }
            set
            {
                _IsVisibleStackConfig = value;
                OnPropertyChanged("IsVisibleStackConfig");
            }
        }

        Visibility _IsVisibleJsonCampos = Visibility.Visible;
        public Visibility IsVisibleJsonCampos
        {
            get { return _IsVisibleJsonCampos; }
            set
            {
                _IsVisibleJsonCampos = value;
                OnPropertyChanged("IsVisibleJsonCampos");
                if (value == Visibility.Visible)
                {
                    IsVisibleWrapCampos = Visibility.Collapsed;
                    JsonCamposBtText = "Campos";
                }
                else
                {
                    IsVisibleWrapCampos = Visibility.Visible;
                    JsonCamposBtText = "Json";
                    JsonToListCampos();
                }
            }
        }

        Visibility _IsVisibleWrapCampos = Visibility.Visible;
        public Visibility IsVisibleWrapCampos
        {
            get
            {
                return _IsVisibleWrapCampos;
            }
            set
            {
                _IsVisibleWrapCampos = value;
                OnPropertyChanged("IsVisibleWrapCampos");
            }
        }



        #endregion

        #region CLIKS

        public ICommand SalvarConfig { get { return new DelegateCommand(_SalvarConfig); } }

        public ICommand SalvarCampos { get { return new DelegateCommand(_SalvarCampos); } }

        public ICommand ShowJsonConfig { get { return new DelegateCommand(_ShowJsonConfig); } }

        public ICommand AddCampo
        {
            get
            {
                return new CommandParametro((obj) => _AddCampo(obj));
            }
        }
        public ICommand UpdateSite
        {
            get
            {
                return new CommandParametro((obj) => _UpdateSite(obj));
            }
        }
        
        public ICommand ShowJsonCampos { get { return new DelegateCommand(_ShowJsonCampos); } }
        private async void _SalvarConfig()
        {
            try
            {
               IsVisibleLoading = Visibility.Visible;
               var request = await apiRest.setConfig(ModelConfig);
              if(!request.Sucesso)
              {
                    alert.Show("Erro ao salvar configuração");
              }
            }catch(Exception ex)
            {
                alert.Show(ex.Message);
            }
            finally
            {
                IsVisibleLoading = Visibility.Collapsed;
            }
        }
        private async void _SalvarCampos()
        {
            try
            {
                IsVisibleLoading = Visibility.Visible;
                var request = await apiRest.setCampos(listCampos);
                if (!request.Sucesso)
                {
                    alert.Show("Erro ao salvar campos");
                }
            }
            catch (Exception ex)
            {
                alert.Show(ex.Message);
            }
            finally
            {
                IsVisibleLoading = Visibility.Collapsed;
            }
        }

        private void _ShowJsonConfig()
        {
            if(IsVisibleJsonConfig==Visibility.Visible) {
                IsVisibleJsonConfig=Visibility.Collapsed;
            }
            else
            {
                IsVisibleJsonConfig = Visibility.Visible;
            }
        }

        private void _ShowJsonCampos()
        {
            if (IsVisibleJsonCampos == Visibility.Visible)
            {
                IsVisibleJsonCampos = Visibility.Collapsed;
            }
            else
            {
                IsVisibleJsonCampos = Visibility.Visible;
            }
        }

        private void _AddCampo(object param)
        {
            if(IsVisibleEdAdd == Visibility.Collapsed)
            {
                IsVisibleEdAdd = Visibility.Visible;
            }
            else
            {
                IsVisibleEdAdd = Visibility.Collapsed;
                var edit = param as TextBox;
                if (edit == null) return;
                string texto = edit.Text.ToString();
                if (texto != "")
                {
                    string nome = Ferramentas.ToUrlSlug(texto);
                    string descricao = texto + " post";
                    listCampos.Add(new Campos()
                    {
                        nome= nome,
                        dados = new DadosCampos() { 
                            description = descricao,
                            type = "string",
                            single = true,
                            show_in_rest = true
                        }
                    });
                    edit.Text = "";
                    ReloadCampos();
                }
            }

        }

        private void _UpdateSite(object param)
        {
            if (param.ToString().Length > 3)
            {
                config.Default.Upgrade();
                config.Default.site = param.ToString();
                config.Default.Save();
            }
           
        }

        #endregion


        #region METODOS
        private void JsonToObjectConfig()
        {
            if (!startView) return;
            ModelConfig = JsonConvert.DeserializeObject<ConfigPlugin>(JsonConfig);
        }

        private void JsonToListCampos()
        {
            if (!startView) return;
            ReloadCampos();
        }


        private List<StackPanel> listToCampos(List<Campos> list)
        {
            List<StackPanel> lista = new();
            StackPanel stack;
            Button bt;
            TextBlock text;
            int i = 0;
            foreach (var item in list)
            {
                stack = new StackPanel()
                {
                    Tag = i.ToString(),
                    Orientation = Orientation.Horizontal,
                    Height = 35,
                    Margin = new Thickness(10),
                    Background = CorImage.GetCor("#FFE8E8E8")
                };
                text = new TextBlock()
                {
                    Text = item.nome,
                    FontSize = 22,
                    Margin = new Thickness(5, 0, 5, 4)
                };

                bt = new Button()
                {
                    Content = "Remover",
                    Tag = i.ToString(),
                    FontSize = 12,
                    Margin = new Thickness(0, 0, 10, 0),
                    Height = 18,
                    Padding = new Thickness(5, 0, 5, 1),

                };
                bt.Click += ButtonRemove_Click;
                stack.Children.Add(text);
                stack.Children.Add(bt);
                lista.Add(stack);
                i++;
            }

            return lista;
        }

        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            var bt = sender as Button;
            if (bt != null)
            {
                try
                {
                    int index = Convert.ToInt32(bt.Tag);
                    listCampos.RemoveAt(index);
                    ReloadCampos();
                }
                catch  (Exception ex)
                {
                    AlertMensagem.instance.Show(ex.Message);  
                }
              
            }
        }

        private void ReloadCampos()
        {
            FilhosCampos = listToCampos(listCampos);
            JsonCampos = JsonConvert.SerializeObject(listCampos);
        }
        #endregion

    }
}
