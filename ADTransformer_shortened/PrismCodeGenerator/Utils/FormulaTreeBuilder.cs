using System;
using System.Collections.Generic;
using System.Linq;
using PrismCodeGenerator.Models;

namespace PrismCodeGenerator.Utils;

public class FormulaTreeBuilder
{
    private static FormulaTreeBuilder? _instance;
    private static readonly object _lock = new();

    private readonly Dictionary<string, string> _formulas = new();
    private readonly HashSet<string> _visited = new();

    public IEnumerable<string> FormulaTree { get; private set; } = new List<string>();
    public HashSet<string> FormulaNames { get; } = new();

    public FormulaTreeBuilder() { }

    public static FormulaTreeBuilder Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new FormulaTreeBuilder();
            }
        }
    }

    public void Build(IEnumerable<Node> roots)
    {
        _formulas.Clear();
        _visited.Clear();
        FormulaNames.Clear();

        foreach (var root in roots)
            GenerateFormulaRecursive(root);

        FormulaTree = _formulas.Values;
    }

    private string GenerateFormulaRecursive(Node node)
    {
        if (_visited.Contains(node.Id))
            return GetReference(node);

        _visited.Add(node.Id);

        string formula;

        if (node.IsLeaf)
        {
            formula = $"{NameFormatter.GetVariableName(node)}=1";
        }
        else if (node.GateType == GateType.Cm)
        {
            var wrapped = node.Children.First(); // первый — оборачиваемый узел
            var cms = node.Children.Skip(1).ToList(); // остальные — контрмеры

            if (wrapped.GateType == GateType.Cm)
            {
                string wrappedFormula = GenerateFormulaRecursive(wrapped);
                string effectiveName = $"{NameFormatter.GetVariableName(node)}_effective";
                formula = wrappedFormula;
                _formulas[effectiveName] = $"formula {effectiveName} = ({formula});";
                FormulaNames.Add(effectiveName);
                return effectiveName;
            }
            else
            {
                string wrappedRef = GenerateFormulaRecursive(wrapped);
                var cmRefs = cms.Select(cm =>
                {
                    string cmEff = GenerateFormulaRecursive(cm);
                    string cmName = $"{cm.Id}_effective";
                    _formulas[cmName] = $"formula {cmName} = ({cmEff});";
                    FormulaNames.Add(cmName);
                    return $"!{cmName}";
                });

                string combined = $"({wrappedRef} & {string.Join(" & ", cmRefs)})";
                string formulaName = $"{NameFormatter.GetVariableName(node)}_triggered";
                _formulas[formulaName] = $"formula {formulaName} = {combined}; //<----{node.Label}";
                FormulaNames.Add(formulaName);
                return formulaName;
            }
        }
        else if (node.GateType == GateType.And || node.GateType == GateType.Or)
        {
            var childFormulas = node.Children.Select(GenerateFormulaRecursive);
            var op = node.GateType == GateType.And ? " & " : " | ";
            formula = $"({string.Join(op, childFormulas)})";

            string formulaName = $"{NameFormatter.GetVariableName(node)}_triggered";
            _formulas[formulaName] = $"formula {formulaName} = {formula}; //<----{node.Label}";
            FormulaNames.Add(formulaName);
            return formulaName;
        }
        else // fallback (например, одиночный базовый не IsLeaf, но почему-то с детьми)
        {
            formula = $"{NameFormatter.GetVariableName(node)}=1";
        }

        return formula;
    }

    private string GetReference(Node node)
    {
        if (node.GateType != GateType.Cm)
        {
            return node.GateType is GateType.And or GateType.Or
                ? $"{NameFormatter.GetVariableName(node)}_triggered"
                : $"{NameFormatter.GetVariableName(node)}=1";
        }

        var wrapped = node.Children.First();
        return wrapped.GateType == GateType.Cm
            ? $"{NameFormatter.GetVariableName(node)}_effective"
            : $"{NameFormatter.GetVariableName(node)}_triggered";
    }
}


public class FormulaNode
{
    public string Id { get; set; }
    public string Label { get; set; }
    public string Formula { get; set; }
    public List<FormulaNode> Children { get; } = new();
}