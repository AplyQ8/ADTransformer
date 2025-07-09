using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrismCodeGenerator.Models;
using PrismCodeGenerator.Utils;

namespace PrismCodeGenerator.SectionGenerators;

public class RewardsSectionGenerator: ICodeSectionGenerator
{
    public string Generate(IEnumerable<Node> nodes)
    {
        var allNodes = TreeWalker.Flatten(nodes);

        var attackerNodes = allNodes.Where(n => n.IsAttackerNode).ToList();
        var defenderNodes = allNodes.Where(n => n.IsDefenderNode).ToList();

        var sb = new StringBuilder();

        GenerateRewardsForPlayer(sb, PlayerType.Attacker, attackerNodes, StaticGlobalVariableHolder.AttackerRewardsName);
        GenerateRewardsForPlayer(sb, PlayerType.Defender, defenderNodes, StaticGlobalVariableHolder.DefenderRewardsName);

        return sb.ToString();
    }
    
    
    private void GenerateCombinedRewardsForPlayer(StringBuilder sb, PlayerType playerType, List<Node> nodes, string rewardName)
    {
        sb.AppendLine($"rewards \"{rewardName}\"");

        // skip event - стоит 0
        var skipEventName = playerType == PlayerType.Attacker ? "attack_skip" : "defense_skip";
        sb.AppendLine($"    [{skipEventName}] true : 0;");

        // Все непустые подмножества (комбинации) узлов
        foreach (var combination in GetAllNonEmptyCombinations(nodes))
        {
            var eventName = NameFormatter.GetCombinedEventName(playerType, combination);
            var costSum = combination.Sum(n => n.Cost ?? 0);
            sb.AppendLine($"    [{eventName}] true : {costSum};");
        }

        sb.AppendLine("endrewards");
    }
    
    private void GenerateRewardsForPlayer(StringBuilder sb, PlayerType playerType, List<Node> nodes, string rewardName)
    {
        sb.AppendLine($"rewards \"{rewardName}\"");

        foreach (var node in nodes)
        {
            var eventName = NameFormatter.GetEventName(playerType, node); // [attack_a1n1] или [defense_d2n3]
            int cost = node.Cost ?? 0;
            sb.AppendLine($"    {eventName} true : {cost};");
        }

        // Также добавим [attack_end_turn] или [defense_end_turn] с 0 стоимостью
        var endTurnEvent = playerType == PlayerType.Attacker
            ? StaticGlobalVariableHolder.AttackerEndTurnEvent
            : StaticGlobalVariableHolder.DefenderEndTurnEvent;
        sb.AppendLine($"    {endTurnEvent} true : 0;");

        sb.AppendLine("endrewards");
    }


    private IEnumerable<List<Node>> GetAllNonEmptyCombinations(List<Node> nodes)
    {
        int n = nodes.Count;
        // Перебираем все битовые маски от 1 до 2^n - 1 (все непустые подмножества)
        for (int mask = 1; mask < (1 << n); mask++)
        {
            var combination = new List<Node>();
            for (int i = 0; i < n; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    combination.Add(nodes[i]);
                }
            }
            yield return combination;
        }
    }
}