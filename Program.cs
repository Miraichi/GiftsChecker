using System;
using System.IO;
using System.Linq;

namespace GiftsChecker
{
    class Program
    {
        static HumbleBundle HBChecker = new HumbleBundle();

        static long Indx = 0;
        static long AllStrings = 0;

        static void Main(string[] args)
        {
            Console.Title = "GiftsChecker ~ v0.0.2";
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.Write("File with urls -> ");
            var file = Console.ReadLine();

            if (file.StartsWith("\""))
                file = file.Replace("\"", "");

            Console.WriteLine();
            AllStrings = File.ReadLines(file).Count();

            
            foreach (var line in File.ReadLines(file))
            {
                Indx++;
                HBChecker.Search(line);
                Console.Title = "GiftsChecker ~ v0.0.2 ~ Checked: " + Indx + "/" + AllStrings;
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("..End! Count urls: " + HBChecker.GetCountUrls());
            Console.ReadKey();
        }
    }
}
