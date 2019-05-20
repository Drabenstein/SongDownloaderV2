using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace ConvertSongNames
{
    class Program
    {
        private static string[] PhrasesToDelete = new string[]
        {
            "(Official Video)",
            "[Official Video]",
            "[Official Audio]",
            "(Official Audio)",
            "(Official Lyric Video)",
            "[Official Lyric Video]",
            "(Lyrics)",
            "[Lyrics]",
            "(Audio)",
            "[Audio]",
            "[Ultra Music]",
            "(Ultra Music)",
            "(Lyric Video)",
            "[Lyric Video]",
            "(Official Music Video)",
            "[Official Music Video]"
        };

        private static string[] AuthorSuffixes = new string[]
        {
            "ft.",
            "feat.",
            "feat",
            "featuring",
            "vs."
        };

        static void Main(string[] args)
        {
            if (args.Length == 1)
            {
                DirectoryInfo dir;

                do
                {
                    Console.Clear();
                    dir = new DirectoryInfo(args[0]);
                    if (!dir.Exists)
                    {
                        Console.WriteLine($"Niepoprawna ścieżka folderu: {dir.FullName}!");
                        Thread.Sleep(2000);
                    }
                } while (!dir.Exists);

                Console.WriteLine("Looking for files");

                foreach (var file in dir.GetFiles())
                {
                    var filename = file.Name;

                    foreach (var phrase in PhrasesToDelete)
                    {
                        if (filename.ToLower().Contains(phrase.ToLower()))
                        {
                            filename = filename.Remove(filename.IndexOf(phrase), phrase.Length);
                        }
                    }

                    foreach (var suffix in AuthorSuffixes)
                    {
                        if (filename.ToLower().Contains(suffix.ToLower()))
                        {
                            int indexOfSuffix = filename.ToLower().IndexOf(suffix.ToLower());
                            string authorPart = filename.Substring(indexOfSuffix, filename.Length - indexOfSuffix).Replace(file.Extension, "");
                            int indexOfDash = filename.IndexOf('-');
                            if (indexOfDash == -1)
                            {
                                continue;
                            }

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

                            char c = filename[indexOfSuffix - 1];
                            if (c == '(' || c == '[')
                            {
                                if (filename.Last() == ')' || filename.Last() == ']')
                                {
                                    indexOfSuffix--;
                                    authorPart.Remove(authorPart.Length - 1);
                                }
                            }

                            if (indexOfSuffix > indexOfDash)
                            {
                                int max = filename.Length - indexOfSuffix - file.Extension.Length;
                                filename = filename.Remove(indexOfSuffix, max);
                                filename = filename.Insert(indexOfDash, authorPart + " ");
                            }
                        }
                    }

                    if (filename != file.Name && !File.Exists(Path.Combine(file.Directory.FullName, filename))) ;
                    {
                        filename = Regex.Replace(filename, @"\s+", " ").Trim();
                        if (filename[filename.Length - file.Extension.Length - 1] == ' ')
                        {
                            filename = filename.Remove(filename.Length - file.Extension.Length - 1, 1);
                        }
                        File.Move(file.FullName, Path.Combine(file.Directory.FullName, filename));
                        Console.WriteLine($"Zmieniono nazwę z: {file.FullName} na: {filename}");
                    }
                }

                Console.WriteLine("Zakończono zmianę nazw");
            }
            else
            {
                Console.WriteLine($"1 PARAMETER IS REQUIRED BUT {args.Length} HAS BEEN SUPPLIED INSTEAD");
            }
        }
    }
}
