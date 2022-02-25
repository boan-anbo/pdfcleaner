// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using iText.Kernel.Exceptions;
using iText.Kernel.Pdf;

namespace PdfCleaner
{
    class MyClass
    {
        public static int Main(string[] args)
        {
            var inputFileOption = new Option<string>("-i");

            var prefixOption = new Option<string>("-p");

            var rootCommand = new RootCommand
            {
                inputFileOption,
                prefixOption
            };


            rootCommand.Description = "PdfCleaner removes all the annotations from a pdf file";
            rootCommand.SetHandler((string inputPath, string outputPrefix) =>
            {
                // check if input path is a directory
                bool isDirectory = System.IO.Directory.Exists(inputPath);

                try
                {
                    if (isDirectory)
                    {
                        var allFiles = System.IO.Directory.GetFiles(inputPath, "*.pdf").ToList();
                        var options = new PdfCleanerArgs(allFiles, outputPrefix);

                        RemoveAnnotationsFromMultiplePdFs(options);
                    }
                    else if (!File.Exists(inputPath))
                    {
                        Console.WriteLine("Input file is not provided");
                        return;
                    }
                    else if (File.Exists(inputPath))
                    {
                        RemoveAnnotationsFromMultiplePdFs(new PdfCleanerArgs(new List<string> { inputPath }, outputPrefix));
                    }
                }
                catch (PdfException e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
            }, inputFileOption, prefixOption);
            return rootCommand.Invoke(args);
        }

        public static void RemoveAnnotationsFromMultiplePdFs(PdfCleanerArgs pdfCleanerArgs)
        {
            foreach (var filePath in pdfCleanerArgs.inputFiles)
            {
                RemoveAnnotations(filePath, pdfCleanerArgs);
            }
        }

        public static void RemoveAnnotations(string inputFilePath, PdfCleanerArgs pdfCleanerArgs)
        {
            try
            {
                PdfDocument pdfDoc =
                    new PdfDocument(new PdfReader(inputFilePath), new PdfWriter(GetOutputPath(inputFilePath, pdfCleanerArgs.outputFilePrefix)));

                var annotationsRemoved = 0;

                var numberOfPages = pdfDoc.GetNumberOfPages();
                for (int i = 1; i < numberOfPages; i++)
                {
                    var page = pdfDoc.GetPage(i);
                    for (int j = 0; j < page.GetAnnotations().Count; j++)
                    {
                        var annotation = page.GetAnnotations()[j];
                        var subtype = annotation.GetSubtype();

                        if (subtype != PdfName.Link)
                        {
                            // page.RemoveAnnotation(annotation);
                            page.RemoveAnnotation(annotation);
                            annotationsRemoved++;
                        }
                    }
                }

                Console.WriteLine($"{annotationsRemoved} Annotations Removed From {inputFilePath}");

                pdfDoc.Close();
            }
            catch (Exception j)
            {
                Console.WriteLine(j);
            }
        }
        
        
    public static string GetOutputPath(string inputPath, string outputFilePrefix)
    {
        return (outputFilePrefix != null && outputFilePrefix.Length > 0 ? outputFilePrefix : "[C] ") + inputPath;
    }
    }
}