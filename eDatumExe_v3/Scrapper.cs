using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace eDatumExe_v3
{
    public class Scrapper
    {
        protected IWebDriver _driver;

        public Scrapper()
        {
            var options = new ChromeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--ignore-ssl-errors");

            _driver = new ChromeDriver(options);

            //_driver.ExecuteScript("scroll(0, 300);");
            //Thread.Sleep(250);
        }

        public static void GetDataWithin3MinutesInterval()
        {
            string jsonBetCityRu = null;
            string json1xBet = null;

            var betCityRuScrapper = new BetCityRuScrapper("https://betcityru.com/en/results/soccer");
            var oneXBet = new OneXBet("https://1xbet.com/en/results/");

            do
            {
                jsonBetCityRu = Task.Run(() =>
                {
                    return betCityRuScrapper.GetBetCityRuDataAsync();
                }).Result;


                json1xBet = Task.Run(() =>
                {
                    return oneXBet.GetOneXBetData();
                }).Result;

                AppendDataToFile($"{DateTime.Now}{jsonBetCityRu}", $"{DateTime.Now}{json1xBet}");

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Data appended!!!");
                Console.ResetColor();

                Thread.Sleep(10000);

            } while (true);

        }

        private static void AppendDataToFile(string json1, string json2)
        {
            Directory.CreateDirectory($"{Environment.CurrentDirectory}\\Soccer Data");
            try
            {
                File.AppendAllText($"{Environment.CurrentDirectory}\\Soccer Data\\BetCityRu.json", json1);
                File.AppendAllText($"{Environment.CurrentDirectory}\\Soccer Data\\1xBet.json", json2);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        protected bool WaitToLoad(By by)
        {
            int i = 0;
            while (i < 1200)
            {
                i++;
                Thread.Sleep(150);
                try
                {
                    _driver.FindElement(by);
                    break;
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine("Waiting to load the page.");
                }
            }
            return i == 1200 ? false : true;
        }
        protected string StringBeauty(string s) => s?.Replace("\r", "").Replace("\n", "").Trim();
    }
}
