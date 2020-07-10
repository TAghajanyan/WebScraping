using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace eDatumExe_v3
{
    public class OneXBet : Scrapper
    {
        private int Id { get; set; }
        private readonly string _link;
        public OneXBet(string link)
        {
            Id = 1;
            _link = link;
        }

        public string GetOneXBetData()
        {
            string json = null;
            _driver.Navigate().GoToUrl(_link);

            //_driver.ExecuteScript("scroll(0, 300);");
            //Thread.Sleep(250);

            try
            {
                List<ResultsChamp> data = null;

                if (WaitToLoad(By.XPath(".//div[@class='c-games__col']")))
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

                var football = document.DocumentNode.SelectNodes(".//div[@class='c-games__item']")
                    .FirstOrDefault(x => x.SelectSingleNode(".//div[@class='c-games__sport']").InnerText == "Football");

                var ligas = football.ChildNodes;
                ligas.Remove(0);
                ligas.Remove(0);

                foreach (var liga in ligas)
                {
                    var id_evn = 1;
                    List<ResultsChampEvents> resultsChampEvents = new List<ResultsChampEvents>();

                    var ligaName = liga.ChildNodes[0].SelectSingleNode(".//div[@class='c-games__name']")?.InnerText;

                    foreach (var item in liga.SelectNodes(".//div[@class='c-games__row u-nvpd c-games__row_light c-games__row_can-toggle']"))
                    {
                        string competitors = item.SelectSingleNode(".//div[@class='c-games__opponents u-dir-ltr']")?.InnerText;
                        string score = item.SelectSingleNode(".//div[@class='c-games__results u-mla u-tar']")?.InnerText;

                        resultsChampEvents.Add(new ResultsChampEvents()
                        {
                            Id = id_evn++,
                            Competitors = StringBeauty(competitors),
                            Score = StringBeauty(score)
                        });
                    }

                    matchesData.Add(new ResultsChamp()
                    {
                        Id = Id++,
                        LigaName = StringBeauty(ligaName),
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