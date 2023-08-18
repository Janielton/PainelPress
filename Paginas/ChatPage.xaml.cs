using ChatGPT.Net.DTO.ChatGPT;
using FontAwesome.WPF;
using IA;
using Newtonsoft.Json;
using PainelPress.Classes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Globalization;
using System.Windows.Interop;
using static SkiaSharp.HarfBuzz.SKShaper;
using PainelPress.Elementos;
using XamlRadialProgressBar;

namespace PainelPress.Paginas
{
    /// <summary>
    /// Interação lógica para ChatPage.xam
    /// </summary>
    public partial class ChatPage : ContentControl
    {
        MainIA mainIA = new MainIA();
        CtlDadosChat ctlDadosChat = new CtlDadosChat();
        int idChat = 0;
        List<Chat> listaChat = new List<Chat>();
        List<ChatItem> listaConversa = new List<ChatItem>();
        Button btAtual = null;
        AlertMensagem alertMensagem = new AlertMensagem();

        public ChatPage()
        {
            InitializeComponent();
            loadChats();
        }

        private async void loadChats(string s = null, int id = 0)
        {
            if (s == null)
            {
                listaChat = await ctlDadosChat.getChats();
            }
            else
            {
                listaChat = await ctlDadosChat.getChats(s);
            }

            stackConversas.Children.Clear();
            if (listaChat.Count>0)
            { 
                int i = 0;
                foreach(var chat in listaChat)
                {
                    Button bt = new Button() {
                        Content = chat.titulo,
                        TabIndex = i,
                        Tag = chat.id,
                        Style = Resources["ButtonChats"] as Style
                    };
                    if (id > 0 && id == chat.id)
                    {
                        btAtual = bt;
                        bt.IsEnabled = false;
                    }
                    bt.Click += (s, e) =>
                    {
                        Button bt = s as Button;
                        if(bt!=null )
                        {
                            OpenChat(listaChat[bt.TabIndex]);
                            if (btAtual != null)
                            {
                                btAtual.IsEnabled = true;
                            }
                            btAtual = bt;
                            bt.IsEnabled = false;
                        }
                    };
                    stackConversas.Children.Add(bt);
                    i++;
                }
            }
            else
            {
                Button bt = new Button()
                {
                    Content = "Nenhuma conversa encontrada",
                    IsEnabled = false,
                    Style = Resources["ButtonChats"] as Style
                };
                stackConversas.Children.Add(bt);
            }
            loadingConversa.Visibility = Visibility.Collapsed;
        }

        private async void OpenChat(Chat ct)
        {
            tbTitulo.Text = ct.titulo;
            stackTitulo.Visibility = Visibility.Visible;
            idChat = ct.id;
            stackChat.Children.Clear();
            listaConversa = await ctlDadosChat.getItensChats(ct.id);
            gridPrompt.Visibility = Visibility.Visible;
            if (listaConversa.Count > 0) setConversa();

        }

        private void setConversa()
        {
            int i = 0;
            foreach (var item in listaConversa)
            {
             
                if (item.isME)
                {
                    stackChat.Children.Add(messagemMe(item.conteudo, i));
                }
                else
                {
                    stackChat.Children.Add(messagemIa(item.conteudo,i));
                }
                i++;
            }
        }

        private async void Request(string msg)
        {
            SetProgresso();
            stackChat.Children.Add(messagemMe(msg, listaConversa.Count));
            double sizeScroll = scrollChat.ScrollableHeight +100;
            string result = await mainIA.RequestGPT(msg);
          //  string result = await getConteudo();
            SetProgresso(false);
            if (string.IsNullOrEmpty(result))
            {
                alertMensagem.Show("Ocorreu um erro ao mandar comando ao chatGPT", "Erro na requisição");
                return;
            }
            stackChat.Children.Add(messagemIa(result, listaConversa.Count+1));
            scrollChat.ScrollToVerticalOffset(sizeScroll);
            saveRequest(msg, result);
        }

        private async void saveRequest(string pergunta, string resposta)
        {
            if (idChat > 0)
            {
                await ctlDadosChat.AdicionarChatItem(new ChatItem()
                {
                    conteudo= pergunta,
                    id_chat=idChat,
                    data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    tipo = 1
                });


                await ctlDadosChat.AdicionarChatItem(new ChatItem()
                {
                    conteudo = resposta,
                    id_chat = idChat,
                    data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    tipo = 2
                });
            }
            else
            {
              
                idChat = await ctlDadosChat.AdicionarChat(new Chat()
                {
                    titulo = pergunta,
                    data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    tipo = 1
                });

                await ctlDadosChat.AdicionarChatItem(new ChatItem()
                {
                    conteudo = pergunta,
                    id_chat = idChat,
                    data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    tipo = 1
                });

                await ctlDadosChat.AdicionarChatItem(new ChatItem()
                {
                    conteudo = resposta,
                    id_chat = idChat,
                    data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    tipo = 2
                });
                loadChats(null, idChat);
                tbTitulo.Text = editChatTitulo.Texto;
                stackTitulo.Visibility = Visibility.Visible;

            }

        }
        private async Task<string> getConteudo()
        {
            try
            {
                await Task.Delay(3000);
                string json = File.ReadAllText(Constants.PASTA + "result.json");
                ChatGptResponse obj = JsonConvert.DeserializeObject<ChatGptResponse>(json);
                return obj.Choices[0].Message.Content;
            }catch(Exception ex) { Debug.WriteLine(ex.Message); }
            return "NO";
        }

        private Border messagemMe(string msg, int index)
        {
            Border border = new Border() { 
                CornerRadius = new CornerRadius(4),
                Background = CorImage.GetCor("#FF3A3A3A"),
                Margin = new Thickness(8, 4, 15, 4),
                Padding = new Thickness(5),
            };

            var stack = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(2, 3, 8, 6),
                HorizontalAlignment = HorizontalAlignment.Left

            };
            stack.Children.Add(new TextBox()
            {
                Width = stackChat.ActualWidth - 50,
                Text = msg,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 16,
                Padding = new Thickness(5),
                Foreground = Brushes.White,
                IsReadOnly = true,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0)
            });
            border.Child = stack;

            return border;
        }

        private Border messagemIa(string msg, int index)
        {
            Border border = new Border()
            {
                CornerRadius = new CornerRadius(8),
                Background = Brushes.White,
                Margin = new Thickness(18, 4, 8, 4)
            };

            Grid grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40),
            });
           
            StackPanel stack = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(2, 3, 8, 6),

            };
            Grid.SetRow(stack, 0);
        
            stack.Children.Add(new TextBox()
            {
                Width = stackChat.ActualWidth - 50,
                Text = msg,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 16,
                Padding = new Thickness(5),
                Foreground = Brushes.Black,
                IsReadOnly = true,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0)
            });
            grid.Children.Add(stack);
        

            StackPanel stackBase = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
                Height = 40
            };

            Grid.SetRow(stackBase, 1);
            Button btCopy = new Button()
            {
                Padding = new Thickness(3),
                Background = Brushes.Transparent,
                Cursor = Cursors.Hand,
                Content = new ImageAwesome() { Icon = FontAwesomeIcon.Copy, Foreground = CorImage.GetCorIcone() },
                TabIndex = index,
                Tag = "copy",
                Width = 30,
                Height = 30,
                Margin = new Thickness(3, 0, 3, 0),
            };
            Button btPostar = new Button()
            {
                Padding = new Thickness(3),
                Background = Brushes.Transparent,
                Cursor = Cursors.Hand,
                Content = new ImageAwesome() { Icon = FontAwesomeIcon.Wordpress, Foreground = CorImage.GetCorIcone() },
                TabIndex = index,
                Tag = "postar",
                Width = 30,
                Height = 30,
                Margin = new Thickness(3, 0, 3, 0),
            };
            btCopy.Click += btBase_Click;
            btPostar.Click += btBase_Click;
            stackBase.Children.Add(btCopy);
            stackBase.Children.Add(btPostar);
            grid.Children.Add(stackBase);  
            border.Child = grid;
            return border;
        }


        private void btBase_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            if(bt != null)
            {
                try
                {
                    string tag = bt.Tag.ToString();
                    string conteudo = listaConversa[bt.TabIndex].conteudo;
                    if (tag == "copy")
                    {
                        Clipboard.SetText(conteudo);
                    }
                    else if (tag == "postar")
                    {
                        new WinContainer(new Postar(conteudo)).Show();
                    }
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                
            }
        }
        private void editPergunta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && editPergunta.Text != "")
            {
                Request(editPergunta.Text);
                editPergunta.Text = "";
            }
        }
        private void botoes_Click(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            if (bt != null)
            {
                string tag = bt.Tag.ToString();
                if (tag == "send")
                {
                    if (editPergunta.Text != "")
                    {
                        Request(editPergunta.Text);
                        editPergunta.Text = "";
                    }
                }
                else if(tag == "config")
                {
                    if (bdKey.IsVisible)
                    {
                        bdKey.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        bdKey.Visibility = Visibility.Visible;
                    }
                }
                else if (tag == "novo")
                {
                    bdAddChat.Visibility = Visibility.Visible;
                    reset();
                }
                else if (tag == "save-chat")
                {
                    saveChat();

                }
                else if (tag == "delete-chat")
                {
                    ctlDadosChat.DeleteChat(idChat);
                    reset();
                    loadChats();
                }
                else if (tag == "close-chats")
                {
                    bt.Tag = "open-chats";
                    bt.Content = new ImageAwesome() { Icon = FontAwesomeIcon.Indent, Width = 23, Foreground = Brushes.WhiteSmoke };
                    gridSidebar.Width = new GridLength(0);
                }
                else if (tag == "open-chats")
                {
                    bt.Tag = "close-chats";
                    bt.Content = new ImageAwesome() { Icon = FontAwesomeIcon.Outdent, Width = 23, Foreground = Brushes.WhiteSmoke };
                    gridSidebar.Width = new GridLength(200);
                }
                else if (tag == "teste-api")
                {
                   
                }
                else if (tag == "save-api")
                {

                }

            }
        }

        private void reset()
        {
            idChat = 0;
            gridPrompt.Visibility = Visibility.Collapsed;
            stackChat.Children.Clear();
            tbTitulo.Text = "";
            stackTitulo.Visibility = Visibility.Collapsed;
        }
        private void editChat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) saveChat();
        }

        private async void saveChat()
        {
            if (editChatTitulo.Texto != "")
            {
                idChat = await ctlDadosChat.AdicionarChat(new Chat()
                {
                    titulo = editChatTitulo.Texto,
                    data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    tipo = 1
                });
                if (idChat == 0)
                {
                    AlertMensagem.instance.Show("Ocorreu um erro ao criar um novo chat", "Erro ao criar chat");
                    return;
                }
                loadChats(null, idChat);
                tbTitulo.Text = editChatTitulo.Texto;
                stackTitulo.Visibility = Visibility.Visible;
            }

            bdAddChat.Visibility = Visibility.Collapsed;
            gridPrompt.Visibility = Visibility.Visible;
         
        }

        private void edit_TextChanged(object sender, TextChangedEventArgs e)
        {

            if (editBusca.Texto.Length > 1)
            {
                loadChats(editBusca.Texto);
            }
            else
            {
                loadChats();
            }
        }

        private void SetProgresso(bool show = true)
        {
            if (show)
            {
                if (bdProgresso.Child == null)
                {
                    bdProgresso.Child = new RadialProgressBar()
                    {
                        Foreground = CorImage.GetCorVerde(),
                        IsIndeterminate = true,
                        IndeterminateSpeedRatio = 0.2
                    };
                }
                bdProgresso.Visibility = Visibility.Visible;
            }
            else
            {
                bdProgresso.Visibility = Visibility.Collapsed;
            }
        }
    }
}
