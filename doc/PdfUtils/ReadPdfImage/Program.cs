using PdfUtils;
using System;
using System.IO;

namespace ReadPdfImage
{
    class Program
    {
        static void Main()
        {
            var args = Environment.GetCommandLineArgs();
            var searchPath = Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(args[0]))), "Data");

            foreach (var filename in Directory.GetFiles(searchPath, "*.pdf", SearchOption.TopDirectoryOnly))
            {
                Console.WriteLine("Extracting images from {0}", Path.GetFileName(filename));

                var images = PdfImageExtractor.ExtractImages(filename);

                Console.WriteLine("{0} images found.", images.Count);
                Console.WriteLine();

                var directory = Path.GetDirectoryName(filename);

                foreach (var name in images.Keys)
                {
                    images[name].Save(Path.Combine(directory, name));
                }
            }
            Console.WriteLine("Done.");
            Console.ReadLine();
        }
    }
}
