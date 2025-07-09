using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using PrismCodeGenerator.Models;

namespace PrismCodeGenerator.Utils;

public class NameFormatter
{
    public static string GetVariableName(Node node)
    {
        return $"{node.Label}_{node.Id}";
    }
    
    public static string GetCombinedEventName(PlayerType playerType, IEnumerable<Node> nodes)
    {
        if (!nodes.Any())
            return playerType == PlayerType.Attacker ? $"{StaticGlobalVariableHolder.AttackerEndTurnEvent}" : $"{StaticGlobalVariableHolder.DefenderEndTurnEvent}";

        var prefix = playerType == PlayerType.Attacker ? "attack" : "defense";
        
        var suffix = string.Concat(
            nodes
                .OrderBy(n => n.Label) // упорядочим по Label, а не только Id
                .ThenBy(n => n.Id)
                .Select(n => $"{n.Label}{n.Id}")
        );

        return $"[{prefix}_{suffix}]";
    }


    public static IEnumerable<string> GetIndividualEventNames(PlayerType playerType, IEnumerable<Node> nodes)
    {
        // Если узлов нет, вернуть только end_turn эвент
        if (!nodes.Any())
        {
            yield return playerType == PlayerType.Attacker
                ? $"{StaticGlobalVariableHolder.AttackerEndTurnEvent}"
                : $"{StaticGlobalVariableHolder.DefenderEndTurnEvent}";
            yield break;
        }

        var prefix = playerType == PlayerType.Attacker ? "attack" : "defense";

        foreach (var node in nodes.OrderBy(n => n.Label).ThenBy(n => n.Id))
        {
            yield return $"[{prefix}_{node.Label}{node.Id}]";
        }

        // Не забудем про завершение хода после всех узлов
        yield return playerType == PlayerType.Attacker
            ? $"{StaticGlobalVariableHolder.AttackerEndTurnEvent}"
            : $"{StaticGlobalVariableHolder.DefenderEndTurnEvent}";
    }
    
    public static string GetEventName(PlayerType player, Node node)
    {
        var prefix = player == PlayerType.Attacker ? "attack" : "defense";
        return $"[{prefix}_{node.Label}{node.Id}]";
    }

    public static string GetAttackerBudgetName()
    {
        return "attacker_budget";
    }

    public static string GetDefenderBudgetName()
    {
        return "defender_budget";
    }
}