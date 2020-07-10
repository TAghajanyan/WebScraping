using System.Collections.Generic;

namespace eDatumExe_v3
{
    public class ResultsChamp
    {
        public int Id { get; set; }
        public string LigaName { get; set; }
        public List<ResultsChampEvents> resultsChampEvents { get; set; }
    }
}
