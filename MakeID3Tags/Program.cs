using System;
using System.IO;

namespace MakeID3Tags
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                DirectoryInfo dir = new DirectoryInfo(args[0]);

                if (dir.Exists)
                {
                    foreach (var file in dir.GetFiles())
                    {
                        using (TagLib.File tagfile = TagLib.File.Create(file.FullName))
                        {
                            string filename = file.Name.Replace(file.Extension, "");
                            int indexOfDash = filename.IndexOf('-');
                            bool isDashPositionCorrect = false;

                            do
                            {

                                if (indexOfDash - 1 < 0 || indexOfDash + 1 >= filename.Length)
                                {
                                    int oldIndex = indexOfDash;
                                    indexOfDash = filename.IndexOf('-', oldIndex);
                                }
                                else if (filename[indexOfDash - 1] == ' ' || filename[indexOfDash + 1] == ' ')
                                {
                                    isDashPositionCorrect = true;
                                }

                            } while (!isDashPositionCorrect);

                            string Author = filename.Substring(0, indexOfDash);
                            string Title = filename.Substring(indexOfDash, filename.Length - indexOfDash);

                            tagfile.Tag.Performers = new string[] { Author };
                            tagfile.Tag.Title = Title;
                            tagfile.Save();

                            Console.WriteLine($"Dodano tagi IDv3 dla: {file.Name}");
                        }
                    }

                    Console.WriteLine("Zakończono dodawanie tagów IDv3");
                }
                else
                {
                    Console.WriteLine($"DIRECTORY DOES NOT EXIST: {dir.FullName}");
                }
            }
            else
            {
                Console.WriteLine($"INVALID NUMBER OF PARAMETERS ({args.Length})");
            }
        }
    }
}
