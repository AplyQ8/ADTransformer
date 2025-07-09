using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrismCodeGenerator.Models;
using PrismCodeGenerator.Utils;

namespace PrismCodeGenerator.SectionGenerators;

public class PlayersSectionGenerator : ICodeSectionGenerator
{
    public string Generate(IEnumerable<Node> nodes)
    {
        var allNodes = TreeWalker.Flatten(nodes);

        var attackers = allNodes.Where(n => n.IsAttackerNode);
        var defenders = allNodes.Where(n => n.IsDefenderNode);
        
        var sb = new StringBuilder();
        GenerateAttacker(ref sb, attackers);
        GenerateDefender(ref sb, defenders);
        GenerateGameManager(ref sb);

        return sb.ToString();
    }
    
    #region PlayerGenerators

    private void GenerateAttacker(ref StringBuilder sb, IEnumerable<Node> attackers)
    {
        var attackerList = attackers.ToList();
        sb.AppendLine("// Define players");
        sb.AppendLine($"player {StaticGlobalVariableHolder.AttackerPlayerName}");
        sb.AppendLine($"    {StaticGlobalVariableHolder.AttackerModuleName}, " +
                      string.Join(", ", GenerateActions(PlayerType.Attacker, attackerList)));
        sb.AppendLine("endplayer\n");
    }

    private void GenerateDefender(ref StringBuilder sb, IEnumerable<Node> defenders)
    {
        var defenderList = defenders.ToList();
        sb.AppendLine($"player {StaticGlobalVariableHolder.DefenderPlayerName}");
        sb.AppendLine($"    {StaticGlobalVariableHolder.DefenderModuleName}, " +
                      string.Join(", ", GenerateActions(PlayerType.Defender, defenderList)));
        sb.AppendLine("endplayer\n");
    }

    private void GenerateGameManager(ref StringBuilder sb)
    {
        sb.AppendLine($"player {StaticGlobalVariableHolder.GameManagerPlayerName}");
        sb.AppendLine($"    {StaticGlobalVariableHolder.GameManagerModuleName}, {StaticGlobalVariableHolder.GameManagerCheckEvent}");
        sb.AppendLine("endplayer");
    }
    
    private IEnumerable<string> GenerateActionCombinations(PlayerType player, List<Node> actionNodes)
    {
        int n = actionNodes.Count;
        if (n == 0)
        {
            yield return player == PlayerType.Attacker ? "[attack_skip]" : "[defense_skip]";
            yield break;
        }

        var max = 1 << n;

        for (int i = 0; i < max; i++)
        {
            var selectedNodes = new List<Node>();
            for (int j = 0; j < n; j++)
            {
                if ((i & (1 << j)) != 0)
                    selectedNodes.Add(actionNodes[j]);
            }

            var eventName = NameFormatter.GetCombinedEventName(player, selectedNodes);
            yield return $"[{eventName}]";
        }
    }

    private IEnumerable<string> GenerateActions(PlayerType player, List<Node> actionNodes) 
    {
        int n = actionNodes.Count;
        if (n == 0)
        {
            yield return player == PlayerType.Attacker
                ? $"{StaticGlobalVariableHolder.AttackerEndTurnEvent}"
                : $"{StaticGlobalVariableHolder.DefenderEndTurnEvent}";
            yield break;
        }

        foreach (var evt in NameFormatter.GetIndividualEventNames(player, actionNodes))
        {
            yield return evt;
        }
    }
    #endregion
}