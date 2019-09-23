using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AssemblyScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\DEV\Assemblies";
            
            // use with class
            //Scanner scanner = new Scanner();
            //var result = scanner.Scan(path);

            // use only method
            var result = Scan(path);

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

        public static Dictionary<string, IEnumerable<string>> Scan(string directory)
        {
            var result = new Dictionary<string, IEnumerable<string>>();

            if (!Directory.Exists(directory))
                throw new DirectoryNotFoundException($"Select directory: {directory} is not found!");

            var di = new DirectoryInfo(directory);
            var files = di.GetFiles("*.DLL", SearchOption.AllDirectories);

            if (files.Length == 0)
                throw new FileNotFoundException($"In selected directory: [{directory}], not found files with *.DLL extension");

            foreach (FileInfo file in files)
            {
                try
                {
                    foreach (Type type in Assembly.LoadFile(file.FullName).GetTypes())
                    {
                        if (type.IsClass)
                        {
                            string name = type.Name;
                            var methods = type.GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance).Where(info => !info.IsSpecialName);
                            var names = methods.Select(info => info.Name);

                            if (result.ContainsKey(type.Name))
                                name = type.Name + "__" + Path.GetRandomFileName();

                            result.Add(name, names);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Failed loading assembly: {file.FullName}");
                    Console.WriteLine(e.Message);
                    if (e is ReflectionTypeLoadException reflectionTypeLoadException)
                        if (reflectionTypeLoadException.LoaderExceptions.Length > 0)
                            foreach (Exception exception in reflectionTypeLoadException.LoaderExceptions)
                                Console.WriteLine(exception.Message);
                    Console.WriteLine();
                    continue;
                }

            }

            return result;
        }
    }
}
