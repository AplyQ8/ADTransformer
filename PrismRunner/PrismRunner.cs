using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PrismCodeGenerator;
using PrismCodeGenerator.Models;

namespace PrismRunner
{
    public class PrismRunner
    {
        private readonly string prismBatPath;
        private readonly string prismWorkingDir;
        
        public event Action<int>? ProgressUpdated;
        
        public PrismRunner()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            prismBatPath = Path.Combine(baseDir, "prism-games-3.2.1", "bin", "prism.bat");
            prismWorkingDir = Path.GetDirectoryName(prismBatPath) ?? "";

            if (!File.Exists(prismBatPath))
            {
                throw new FileNotFoundException("prism.bat has not been found: " + prismBatPath);
            }
        }
        
        public void RunSingleModel(string modelPath, string propsPath)
        {
            ConsolePrismOutput(modelPath, propsPath);
        }
        
        public async Task<ParetoResults> RunModelWithVaryingBudgets(string modelPath, string propsPath, IEnumerable<Node> nodes)
        {
            var validResults = new List<PrismOutputResult>();
            int? minFailedAttackerBudget = null;
            var locker = new object();

            var attackSums = AttackerCostSummarizer.AllSubsetSums(nodes).ToList();
            var defenderSums = DefenderCostSummarizer.AllSubsetSums(nodes).ToList();

            int totalAttacks = attackSums.Count;
            int completed = 0;

            int maxConcurrency = Environment.ProcessorCount;
            var semaphore = new SemaphoreSlim(maxConcurrency);
            var tasks = new List<Task>();

            var token = new CancellationTokenSource().Token;
            int? minValidDefenderSum = null;

            foreach (int attackerBudget in attackSums)
            {
                await semaphore.WaitAsync(token);

                tasks.Add(Task.Run(() =>
                {
                    try
                    {
                        if (token.IsCancellationRequested) return;

                        bool foundValid = false;

                        foreach (var defenderSum in defenderSums)
                        {
                            if (token.IsCancellationRequested) return;

                            string output = "", errors = "";
                            var startInfo = CreatePrismProcessWithConstants(modelPath, propsPath, defenderSum, attackerBudget);
                            RunPrismProcess(startInfo, ref output, ref errors);

                            int result;
                            try
                            {
                                result = PrismOutputParser.ResultReturner(output);
                            }
                            catch (NoAppropriateResult)
                            {
                                continue;
                            }

                            if (result == 0)
                            {
                                foundValid = true;
                                lock (locker)
                                {
                                    validResults.Add(new PrismOutputResult(attackerBudget, defenderSum));

                                    if (minValidDefenderSum == null || defenderSum > minValidDefenderSum)
                                    {
                                        minValidDefenderSum = defenderSum;

                                        // Обрезаем список до значений >= minValidDefenderSum
                                        defenderSums = defenderSums.Where(d => d >= minValidDefenderSum.Value).ToList();
                                    }
                                }
                                break;
                            }
                        }

                        if (!foundValid)
                        {
                            lock (locker)
                            {
                                if (minFailedAttackerBudget == null || attackerBudget < minFailedAttackerBudget)
                                {
                                    minFailedAttackerBudget = attackerBudget;
                                }
                            }
                        }
                    }
                    catch (Exception ex) when (!(ex is NoAppropriateResult))
                    {
                        lock (locker)
                        {
                            if (minFailedAttackerBudget == null || attackerBudget < minFailedAttackerBudget)
                            {
                                minFailedAttackerBudget = attackerBudget;
                            }
                        }
                    }
                    finally
                    {
                        int current = Interlocked.Increment(ref completed);
                        int percent = (int)((current / (double)totalAttacks) * 100);
                        ProgressUpdated?.Invoke(percent);

                        semaphore.Release();
                    }
                }, token));
            }

            await Task.WhenAll(tasks);

            if (minFailedAttackerBudget != null)
            {
                var filteredValidResults = validResults
                    .Where(r => r.AttackerCost <= minFailedAttackerBudget.Value)
                    .ToList();

                filteredValidResults.Add(new PrismOutputResult(minFailedAttackerBudget.Value, defenderSums.Last()));

                //FileSaver.PrismOutputToCsv(filteredValidResults);
                //return false;
                return new ParetoResults(false, filteredValidResults);
            }
            else
            {
                //FileSaver.PrismOutputToCsv(validResults);
                //return true;
                return new ParetoResults(true, validResults);
            }
        }

        public bool RunModelWithVaryingBudgetsNonAsync(string modelPath, string propsPath, IEnumerable<Node> nodes)
        {
            List<PrismOutputResult> allResults = new List<PrismOutputResult>();
            var attackSums = AttackerCostSummarizer.AllSubsetSums(nodes);
            var defenderSums = DefenderCostSummarizer.AllSubsetSums(nodes);
            var maxDefenderBudget = DefenderCostSummarizer.AllSubsetSums(nodes).Last();
            var maxAttackerBudget = attackSums.Last();
            
            for (int attackerBudget = 0; attackerBudget <= maxAttackerBudget; attackerBudget++)
            {
                string output = "";
                string errors = "";
            
                RunPrismProcess(CreatePrismProcessWithConstants(modelPath, propsPath, maxDefenderBudget, attackerBudget), ref output, ref errors);
                var results = PrismOutputParser.ParseModelResults(output);
                int defenderParetoPoint;
                try
                {
                    defenderParetoPoint = ParetoPointFinder.FindPoint(results, defenderSums);
                }
                catch (NoAppropriateResult)
                {
                    //FileSaver.PrismOutputToCsv(allResults);
                    return false;
                }
            
                var outputResult = new PrismOutputResult(attackerBudget, defenderParetoPoint);
                allResults.Add(outputResult);
            }
            
            //FileSaver.PrismOutputToCsv(allResults);
            return true;
        }
        
        private void ConsolePrismOutput(string modelPath, string propsPath)
        {
            string output = "";
            string errors = "";
            RunPrismProcess(CreatePrismProcess(modelPath, propsPath), ref output, ref errors);
            Console.WriteLine("Output:\n" + output);
            Console.WriteLine("Errors:\n" + errors);
        }
        

        private ProcessStartInfo CreatePrismProcess(string modelPath, string propsPath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"\"{prismBatPath}\" \"{Path.GetFullPath(modelPath)}\" \"{Path.GetFullPath(propsPath)}\"\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = prismWorkingDir
            };
            return startInfo;
        }
        private ProcessStartInfo CreatePrismProcessWithConstants(string modelPath, string propsPath, int defenderBudget, int attackerBudget)
        {
            string constants = $"-const INIT_DEFENDER_BUDGET={defenderBudget},INIT_ATTACKER_BUDGET={attackerBudget}";
    
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"\"{prismBatPath}\" \"{Path.GetFullPath(modelPath)}\" \"{Path.GetFullPath(propsPath)}\" {constants}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = prismWorkingDir
            };
            return startInfo;
        }


        private void RunPrismProcess(ProcessStartInfo startInfo, ref string output, ref string errors)
        {
            try
            {
                using var process = Process.Start(startInfo);
                output = process.StandardOutput.ReadToEnd();
                errors = process.StandardError.ReadToEnd();

                process.WaitForExit();
            }
            catch (Exception ex)
            {
                throw new Exception("PRISM startup error: " + ex.Message);
            }
        }
        
    }


}