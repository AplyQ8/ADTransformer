using System.Collections.Generic;
using System.Linq;
using PrismCodeGenerator.Models;

namespace PrismCodeGenerator;

public class DefenderCostSummarizer
{
    static IEnumerable<Node> Flatten(Node node)
    {
        yield return node;
        foreach (var child in node.Children)
        {
            foreach (var descendant in Flatten(child))
            {
                yield return descendant;
            }
        }
    }

    private static List<int> GetAllDefenderLeafActionCosts(IEnumerable<Node> nodes)
    {
        return nodes
            .Where(n => n.IsActionNode && n.IsDefenderNode && n.Cost.HasValue)
            .Select(n => n.Cost.Value)
            .ToList();
    }

    public static List<int> AllSubsetSums(IEnumerable<Node> nodes)
    {
        var flatNodes = nodes.SelectMany(n => Flatten(n));
        var defenderCosts = GetAllDefenderLeafActionCosts(flatNodes);
        var sums = new HashSet<int>();
        int total = 1 << defenderCosts.Count;

        for (int mask = 0; mask < total; mask++)
        {
            int sum = 0;
            for (int i = 0; i < defenderCosts.Count; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    sum += defenderCosts[i];
                }
            }
            sums.Add(sum);
        }

        return sums.OrderBy(x => x).ToList();
    }
}