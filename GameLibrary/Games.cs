using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace GameLibrary
{
    public struct ResultBullsAndCows
    {
        public bool result;
        public string TextView;

        public ResultBullsAndCows(bool result, string TextView = "")
        {
            this.result = result;
            this.TextView = TextView;
        }

    }

    public class BullsAndCows
    {
        int[] numbers = new int[4];
        public int UserNumber;
        public static int tries = 0;
        public int Bulls { get; private set; }
        public int Cows { get; private set; }
        public BullsAndCows()
        {
            Bulls = 0;
            Cows = 0;
        }

        public void createNewNumber()
        {
            int tempNumber = 0;
            tries = 0;
            Random rnd = new Random();
            for (int i = 0; i < 4; i++)
            {
            markNumber: tempNumber = rnd.Next(0, 9);
                if (i != 0)
                {
                    for (int j = 0; j < i; j++)
                    {
                        if (tempNumber == numbers[j])
                        {
                            goto markNumber;
                        }
                    }
                }
                numbers[i] = tempNumber;
            }
        }

        public ResultBullsAndCows CompareNumber(string num)
        {
            try
            {
                UserNumber = Convert.ToInt32(num);
            }
            catch
            {
                return new ResultBullsAndCows(false, "Вы ввели не число!");
            }
            Bulls = 0;Cows = 0;
            string strNum = num;
            for (int i = 0; i < strNum.Length; i++)
            {
                for (int j = i + 1; j < strNum.Length; j++)
                {
                    if (strNum[i] == strNum[j])
                    {
                        Bulls = 0;
                        Cows = 0;
                        return new ResultBullsAndCows(false, "В числе не должно быть повторяющихся чисел!");
                    }
                }


                for (int ii = 0; ii < strNum.Length; ii++)
                {
                    if (Convert.ToString(numbers[ii]) == Convert.ToString(strNum[i]))
                    {
                        if (ii == i)
                        {
                            Bulls++;
                        }
                        else
                        {
                            Cows++;
                        }
                    }
                }
            }
            tries++;
            if (Bulls == 4)
            {
                return new ResultBullsAndCows(true, $"Попытка {tries} - {num}: Быков - {Bulls}, коров - {Cows}");
            }
            else
            {
                return new ResultBullsAndCows(false, $"Попытка {tries} - {num}: Быков - {Bulls}, коров - {Cows}");
            }

        }

    }

    public class ConfigUrlModel
    {
        [JsonProperty("question")]
        public string Question { get; set; }
        [JsonProperty("ask1")]
        public string Ask1 { get; set; }
        [JsonProperty("ask2")]
        public string Ask2 { get; set; }
        [JsonProperty("ask3")]
        public string Ask3 { get; set; }
        [JsonProperty("ask4")]
        public string Ask4 { get; set; }
        [JsonProperty("ask")]
        public string Ask { get; set; }
    }

    public class ConfigUrlsModel
    {
        [JsonProperty]
        public List<ConfigUrlModel> ConfigUrls { get; set; }
    }

    public class FootballQuiz
    {
        public static int index = 0;

        public int AllAsks { get; set; }
        public int RightAsks { get; set; }
        public List<ConfigUrlModel>? configUrls { get; set; }
        public ConfigUrlsModel result { get; set; }
        
        public FootballQuiz()
        {
            index = 0;
            AllAsks = 0;
            RightAsks = 0;
            string json = "";
            //using (FileStream fs = new FileStream(@"C:\Users\Alexey\source\repos\GamesApp\GameLibrary\Football.json", FileMode.OpenOrCreate))
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Football.json");
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                byte[] array = new byte[fs.Length];
                fs.Read(array, 0, array.Length);
                json = System.Text.Encoding.Default.GetString(array);
            }
            configUrls = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ConfigUrlModel>>(json);
            result = new ConfigUrlsModel { ConfigUrls = configUrls };
        }
    }
}
