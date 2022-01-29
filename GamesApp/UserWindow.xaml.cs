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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DBLibrary;

namespace GamesApp
{
    public class MyCommands
    {
        public static RoutedCommand MainMenuCommand { get; set; }

        static MyCommands()
        {
            MainMenuCommand = new RoutedCommand("MainMenuCommand", typeof(UserWindow));
        }
    }


    public partial class UserWindow : Window
    {
        MainWindow? ownerWin = null;
        Users currentUser;
        Lazy<BullsAndCows> BullsCows = new Lazy<BullsAndCows>();
        FootballQuiz quiz;

        public UserWindow()
        {
            InitializeComponent();
        }

        public UserWindow(MainWindow windowStart, Users user)
        {
            InitializeComponent();
            ownerWin = windowStart;
            currentUser = user;
            currentWindow.Title = user.Login;

            ObservableCollection<Games> listGames = new ObservableCollection<Games>();
            listGames.Add(new Games("Быки и коровы", @"Resources\BullsAndCow.jpeg"));
            listGames.Add(new Games("Футбольная викторина", @"Resources\ball.jpg"));
            GamesList.ItemsSource = listGames;
            Games.ID = 1;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ownerWin != null)
            {
                ownerWin.Show();
            }
        }

        private void ExecutedMainMenu(object sender, ExecutedRoutedEventArgs e)
        {
            Game1.Visibility = System.Windows.Visibility.Hidden;
            Game2.Visibility = System.Windows.Visibility.Hidden;
            MainMenu.Visibility = System.Windows.Visibility.Visible;
            GamesList.SelectedItem = null;
        }

        private void CheckNumber_Click(object sender, RoutedEventArgs e)
        {
            BullsCows.Value.CompareNumber(Game1Input.Text, this);
        }

        private void NewGame1_Click(object sender, RoutedEventArgs e)
        {
            CheckNumber.IsEnabled = true;
            Game1Info.Text = "";
            Game1Input.Text = "";
            BullsCows.Value.createNewNumber();
        }

        private void GamesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Games selectedGame = GamesList.SelectedItem as Games;

            if (selectedGame != null)
            {
                if (selectedGame.GameId == 1)
                {
                    Game1Info.Text = ""; Game1Input.Text = "";
                    MainMenu.Visibility = System.Windows.Visibility.Hidden;
                    Game1.Visibility = System.Windows.Visibility.Visible;
                }
                else if (selectedGame.GameId == 2)
                {
                    RightAsks.Text = "0"; AllAsks.Text = "0"; CountAsk.Text = ""; Question.Text = "";
                    ask1.Visibility = System.Windows.Visibility.Hidden;
                    ask2.Visibility = System.Windows.Visibility.Hidden;
                    ask3.Visibility = System.Windows.Visibility.Hidden;
                    ask4.Visibility = System.Windows.Visibility.Hidden;
                    MainMenu.Visibility = System.Windows.Visibility.Hidden;
                    Game2.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutProgram aboutPorg = new AboutProgram();
            aboutPorg.Owner = this;
            aboutPorg.ShowDialog();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ConfigUrlModel q1 = quiz.configUrls[FootballQuiz.index];
                AskStruct askStruct = new AskStruct(ask1.IsChecked, ask2.IsChecked, ask3.IsChecked, ask4.IsChecked,
                    (string)ask1.Content, (string)ask2.Content, (string)ask3.Content, (string)ask4.Content);
                quiz.NextQuestion(askStruct, q1);
                AllAsks.Text = Convert.ToString(quiz.AllAsks);
                RightAsks.Text = Convert.ToString(quiz.RightAsks);
                FootballQuiz.index++;
                CountAsk.Text = $"Вопрос {FootballQuiz.index} из {quiz.configUrls.Count}";

                ConfigUrlModel q = quiz.configUrls[FootballQuiz.index];
                FootballQuiz.FullAsks(this, true, q);
            }
            catch
            {
                FootballQuiz.FullAsks(this,false);
            }
        }

        private void NewGame2_Click(object sender, RoutedEventArgs e)
        {
            quiz = FootballQuiz.NewGame(this);
        }
    }

    public class Games
    {
        public static int ID = 1;
        public string Name { get; set; }
        public string ImageSource { get; set; }

        public int GameId { get; set; }

        public Games(string name, string imageSource)
        {
            Name = name;
            ImageSource = imageSource;
            GameId = ID;
            ID++;
        }
    }
}
