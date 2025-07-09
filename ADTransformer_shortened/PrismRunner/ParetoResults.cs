using System.Collections.Generic;

namespace PrismRunner;

public class ParetoResults
{
    public bool DefenderWon { get; private set; }
    public List<PrismOutputResult> Results { get; private set; }
    public ParetoResults(bool defenderWon, List<PrismOutputResult> results)
    {
        DefenderWon = defenderWon;
        Results = results;
    }
}