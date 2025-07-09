using System.Collections.Generic;
using System.Linq;
using PrismCodeGenerator.Models;
using PrismCodeGenerator.SectionGenerators;
using PrismCodeGenerator.Utils;

namespace PrismCodeGenerator;

public class PrismCodeComposer
{
    private List<ICodeSectionGenerator> _generators;

    public PrismCodeComposer()
    { }

    public string Compose(IEnumerable<Node> nodes)
    {
        InitializeGenerators(nodes);
        return string.Join("\n\n", _generators.Select(g => g.Generate(nodes)));
    }

    public Dictionary<string, string> ComposeWithDifferentBudget(IEnumerable<Node> nodes)
    {
        InitializeGenerators(nodes);

        var nonBudgetGenerators = _generators
            .Where(g => g is not BudgetSectionGenerator)
            .ToList();

        string commonCode = string.Join("\n\n", nonBudgetGenerators.Select(g => g.Generate(nodes)));

        var result = new Dictionary<string, string>();

        var defenderVariableCombinations = DefenderCombinationGenerator.GenerateDefenderCombinations(nodes);

        int maxAttackerBudget = BudgetManager.Instance.MaxAttackerBudget;
        int maxDefenderBudget = BudgetManager.Instance.MaxDefenderBudget;

        var attackerSums = AttackerCostSummarizer.AllSubsetSums(nodes);

        foreach (var attackerSum in attackerSums)
        {
            var budgetGenerator = new BudgetSectionGenerator(attackerSum, maxDefenderBudget);
            string budgetCode = budgetGenerator.Generate(nodes);
            foreach (var combination in defenderVariableCombinations)
            {
                string fullCode = $"{commonCode}\n\n{GenerateDefenderGlobalVariables.Generate(combination)}\n\n{budgetCode}";
                string key = $"attacker{attackerSum}_defender{combination.TotalCost}";
                result[key] = fullCode;
            }
        }

        return result;
    }


    private void InitializeGenerators(IEnumerable<Node> nodes)
    {
        BudgetManager.Instance.Initialize(nodes);
        FormulaTreeBuilder.Instance.Build(nodes);
        _generators = new List<ICodeSectionGenerator>
        {
            new SmgSectionGenerator(),
            new PlayersSectionGenerator(),
            new GlobalsSectionGenerator(),
            new BudgetSectionGenerator(BudgetManager.Instance.AttackerBudget, BudgetManager.Instance.DefenderBudget),
            new FormulasSectionGenerator(),
            new PlayerModuleSectionGenerator(),
            new RewardsSectionGenerator(),
            new LabelsSectionGenerator()
        };
    }
}