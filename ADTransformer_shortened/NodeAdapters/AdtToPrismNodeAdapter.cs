using System;
using System.Collections.Generic;
using System.Linq;
using ADTransformer; 
using PrismCodeGenerator.Models; 

namespace NodeAdapters;

public class AdtToPrismNodeAdapter
{
    private int _idCounter = 0;
    private Dictionary<AdtNode, Node> _convertedNodes = new();
    private Dictionary<string, Node> _labelToNode = new();

    public Node Convert(AdtNode root)
    {
        var result = ConvertRecursive(root, null);
        return result;
    }
    private Node ConvertRecursive(AdtNode adtNode, AdtNode? parent)
    {
        // Если уже существует узел с таким же лейблом — возвращаем его
        if (_labelToNode.TryGetValue(adtNode.Label, out var existingNode))
        {
            _convertedNodes[adtNode] = existingNode; // связываем текущий AdtNode с найденным Node
            return existingNode;
        }

        var owner = MapRoleToPlayer(adtNode);
        var gateType = MapRefinementToGateType(adtNode);

        var node = new Node
        {
            Id = $"n{_idCounter++}",
            Label = adtNode.Label,
            GateType = gateType,
            Owner = owner,
            Cost = adtNode.Price.HasValue ? (int)Math.Round(adtNode.Price.Value) : (int?)null
        };

        // Сохраняем в оба словаря до рекурсивных вызовов — на случай циклов или повторных ссылок
        _convertedNodes[adtNode] = node;
        _labelToNode[adtNode.Label] = node;

        // Обрабатываем дочерние узлы, кроме контрмер
        foreach (var child in adtNode.Children)
        {
            if (child.IsCountermeasure)
                continue;

            var childNode = ConvertRecursive(child, adtNode);
            node.Children.Add(childNode);
        }

        node.IsActionNode = node.GateType == GateType.Basic;

        // Обработка контрмер
        var countermeasures = adtNode.Children.Where(c => c.IsCountermeasure).ToList();
        if (countermeasures.Any())
        {
            var cmNode = new Node
            {
                Id = $"n{_idCounter++}",
                Label = $"cm_{adtNode.Label}",
                GateType = GateType.Cm,
                Owner = PlayerType.Defender,
                IsActionNode = false
            };

            cmNode.Children.Add(node); // сам node — первый потомок

            foreach (var cm in countermeasures)
            {
                var cmChildNode = ConvertRecursive(cm, adtNode);
                cmNode.Children.Add(cmChildNode);
            }

            return cmNode;
        }

        return node;
    }



    private GateType? MapRefinementToGateType(AdtNode adtNode)
    {
        // Игнорируем countermeasure-узлы
        var nonCmChildren = adtNode.Children.Where(c => !c.IsCountermeasure).ToList();

        if (nonCmChildren.Count == 0)
            return GateType.Basic;

        return adtNode.Refinement switch
        {
            RefinementType.Conjunctive => GateType.And,
            RefinementType.Disjunctive => GateType.Or,
            _ => null
        };
    }

    private PlayerType? MapRoleToPlayer(AdtNode adtNode)
    {
        return adtNode.Role switch
        {
            NodeType.Attacker => PlayerType.Attacker,
            NodeType.Defender => PlayerType.Defender,
            _ => null
        };
    }
}