using System;
using System.Collections.Generic;
using System.IO;
using Utilities;

namespace PrismFileExporter
{
    public class PrismExporter
    {
        private static PrismExporter? _instance;
        private static readonly object _lock = new();
        
        public static PrismExporter Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new PrismExporter();
                }
            }
        }
        
        /// <summary>
        /// Saves 'string' variable as .prism file in directoryPath with concrete filename
        /// </summary>
        public void SaveSingle(string content, string filename, string directoryPath)
        {
            Directory.CreateDirectory(directoryPath);

            string txtPath = Path.Combine(directoryPath, filename + ".txt");
            string prismPath = Path.Combine(directoryPath, filename + ".prism");

            File.WriteAllText(txtPath, content);
            
            if (File.Exists(prismPath))
                File.Delete(prismPath);

            File.Move(txtPath, prismPath);
        }
        
        /// <summary>
        /// Saves a dictionary of Prism code strings into .prism files directly inside the 'GeneratedPrismFiles' folder at the solution root.
        /// </summary>
        public void SaveMultiple(Dictionary<string, string> budgetToContent, string baseDirectoryName = "ParetoFrontModels")
        {
            string solutionRoot = SolutionRootFinder.Instance.GetSolutionRoot();
            string baseDirPath = Path.Combine(solutionRoot, baseDirectoryName);

            Directory.CreateDirectory(baseDirPath);

            foreach (var kvp in budgetToContent)
            {
                string budgetKey = kvp.Key; // e.g. "attacker100_defender200"
                string content = kvp.Value;

                // Save file directly in baseDirPath
                string fileName = $"{budgetKey}.prism";
                string filePath = Path.Combine(baseDirPath, fileName);

                File.WriteAllText(filePath, content);

                // Register in global dictionary
                PrismFileRegistry.FilePathsByBudget[budgetKey] = filePath;
            }
        }


    }
}