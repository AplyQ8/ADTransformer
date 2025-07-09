using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrismCodeGenerator.Models;
using PrismCodeGenerator.Utils;

namespace PrismCodeGenerator.SectionGenerators;

public class PlayerModuleSectionGenerator: ICodeSectionGenerator
{
    public string Generate(IEnumerable<Node> nodes)
    {
        var allNodes = TreeWalker.Flatten(nodes);

        var attackers = allNodes.Where(n => n.IsAttackerNode).ToList();
        var defenders = allNodes.Where(n => n.IsDefenderNode).ToList();
        
        var sb = new StringBuilder();
        GenerateAttackerModule(ref sb, attackers);
        GenerateDefenderModule(ref sb, defenders);
        GenerateGameManagerModule(sb);
        return sb.ToString();
    }

    #region Attacker Module Generation

    private void GenerateAttackerModule(ref StringBuilder sb, List<Node> attackerEvents)
    {
        sb.AppendLine($"module {StaticGlobalVariableHolder.AttackerModuleName}");

        foreach (var transition in GenerateActions(PlayerType.Attacker, attackerEvents))
        {
            sb.AppendLine("    " + transition);
        }
        

        sb.AppendLine("endmodule");
    }

    #endregion
    
    #region Defender Module Generation

    private void GenerateDefenderModule(ref StringBuilder sb, List<Node> defenderEvents)
    {
        sb.AppendLine($"module {StaticGlobalVariableHolder.DefenderModuleName}");

        foreach (var transition in GenerateActions(PlayerType.Defender, defenderEvents))
        {
            sb.AppendLine("    " + transition);
        }

        //sb.AppendLine($"    {StaticGlobalVariableHolder.DefenderSkipEvent} {StaticGlobalVariableHolder.TurnVariable}=0 -> ({StaticGlobalVariableHolder.TurnVariable}'=1);");
        sb.AppendLine("endmodule");
    }

    #endregion
    
    #region Common Transition Generation

    private IEnumerable<string> GenerateActionTransitions(PlayerType player, List<Node> actionNodes)
    {
        int n = actionNodes.Count;
        var turnValue = player == PlayerType.Attacker ? 1 : 0;
        var nextTurn = player == PlayerType.Attacker ? 2 : 1;
        var budgetName = player == PlayerType.Attacker
            ? NameFormatter.GetAttackerBudgetName()
            : NameFormatter.GetDefenderBudgetName();

        if (n == 0)
        {
            yield return $"[{NameFormatter.GetCombinedEventName(player, new List<Node>())}] {StaticGlobalVariableHolder.TurnVariable}={turnValue} -> ({StaticGlobalVariableHolder.TurnVariable}'={nextTurn});";
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

            var guardConditions = new List<string>
            {
                $"{StaticGlobalVariableHolder.TurnVariable}={turnValue}"
            };
            var updates = new List<string>
            {
                $"{StaticGlobalVariableHolder.TurnVariable}'={nextTurn}"
            };

            int totalCost = 0;

            foreach (var node in selectedNodes)
            {
                guardConditions.Add($"{NameFormatter.GetVariableName(node)}=0");
                totalCost += node.Cost ?? 0;
                updates.Add($"{NameFormatter.GetVariableName(node)}'=1");
            }

            guardConditions.Add($"{budgetName}>={totalCost}");
            updates.Add($"{budgetName}'={budgetName}-{totalCost}");

            yield return $"[{eventName}] {string.Join(" & ", guardConditions)} -> ({string.Join(") & (", updates)});";
        }
    }
    
    private IEnumerable<string> GenerateActions(PlayerType player, List<Node> nodes)
    {
        string turnVar = StaticGlobalVariableHolder.TurnVariable;

        if (nodes.Count == 0)
        {
            var endTurnEvent = player == PlayerType.Attacker
                ? StaticGlobalVariableHolder.AttackerEndTurnEvent
                : StaticGlobalVariableHolder.DefenderEndTurnEvent;

            int currentTurn = player == PlayerType.Attacker ? 1 : 0;
            int nextTurn = player == PlayerType.Attacker ? 2 : 1;
            string doneVar = player == PlayerType.Attacker
                ? StaticGlobalVariableHolder.AttackerEndTurnVariable
                : StaticGlobalVariableHolder.DefenderEndTurnVariable;

            yield return $"{endTurnEvent} {turnVar}={currentTurn} -> ({turnVar}'={nextTurn}) & ({doneVar}'=true);";
            yield break;
        }

        foreach (var node in nodes)
        {
            var eventName = NameFormatter.GetEventName(player, node); // e.g., [attack_a1n1]
            int currentTurn = player == PlayerType.Attacker ? 1 : 0;
            string turnCheck = $"{turnVar}={currentTurn}";
            string nodeVar = $"{NameFormatter.GetVariableName(node)}";
            string budgetVar = player == PlayerType.Attacker ? "attacker_budget" : "defender_budget";

            yield return
                $"{eventName} {turnCheck} & {nodeVar}=0 & {budgetVar}>={node.Cost} -> " +
                $"({nodeVar}'=1) & ({budgetVar}'={budgetVar}-{node.Cost});";
        }

        var endEvent = player == PlayerType.Attacker
            ? StaticGlobalVariableHolder.AttackerEndTurnEvent
            : StaticGlobalVariableHolder.DefenderEndTurnEvent;

        int currentTurnFinal = player == PlayerType.Attacker ? 1 : 0;
        int nextTurnFinal = player == PlayerType.Attacker ? 2 : 1;
        string doneVarFinal = player == PlayerType.Attacker
            ? StaticGlobalVariableHolder.AttackerEndTurnVariable
            : StaticGlobalVariableHolder.DefenderEndTurnVariable;

        yield return $"{endEvent} {turnVar}={currentTurnFinal} -> ({turnVar}'={nextTurnFinal}) & ({doneVarFinal}'=true);";
    }
 

    #endregion
    

    #region Game Manager Module Generation

    private void GenerateGameManagerModule(StringBuilder sb)
    {
        sb.AppendLine($"module {StaticGlobalVariableHolder.GameManagerModuleName}");
        sb.AppendLine($"    {StaticGlobalVariableHolder.GameManagerCheckEvent} {StaticGlobalVariableHolder.TurnVariable}=2 -> (top_event'={FormulaTreeBuilder.Instance.FormulaNames.Last()}) & ({StaticGlobalVariableHolder.TurnVariable}'=3);");
        sb.AppendLine("endmodule");
    }

    #endregion
}