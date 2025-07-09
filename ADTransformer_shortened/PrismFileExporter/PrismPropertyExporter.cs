using System.Collections.Generic;
using System.IO;
using Utilities;

namespace PrismFileExporter;

public class PrismPropertyExporter
{
    private static PrismPropertyExporter? _instance;
    private static readonly object _lock = new();
        
    public static PrismPropertyExporter Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new PrismPropertyExporter();
            }
        }
    }

    public void GeneratePropertyList(string content, string filename, string directoryPath)
    {
        
    }

    public string GenerateParetoFrontProperties(string content, string baseDirectoryName = "ParetoFrontProps")
    {
        // string solutionRoot = SolutionRootFinder.Instance.GetSolutionRoot();
        // string baseDirPath = Path.Combine(solutionRoot, baseDirectoryName);
        //
        // Directory.CreateDirectory(baseDirPath);
        
        string fileName = "paretoFrontProperty.props";
        //string filePath = Path.Combine(baseDirPath, fileName);
        
        string filePath = Path.Combine(baseDirectoryName, fileName);
        
        File.WriteAllText(filePath, content);

        return filePath;
    }
}