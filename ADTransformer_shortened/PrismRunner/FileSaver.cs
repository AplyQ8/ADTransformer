using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace PrismRunner;

public class FileSaver
{
    public static void PrismOutputToCsv(List<PrismOutputResult> results, string savePath, string fileName)
    {
        // string solutionRoot = SolutionRootFinder.Instance.GetSolutionRoot();
        // string baseDirPath = Path.Combine(solutionRoot, csvPath);
        // var sb = new StringBuilder();
        // sb.AppendLine("attackerCost,defenderCost");
        //
        // foreach (var result in results)
        // {
        //     // sb.AppendLine
        //     //     ($"\"{result.AttackerCost.ToString(CultureInfo.InvariantCulture)}\"," +
        //     //      $"\"{result.DefenderCost.ToString(CultureInfo.InvariantCulture)}\"");
        //     sb.AppendLine($"{result.AttackerCost.ToString(CultureInfo.InvariantCulture)},{result.DefenderCost.ToString(CultureInfo.InvariantCulture)}");
        //
        // }
        //
        // File.WriteAllText(baseDirPath, sb.ToString());
        string fullFilePath = Path.Combine(savePath, fileName);

        var sb = new StringBuilder();

        // Заголовок CSV
        sb.AppendLine("attackerCost,defenderCost");

        foreach (var result in results)
        {
            sb.AppendLine($"{result.AttackerCost.ToString(CultureInfo.InvariantCulture)},{result.DefenderCost.ToString(CultureInfo.InvariantCulture)}");
        }

        // Записываем данные в файл (создаст файл, если его нет, или перезапишет существующий)
        File.WriteAllText(fullFilePath, sb.ToString());
    }
}