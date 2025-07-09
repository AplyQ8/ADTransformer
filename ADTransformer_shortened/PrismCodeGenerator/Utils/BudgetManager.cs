using System;
using System.Collections.Generic;
using System.Linq;
using PrismCodeGenerator.Models;

namespace PrismCodeGenerator.Utils;

public class BudgetManager
{
    private static BudgetManager? _instance;
    private static readonly object _lock = new();

    public int AttackerBudget { get; private set; }
    public int DefenderBudget { get; private set; }

    public int MaxAttackerBudget { get; private set; }
    public int MaxDefenderBudget { get; private set; }
    
    private BudgetManager() { }

    public static BudgetManager Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new BudgetManager();
            }
        }
    }
    
    public void Initialize(IEnumerable<Node> nodes)
    {
        if (nodes == null)
            throw new ArgumentNullException(nameof(nodes));

        var allNodes = TreeWalker.Flatten(nodes);

        MaxAttackerBudget = allNodes
            .Where(n => n.Owner == PlayerType.Attacker && n.Cost.HasValue)
            .Sum(n => n.Cost!.Value);

        MaxDefenderBudget = allNodes
            .Where(n => n.Owner == PlayerType.Defender && n.Cost.HasValue)
            .Sum(n => n.Cost!.Value);

        // По умолчанию — максимальные значения
        AttackerBudget = MaxAttackerBudget;
        DefenderBudget = MaxDefenderBudget;
    }

    public void SetAttackerBudget(int value)
    {
        if (value < 0 || value > MaxAttackerBudget)
            throw new ArgumentOutOfRangeException(nameof(value), $"Attacker budget must be between 0 and {MaxAttackerBudget}");
        AttackerBudget = value;
    }

    public void SetDefenderBudget(int value)
    {
        if (value < 0 || value > MaxDefenderBudget)
            throw new ArgumentOutOfRangeException(nameof(value), $"Defender budget must be between 0 and {MaxDefenderBudget}");
        DefenderBudget = value;
    }
}