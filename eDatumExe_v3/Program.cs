using System;

namespace eDatumExe_v3
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Press 'Esc' for cancel!");
            Scrapper.GetDataWithin3MinutesInterval();
        }
    }
}
