namespace WxsMerger
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            ValidateArgs(args);
            var contentTarget = ReadTargetWxsFile(args[0]);
            var content32bit = ReadSourceWxsFile(args[1]);
            var content64bit = ReadSourceWxsFile(args[2]);
            contentTarget = contentTarget.Replace("<!-- Files-x86.wxs -->", content32bit);
            contentTarget = contentTarget.Replace("<!-- Files-x64.wxs -->", content64bit);
            File.WriteAllText(args[3], contentTarget);
        }

        private static string ReadSourceWxsFile(string filename)
        {
            var lines = ReadWxsFile(filename);
            while (string.IsNullOrWhiteSpace(lines[0]))
            {
                lines.RemoveAt(0);
            }

            lines.RemoveAt(0);
            lines.RemoveAt(0);
            while (string.IsNullOrWhiteSpace(lines[lines.Count - 1]))
            {
                lines.RemoveAt(lines.Count - 1);
            }

            lines.RemoveAt(lines.Count - 1);
            return string.Join(Environment.NewLine, lines);
        }

        private static string ReadTargetWxsFile(string filename)
        {
            var lines = ReadWxsFile(filename);
            return string.Join(Environment.NewLine, lines);
        }

        private static List<string> ReadWxsFile(string filename)
        {
            var lines = new List<string>();
            using (var file = new FileStream(filename, FileMode.Open))
            {
                using (var reader = new StreamReader(file))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (line != null)
                        {
                            lines.Add(line);
                        }
                    }
                }
            }

            return lines;
        }

        private static void ValidateArgs(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine(
                    "This command requires 4 arguments, in order: Template filename, 32Bit wxs filename, 64bit wxs filename, Output wxs filename");
                Environment.Exit(-1);
            }

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("The specified template filename does not exist.");
                Environment.Exit(-2);
            }

            if (!File.Exists(args[1]))
            {
                Console.WriteLine("The specified 32bit wxs filename does not exist.");
                Environment.Exit(-3);
            }

            if (!File.Exists(args[2]))
            {
                Console.WriteLine("The specified 64bit wxs filename does not exist.");
                Environment.Exit(-4);
            }
        }
    }
}