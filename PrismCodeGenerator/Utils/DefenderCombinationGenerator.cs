using System.Collections.Generic;
using System.Linq;
using PrismCodeGenerator.Models;

namespace PrismCodeGenerator.Utils;

public class DefenderCombinationGenerator
{
    public static List<DefenderNodeCombination> GenerateDefenderCombinations(IEnumerable<Node> nodes)
    {
        var defenderNodes = nodes
            .SelectMany(n => TreeWalker.Flatten(n))
            .Where(n => n.IsDefenderNode && n.Cost.HasValue)
            .ToList();

        int count = defenderNodes.Count;
        int totalCombinations = 1 << count;
        var combinations = new List<DefenderNodeCombination>();

        for (int mask = 0; mask < totalCombinations; mask++)
        {
            var combo = new DefenderNodeCombination();
            int totalCost = 0;

            for (int i = 0; i < count; i++)
            {
                var node = defenderNodes[i];
                int value = (mask & (1 << i)) != 0 ? 1 : 0;
                string varName = NameFormatter.GetVariableName(node);

                combo.VariableAssignments[varName] = value;
                if (value == 1)
                    totalCost += node.Cost.Value;
            }

            combo.TotalCost = totalCost;
            combinations.Add(combo);
        }

        return combinations;
    }
}

public class DefenderNodeCombination
{
    public Dictionary<string, int> VariableAssignments { get; set; } = new();
    public int TotalCost { get; set; }

    public override string ToString()
    {
        var vars = string.Join(" ", VariableAssignments.Select(kv => $"{kv.Key}={kv.Value}"));
        return $"{vars} | cost={TotalCost}";
    }
}