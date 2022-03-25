using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mp4BoxParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() == 0)
            {
                Console.WriteLine("ファイルを指定してください。");
                return;
            }
            bool searchChildBox = args.Count() > 1 ? args[1] == "1" : true;
            ParseMp4Box(args[0], searchChildBox);
        }

        private static void ParseMp4Box(string filePath, bool searchChildBox)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("ファイルが存在しません。");
                return;
            }

            using (var parser = new Mp4BoxParser(filePath, searchChildBox))
            {
                foreach (var box in parser.List)
                {
                    byte[] space = new byte[box.Level];
                    for (int i = 0; i < box.Level; i++)
                    {
                        space[i] = (byte)' ';
                    }
                    string spaceStr = Encoding.UTF8.GetString(space, 0, space.Length);
                    Console.WriteLine($"{box.Offset:D20} {box.Size:D20} {spaceStr}{box.Name}");
                }
            }
            Console.ReadKey();
        }
    }
}
