namespace PrismRunner;

public readonly struct PrismOutputResult
{
    public double AttackerCost { get; }
    public double DefenderCost { get; }

    public PrismOutputResult(double attackerCost, double defenderCost)
    {
        AttackerCost = attackerCost;
        DefenderCost = defenderCost;
    }

    public override string ToString()
    {
        return $"AttackerCost = {AttackerCost}, DefenderCost = {DefenderCost}";
    }
}