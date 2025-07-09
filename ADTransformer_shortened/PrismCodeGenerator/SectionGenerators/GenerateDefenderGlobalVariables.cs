using System;
using System.Collections.Generic;
using System.Text;
using PrismCodeGenerator.Models;
using PrismCodeGenerator.Utils;

namespace PrismCodeGenerator.SectionGenerators;

public class GenerateDefenderGlobalVariables
{
    public static string Generate(DefenderNodeCombination combination)
    {
        var sb = new StringBuilder();
        sb.AppendLine("//Defender variables");
        foreach (var variableAssignment in combination.VariableAssignments)
        {
            sb.AppendLine("global " + $"{variableAssignment.Key}: [0..1] init {variableAssignment.Value};");
        }
        return sb.ToString();
    }
}