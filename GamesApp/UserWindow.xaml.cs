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
using GameLibrary;
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

            ResultBullsAndCows newResult = BullsCows.Value.CompareNumber(Game1Input.Text);
            if (newResult.result)
            {
                Game1Info.Text = Game1Info.Text + newResult.TextView + Environment.NewLine;
                Game1Info.Text = Game1Info.Text + "Вы выиграли!" + Environment.NewLine;
                CheckNumber.IsEnabled = false;
            }
            else
            {
                Game1Info.Text = Game1Info.Text + newResult.TextView + Environment.NewLine;
            }

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
                else
                {

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
                if (ask1.IsChecked == true)
                {
                    if ((string)ask1.Content == q1.Ask)
                    {
                        quiz.RightAsks++;
                    }
                }
                else if (ask2.IsChecked == true)
                {
                    if ((string)ask2.Content == q1.Ask)
                    {
                        quiz.RightAsks++;
                    }
                }
                else if (ask3.IsChecked == true)
                {
                    if ((string)ask3.Content == q1.Ask)
                    {
                        quiz.RightAsks++;
                    }
                }
                else if (ask4.IsChecked == true)
                {
                    if ((string)ask4.Content == q1.Ask)
                    {
                        quiz.RightAsks++;
                    }
                }
                quiz.AllAsks++;
                AllAsks.Text = Convert.ToString(quiz.AllAsks);
                RightAsks.Text = Convert.ToString(quiz.RightAsks);
                FootballQuiz.index++;
                CountAsk.Text = $"Вопрос {FootballQuiz.index} из {quiz.configUrls.Count}";

                ConfigUrlModel q = quiz.configUrls[FootballQuiz.index];
                FullAsks(true, q);
            }
            catch
            {
                FullAsks(false);
            }

        }

        private void FullAsks(bool CorrectTry, ConfigUrlModel q = null)
        {
            if (CorrectTry)
            {
                Question.Text = q.Question;
                ask1.Content = q.Ask1;
                ask2.Content = q.Ask2;
                ask3.Content = q.Ask3;
                ask4.Content = q.Ask4;
            }
            else
            {
                ask1.Content = "";
                ask2.Content = "";
                ask3.Content = "";
                ask4.Content = "";
                ask1.Visibility = System.Windows.Visibility.Hidden;
                ask2.Visibility = System.Windows.Visibility.Hidden;
                ask3.Visibility = System.Windows.Visibility.Hidden;
                ask4.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        private void NewGame2_Click(object sender, RoutedEventArgs e)
        {
            quiz = new FootballQuiz();
            try
            {
                ConfigUrlModel q = quiz.configUrls[FootballQuiz.index];
                RightAsks.Text = "0"; AllAsks.Text = "0"; CountAsk.Text = ""; Question.Text = "";
                CountAsk.Text = $"Вопрос {FootballQuiz.index} из {quiz.configUrls.Count}";
                FullAsks(true, q);
                ask1.Visibility = System.Windows.Visibility.Visible;
                ask2.Visibility = System.Windows.Visibility.Visible;
                ask3.Visibility = System.Windows.Visibility.Visible;
                ask4.Visibility = System.Windows.Visibility.Visible;
            }
            catch
            {
                FullAsks(false);
            }
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
