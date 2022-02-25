namespace PdfCleaner;

public class PdfCleanerArgs
{
    public List<string> inputFiles { get; set; } = new List<string>();
    public string outputFilePrefix { get; set; } = String.Empty;

    public PdfCleanerArgs(List<string> inputFiles, string outputFilePrefix)
    {
        foreach (var inputFile in inputFiles)
        {
            if (!File.Exists(inputFile))
            {
                throw new FileNotFoundException($"File not found {this.inputFiles}");
            }

            this.inputFiles.Add(inputFile);
        }
        
        this.outputFilePrefix = outputFilePrefix;
    }

    
}