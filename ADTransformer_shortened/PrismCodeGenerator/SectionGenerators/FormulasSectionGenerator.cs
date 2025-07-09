using System;
using System.Collections.Generic;
using PrismCodeGenerator.Models;
using PrismCodeGenerator.Utils;

namespace PrismCodeGenerator.SectionGenerators;

public class FormulasSectionGenerator : ICodeSectionGenerator
{
    public string Generate(IEnumerable<Node> nodes)
    {
        var formulas = FormulaTreeBuilder.Instance.FormulaTree;
        return string.Join(Environment.NewLine, formulas);
    }
    
}