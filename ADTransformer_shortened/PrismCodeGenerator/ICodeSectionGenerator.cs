using System.Collections.Generic;
using PrismCodeGenerator.Models;

namespace PrismCodeGenerator
{
    public interface ICodeSectionGenerator
    {
        string Generate(IEnumerable<Node> nodes);
    }
}