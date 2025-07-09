using System.Collections.Generic;
using System.Text;
using PrismCodeGenerator.Models;

namespace PrismCodeGenerator.SectionGenerators;

public class LabelsSectionGenerator: ICodeSectionGenerator
{
    public string Generate(IEnumerable<Node> nodes)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"label \"{StaticGlobalVariableHolder.TopEventLabelName}\" = top_event;");
        sb.AppendLine($"label \"{StaticGlobalVariableHolder.SystemSecureLabelName}\" = !top_event;");
        sb.AppendLine(
            $"label \"{StaticGlobalVariableHolder.GameEndLabelName}\" = ({StaticGlobalVariableHolder.TurnVariable} = 3);");
        
        return sb.ToString();
    }
}