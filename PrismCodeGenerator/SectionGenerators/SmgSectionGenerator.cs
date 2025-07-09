using System.Collections.Generic;
using PrismCodeGenerator.Models;

namespace PrismCodeGenerator.SectionGenerators
{
    public class SmgSectionGenerator: ICodeSectionGenerator
    {
        public string Generate(IEnumerable<Node> nodes)
        {
            return "smg";
        }
    }
}