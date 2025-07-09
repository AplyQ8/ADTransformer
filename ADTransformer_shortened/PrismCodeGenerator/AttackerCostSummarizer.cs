using System.Collections.Generic;
using System.Linq;
using PrismCodeGenerator.Models;

namespace PrismCodeGenerator;

public class AttackerCostSummarizer
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
    private static List<int> GetAllAttackerLeafActionCosts(IEnumerable<Node> nodes)
    {
        return nodes
            .Where(n => /*n.IsLeaf &&*/ n.IsActionNode && n.IsAttackerNode && n.Cost.HasValue)
            .Select(n => n.Cost.Value)
            .ToList();
    }
    public static List<int> AllSubsetSums(IEnumerable<Node> nodes)
    {
        var flatNodes = nodes.SelectMany(n => Flatten(n));
        var attackerCosts = GetAllAttackerLeafActionCosts(flatNodes);
        var sums = new HashSet<int>();
        int total = 1 << attackerCosts.Count;

        for (int mask = 0; mask < total; mask++)
        {
            int sum = 0;
            for (int i = 0; i < attackerCosts.Count; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    sum += attackerCosts[i];
                }
            }
            sums.Add(sum);
        }

        return sums.OrderBy(x => x).ToList();
    }
    
}