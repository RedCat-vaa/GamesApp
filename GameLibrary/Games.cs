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
                return new ResultBullsAndCows(false, "?? ????? ?? ?????!");
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
                        return new ResultBullsAndCows(false, "? ????? ?? ?????? ???? ????????????? ?????!");
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
                return new ResultBullsAndCows(true, $"??????? {tries} - {num}: ????? - {Bulls}, ????? - {Cows}");
            }
            else
            {
                return new ResultBullsAndCows(false, $"??????? {tries} - {num}: ????? - {Bulls}, ????? - {Cows}");
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

    public struct AskStruct
    {
        public bool? Ask1;
        public bool? Ask2;
        public bool? Ask3;
        public bool? Ask4;
        public string? strAsk1;
        public string? strAsk2;
        public string? strAsk3;
        public string? strAsk4;

        public AskStruct(bool? AskBool1, bool? AskBool2, bool? AskBool3, bool? AskBool4, string? strAsk1, string? strAsk2, string? strAsk3, string? strAsk4)
        {
            Ask1 = AskBool1; Ask2 = AskBool2; Ask3 = AskBool3; Ask4 = AskBool4;
            this.strAsk1 = strAsk1; this.strAsk2 = strAsk2; this.strAsk3 = strAsk3; this.strAsk4 = strAsk4;
        }
    }

    public class FootballQuiz
    {
        public static int index = 0;

        public int AllAsks { get; set; }
        public int RightAsks { get; set; }
        public List<ConfigUrlModel>? configUrls { get; set; }
        public ConfigUrlsModel result { get; set; }
        
        public void NextQuestion(AskStruct askStruct,   ConfigUrlModel q1)
        {
            if (askStruct.Ask1 == true)
            {
                if (askStruct.strAsk1 == q1.Ask)
                {
                    RightAsks++;
                }
            }
            else if (askStruct.Ask2 == true)
            {
                if (askStruct.strAsk2 == q1.Ask)
                {
                    RightAsks++;
                }
            }
            else if (askStruct.Ask3 == true)
            {
                if (askStruct.strAsk3 == q1.Ask)
                {
                    RightAsks++;
                }
            }
            else if (askStruct.Ask4 == true)
            {
                if (askStruct.strAsk4 == q1.Ask)
                {
                    RightAsks++;
                }
            }
            AllAsks++;

        }
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
