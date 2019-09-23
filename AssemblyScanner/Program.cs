using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\DEV\Assemblies";
            Scanner scanner = new Scanner();
            var result = scanner.Scan(path);

            Print(result);

            Console.ReadKey();
        }

        static void Print(Dictionary<string, IEnumerable<string>> dictionary)
        {
            if (dictionary == null)
            {
                Console.WriteLine("Sending parameter in Null");
                return;
            }

            foreach (var pair in dictionary)
            {
                Console.WriteLine($"Class name: {pair.Key}");
                Console.WriteLine($"Methods:");
                foreach (string s in pair.Value)
                    Console.WriteLine($"\t{s}");

                Console.WriteLine(new string('-', 30));
            }
        }
    }
}
