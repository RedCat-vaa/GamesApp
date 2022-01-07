using System;
using System.Collections.Generic;
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
using DBLibrary;
using ClientServer;

namespace GamesApp
{
    public enum typeApp
    {
        Client,
        ClientServer,
        Server
    }
    public partial class MainWindow : Window
    {

        ServerApp server;
        public MainWindow()
        {
            InitializeComponent();
            Client.IsChecked = true;
        }

        #region Authentication
        private void OpenUser_Click(object sender, RoutedEventArgs e)
        {
            Users? FindUser = null;
            typeApp type = typeApp.Client;
            if (ClientServer.IsChecked==true)
            {
                type = typeApp.ClientServer;
            }
            else if (Server.IsChecked == true)
            {
                type = typeApp.Server;
            }

            if (type == typeApp.ClientServer)
            {
                (string ip, string port) =  ClientApp.OutputIP();
                ClientApp client = new ClientApp(ip, port);
                string result = client.ConnectionServer(LoginTxt.Text, PasswordTxt.Password, AuthOrReg.Auth);
                //string result = client.AnswerFromServer(ip, port,6);

                if (result== "Success")
                {
                    UserWindow userWin = new UserWindow(this, new Users { Login = LoginTxt.Text, Password = PasswordTxt.Password });
                    this.Hide();
                    userWin.ShowDialog();
                }
                else
                {
                    MessageBox.Show(result);
                }

                return;
            }

            //Client
            using (DB db = new DB())
            {
                var users = db.Users.Where(user => user.Login == LoginTxt.Text);
                if (users.Count()==0)
                {
                    MessageBox.Show("Пользователь не найден!");
                    return;
                }
                else
                {
                    foreach(Users user in users)
                    {
                        if(user.Password==PasswordTxt.Password)
                        {
                            FindUser = user;
                        }
                    }
                }
            }

            if(FindUser!=null)
            {
                UserWindow userWin = new UserWindow(this, FindUser);
                this.Hide();
                userWin.ShowDialog();
            }
            else
            {
                MessageBox.Show("Неверный пароль!");
            }
               
        }
        #endregion

        private void RegUser_Click(object sender, RoutedEventArgs e)
        {
            RegWindow rgWin = new RegWindow();
            rgWin.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (server!=null)
            {
                server.stopServer();
            }
           
            this.Close();
        }

        private void HandlerEventServer(object sender, ServerEventArgs ev)
        {
            ServerLog.Dispatcher.Invoke(new Action(()=>ServerLog.Text += ev.Message + Environment.NewLine));
        }

        private void Server_Checked(object sender, RoutedEventArgs e)
        {
            if (Server.IsChecked==true)
            {
                GroupServer.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            server = new ServerApp();
            server.eventSendMessage += HandlerEventServer;
            server.startServerAsync();
        }
    }
}
