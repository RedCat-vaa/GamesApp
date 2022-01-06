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
using System.Windows.Shapes;
using DBLibrary;
using ClientServer;

namespace GamesApp
{
    /// <summary>
    /// Логика взаимодействия для RegWindow.xaml
    /// </summary>
    public partial class RegWindow : Window
    {
        public RegWindow()
        {
            InitializeComponent();
            Client.IsChecked = true;
        }


        private void RegUser_Click(object sender, RoutedEventArgs e)
        {

            if (RegValidation() == false)
            {
                return;
            }

            if (ClientServer.IsChecked == true)
            {
                (string ip, string port) = ClientApp.OutputIP();
                ClientApp client = new ClientApp(ip, port);
                string result = client.ConnectionServer(LoginTxt.Text, PasswordTxt.Password, AuthOrReg.Reg);

                if (result == "Success")
                {
                    MessageBox.Show("Регистрация прошла успешно");
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
                if (users.Count()>0)
                {
                    MessageBox.Show("Логин уже занят!");
                    return;
                }
                Users newUser = new Users { Login = LoginTxt.Text, Password = PasswordTxt.Password};
                db.Users.Add(newUser);
                db.SaveChanges();
                MessageBox.Show("Регистрация прошла успешно");
            }
               
        }

        private bool RegValidation()
        {
            if (LoginTxt.Text.Length < 4)
            {
                MessageBox.Show("Логин должен иметь больше 3 символов!");
                return false;
            }
            else if (PasswordTxt.Password.Length < 6)
            {
                MessageBox.Show("Пароль должен иметь больше 5 символов!");
                return false;
            }
            else if (PasswordTxt.Password != PasswordTxtRepeat.Password)
            {
                MessageBox.Show("Пароли не совпадают!");
                return false;
            }

            return true;
        }

    }
}
