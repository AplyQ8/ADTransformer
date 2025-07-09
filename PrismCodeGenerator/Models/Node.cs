using System.Collections.Generic;

namespace PrismCodeGenerator.Models
{
    public class Node
    {
        public string Id { get; set; }
        public string Label { get; set; }
        public GateType? GateType { get; set; }
        public PlayerType? Owner { get; set; }
        public int? Cost { get; set; }
        public List<Node> Children { get; set; } = new();
        public bool IsLeaf => Children.Count == 0;
        public bool IsActionNode { get; set; } = false;

        public bool IsAttackerNode => IsActionNode && Owner == PlayerType.Attacker;
        public bool IsDefenderNode => IsActionNode && Owner == PlayerType.Defender;
        
        public override bool Equals(object obj)
        {
            return obj is Node other && Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}