using System;
using System.Diagnostics;
using System.IO;

namespace ParetoFrontBuilder
{
    public class ParetoBuilder
    {
        public static void RunParetoScript(string csvPath, bool defenderWon, string savePath)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            //string pythonExe = Path.Combine(baseDir, "myenv", "Scripts", "python.exe");
            //string scriptPath = Path.Combine(baseDir, "Scripts", "paretoBuilder.py");
            string scriptExePath = Path.Combine(baseDir, "Scripts", "dist", "paretoBuilder", "paretoBuilder.exe");

            
            savePath = Path.GetFullPath(savePath);
            string outputImagePath = Path.Combine(savePath, "pareto_front.png");

            var psi = new ProcessStartInfo
            {
                FileName = scriptExePath,
                Arguments = $"\"{csvPath}\" {defenderWon.ToString().ToLower()} \"{outputImagePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Path.GetDirectoryName(scriptExePath)
            };
            using var process = new Process { StartInfo = psi };
            process.Start();

            string output = process.StandardOutput.ReadToEnd();
            string errors = process.StandardError.ReadToEnd();
            process.WaitForExit();
            // string imagePath = Path.Combine(Path.GetDirectoryName(scriptPath), "pareto_front.png");
            // if (File.Exists(imagePath))
            // {
            //     Process.Start(new ProcessStartInfo(imagePath) { UseShellExecute = true });
            // }

            Console.WriteLine("Python Output:");
            Console.WriteLine(output);
            if (!string.IsNullOrWhiteSpace(errors))
            {
                Console.WriteLine("Python Errors:");
                Console.WriteLine(errors);
            }
        }
    }
}