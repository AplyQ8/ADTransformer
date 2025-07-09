using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrismCodeGenerator.Models;
using PrismCodeGenerator.Utils;

namespace PrismCodeGenerator;

public class PropertyGenerator
{
    /// <summary>
    /// Generates property list based on List<> of strings;
    /// </summary>
    /// <param name="properties">Properties in the list</param>
    public string GeneratePropertyList(List<string> properties)
    {
        return "";
    }
    
    /// <summary>
    /// Generates property list using all subset sums of defender node costs.
    /// </summary>
    /// <param name="defenderNodes">List of defender action root nodes</param>
    public string GeneratePropertyList(IEnumerable<Node> nodes)
    {

        StringBuilder sb = new StringBuilder();
        sb.AppendLine(
            $"<<attacker>> Pmax=? [ F \"{StaticGlobalVariableHolder.TopEventLabelName}\" & (\"{StaticGlobalVariableHolder.GameEndLabelName}\" | \"deadlock\") ]"
        );
        sb.AppendLine();

        return sb.ToString();
    }

    private string GenerateOldVersion(IEnumerable<Node> nodes)
    {
        var allNodes = TreeWalker.Flatten(nodes);
        var defenderNodes = allNodes.Where(n => n.IsDefenderNode).ToList();
        
        var budgetSums = DefenderCostSummarizer.AllSubsetSums(defenderNodes);
        
        StringBuilder sb = new StringBuilder();
        foreach (var budget in budgetSums)
        {
            sb.AppendLine(
                $"filter(min, Pmax=? [ F \"{StaticGlobalVariableHolder.SystemSecureLabelName}\" & (\"{StaticGlobalVariableHolder.GameEndLabelName}\" | \"deadlock\") ], defender_budget={budget})"
            );
            sb.AppendLine();
        }

        return sb.ToString();
    }

}