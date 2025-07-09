#nullable enable
using System.Collections.Generic;

namespace ADTransformer;

public class AdtNode
{
    public string Label { get; set; }
    public NodeType Role { get; set; }
    public RefinementType Refinement { get; set; }
    public List<AdtNode> Children { get; set; } = new();
    public AdtNode? Parent { get; set; }
    public bool IsCountermeasure { get; set; } = false;
    public double? Price { get; set; } = null;
}