using System.Collections.Generic;
using PrismCodeGenerator.Models;

namespace PrismCodeGenerator.Utils;

public static class TreeWalker
{
    /// <summary>
    /// Recursively expands a graph of nodes into a flat list, avoiding duplicates.
    /// </summary>
    public static IEnumerable<Node> Flatten(Node root)
    {
        var visited = new HashSet<Node>();
        return FlattenInternal(root, visited);
    }
    
    /// <summary>
    /// Recursively flattens a list of graphs into a list, avoiding duplicates.
    /// </summary>
    public static IEnumerable<Node> Flatten(IEnumerable<Node> roots)
    {
        var visited = new HashSet<Node>();
        foreach (var root in roots)
        {
            foreach (var node in FlattenInternal(root, visited))
            {
                yield return node;
            }
        }
    }
    
    private static IEnumerable<Node> FlattenInternal(Node node, HashSet<Node> visited)
    {
        if (!visited.Add(node))
            yield break;

        yield return node;

        foreach (var child in node.Children)
        {
            foreach (var descendant in FlattenInternal(child, visited))
            {
                yield return descendant;
            }
        }
    }
}