using System.Collections.Generic;
using System.Linq;
using System.Text;
using PrismCodeGenerator.Models;
using PrismCodeGenerator.Utils;

namespace PrismCodeGenerator.SectionGenerators;

public class GlobalsSectionGenerator: ICodeSectionGenerator
{
    public string Generate(IEnumerable<Node> nodes)
    {
        var allNodes = TreeWalker.Flatten(nodes);

        var attackers = allNodes.Where(n => n.IsAttackerNode);
        var defenders = allNodes.Where(n => n.IsDefenderNode);
        
        var sb = new StringBuilder();
        GenerateTurnVariable(ref sb);
        GenerateTopEventVariable(ref sb);
        GenerateAttackerVariables(ref sb, attackers);
        GenerateDefenderVariables(ref sb, defenders);
        GeneratePlayerDoneBooleans(ref sb);
        return sb.ToString();
    }
    
    #region Seperate Global Generators

    private void GenerateTurnVariable(ref StringBuilder sb)
    {
        sb.AppendLine("//Global variables");
        sb.AppendLine($"global {StaticGlobalVariableHolder.TurnVariable} : [0..3] init 0;");
    }

    private void GenerateTopEventVariable(ref StringBuilder sb)
    {
        sb.AppendLine($"global {StaticGlobalVariableHolder.TopEventVariable} : bool init false;");
    }

    private void GenerateAttackerVariables(ref StringBuilder sb, IEnumerable<Node> attackerEvents)
    {
        if (!attackerEvents.Any())
            return;
        sb.AppendLine("//Attacker variables");
        foreach (var attackerEvent in attackerEvents)
        {
            sb.AppendLine("global " + $"{NameFormatter.GetVariableName(attackerEvent)}: [0..1] init 0;");
        }
    }

    private void GenerateDefenderVariables(ref StringBuilder sb, IEnumerable<Node> defenderEvents)
    {
        if (!defenderEvents.Any())
            return;
        sb.AppendLine("//Defender variables");
        foreach (var defenderEvent in defenderEvents)
        {
            sb.AppendLine("global " + $"{NameFormatter.GetVariableName(defenderEvent)}: [0..1] init 0;");
        }
    }

    private void GeneratePlayerDoneBooleans(ref StringBuilder sb)
    {
        sb.AppendLine();
        sb.AppendLine($"global {StaticGlobalVariableHolder.AttackerEndTurnVariable} : bool init false;");
        sb.AppendLine($"global {StaticGlobalVariableHolder.DefenderEndTurnVariable} : bool init false;");
        sb.AppendLine();
    }

    #endregion
}