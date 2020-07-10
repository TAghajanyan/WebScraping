using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;

namespace eDatumExe_v3
{
    public class BetCityRuScrapper : Scrapper
    {
        private int Id { get; set; }
        private readonly string _link;
        public BetCityRuScrapper(string link)
        {
            Id = 1;
            _link = link;
        }

        public string GetBetCityRuDataAsync()
        {
            string json = null;

            _driver.Navigate().GoToUrl(_link);
            //Thread.Sleep(5000);

            try
            {
                List<ResultsChamp> data = null;

                if (WaitToLoad(By.XPath(".//div[@class='results-champ']")))
                    data = GetData(_driver.PageSource);

                json = JsonConvert.SerializeObject(data, Formatting.Indented);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return json;
        }

        private List<ResultsChamp> GetData(string content)
        {
            var document = new HtmlDocument();
            List<ResultsChamp> matchesData = new List<ResultsChamp>();

            try
            {
                document.LoadHtml(content);
                HtmlNodeCollection ligas = document.DocumentNode.SelectNodes(".//div[@class='results-champ']");

                foreach (var liga in ligas)
                {
                    var id_evn = 1;
                    List<ResultsChampEvents> resultsChampEvents = new List<ResultsChampEvents>();

                    var ligaName = liga.ChildNodes[0].SelectSingleNode(".//div[@class='results-champ__title-text']")?.InnerText;

                    if (!ligaName.Contains("football") && !ligaName.Contains("soccer"))
                        continue;

                    foreach (var item in liga.ChildNodes[1].SelectNodes(".//app-results-event"))
                    {
                        string competitors = item.SelectSingleNode(".//span[@class='results-event__name']")?.InnerText;
                        string score = item.SelectSingleNode(".//b")?.InnerText;

                        resultsChampEvents.Add(new ResultsChampEvents()
                        {
                            Id = id_evn++,
                            Competitors = competitors,
                            Score = score
                        });
                    }

                    matchesData.Add(new ResultsChamp()
                    {
                        Id = Id++,
                        LigaName = ligaName,
                        resultsChampEvents = resultsChampEvents
                    });
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return matchesData;
        }
    }
}
