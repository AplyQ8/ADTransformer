using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ADTransformer;
using NodeAdapters;
using ParetoFrontBuilder;
using PrismCodeGenerator;
using PrismCodeGenerator.Models;
using PrismFileExporter;
using PrismRunner;
using Utilities;

namespace CommandExecutor
{
    public class CommandExecutor
    {
        private readonly ExecutionStatus _status = new ExecutionStatus();
        public ExecutionStatus Status => _status;
        
        public event Action? StatusChanged;
        public event Action? ProgressChanged;
        
        private void SetStatus(string message)
        {
            _status.SetStatus(message);
            StatusChanged?.Invoke();
        }

        private void SetProgress(int progress)
        {
            _status.SetProgress(progress);
            ProgressChanged?.Invoke();
        }

        public async Task Execute(string input)
        {
            Status.Reset();
            var args = ParseArguments(input);

            if (args.Count == 0) 
                throw new CommandExecutorException("No command provided.");
                

            string command = args[0];

            switch (command)
            {
                case "-model":
                    ExecuteModelCommand(args);
                    break;
                case "-pareto":
                    await ExecuteParetoCommand(args);
                    break;
                default:
                    throw new CommandExecutorException($"Unknown command: {command}");
            }
        }
        
        private void ExecuteModelCommand(List<string> args)
        {
            if (args.Count < 3)
            {
                throw new CommandExecutorException("Usage: -model [path_to_XML] [path_to_save]");
            }

            string xmlPath = args[1];
            string savePath = args[2];
            
            EnsureDirectoryExists(savePath);

            BuildModel(xmlPath, savePath);
        }
        
        private async Task ExecuteParetoCommand(List<string> args)
        {
            if (args.Count < 3)
            {
                throw new CommandExecutorException("Usage: -pareto [path_to_XML] [path_to_save] [-model] [-data]");
            }

            string xmlPath = args[1];
            string savePath = args[2];
            
            EnsureDirectoryExists(savePath);
            
            bool saveModel = args.Contains("-model");
            bool saveData = args.Contains("-data");

            await BuildParetoFront(xmlPath, savePath, saveModel, saveData);
        }
        
        private List<string> ParseArguments(string input)
        {
            // Простейший парсер: разбиваем по пробелам, можно заменить на более сложный при необходимости
            return new List<string>(input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
        }
        
        private void BuildModel(string xmlPath, string savePath)
        {
            SetStatus("Parsing Tree");
            SetProgress(0);
            var parser = new AdtParser();
            AdtNode treeRoot;
            try
            {
                treeRoot = parser.Parse(xmlPath);
            }
            catch (ParserException ex)
            {
                throw new CommandExecutorException($"{ex.Message}");
            }
            var adapter = new AdtToPrismNodeAdapter();
            Node prismRoot = adapter.Convert(treeRoot);
            SetStatus("Building model");
            SetProgress(33);
            var composer = new PrismCodeComposer();
            string code = composer.Compose(new[] { prismRoot });
            SetStatus("Saving Model");
            SetProgress(66);
            PrismExporter.Instance.SaveSingle(code, "generatedSample", $@"{savePath}");
            SetStatus($"The model has been built and saved to {savePath}");
            SetProgress(100);
        }
        
        private async Task BuildParetoFront(string xmlPath, string savePath, bool saveModel, bool saveData)
        {
            SetStatus("Parsing Tree");
            var parser = new AdtParser();
            AdtNode treeRoot;
            try
            {
                treeRoot = parser.Parse(xmlPath);
            }
            catch (ParserException ex)
            {
                throw new CommandExecutorException($"{ex.Message}");
            }
            var adapter = new AdtToPrismNodeAdapter();
            Node prismRoot = adapter.Convert(treeRoot);
            SetStatus("Building model");
            var composer = new PrismCodeComposer();
            string code = composer.Compose(new[] { prismRoot });
            
            var generator = new PropertyGenerator();
            string propertyList = generator.GeneratePropertyList(new[] { prismRoot });
            var propsPath = PrismPropertyExporter.Instance.GenerateParetoFrontProperties(propertyList, savePath);
            
            SetStatus("Saving Model");
            PrismExporter.Instance.SaveSingle(code, "generatedSample", $@"{savePath}");
                
            SetStatus("Building Pareto Front");
            string modelPath = Path.Combine($@"{savePath}", "generatedSample.prism");
            var results = await RunPrism(prismRoot , modelPath, propsPath);
            
            SetStatus("Saving Data");
            string pathToCSV = Path.Combine(savePath, "data.csv");
            SaveData(results.Results, savePath, "data.csv");
                
            SetStatus("Plotting");
            BuildPareto(results.DefenderWon, savePath, pathToCSV);
            
            SetStatus($"Done and every element has been saved to {savePath}");
        }
        
        private async Task<ParetoResults> RunPrism(Node prismRoot, string savePath, string propertyPath)
        {
            string propsPath = Path.Combine($@"{propertyPath}");
            string modelPath = Path.Combine($@"{savePath}");
            var prismRunner = new PrismRunner.PrismRunner();
            prismRunner.ProgressUpdated += SetProgress;
            var results = await prismRunner.RunModelWithVaryingBudgets(modelPath, propsPath, new[] { prismRoot });
            prismRunner.ProgressUpdated -= SetProgress;
            return results;
        }
        
        static void BuildPareto(bool defenderWon, string savePath, string pathToCSV)
        {
            ParetoBuilder.RunParetoScript(pathToCSV, defenderWon, savePath);
        }

        private static void SaveData(List<PrismOutputResult> results, string savePath, string fileName)
        {
            FileSaver.PrismOutputToCsv(results, savePath, fileName);
            
        }
        
        private void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
                throw new CommandExecutorException($"Save path is not a valid directory: {path}");
        }
    }
}