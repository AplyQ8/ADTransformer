using System;
using System.Collections.Generic;

namespace PrismRunner;

public class ParetoPointFinder
{
    public static int FindPoint(List<double> results, List<int> defenderSums)
    {
        var indicesOfOnes = GetIndicesOfOnes(results);
        
        try
        {
            var defenderCosts = GetNumbersByIndices(indicesOfOnes, ReverseList(defenderSums));
            return FindMinimum(defenderCosts);
        }
        catch (ArgumentException e)
        {
            
            throw new NoAppropriateResult();
        }
        
    }
    
    private static List<int> GetIndicesOfOnes(List<double> values)
    {
        var indices = new List<int>();

        for (int i = 0; i < values.Count; i++)
        {
            if (Math.Abs(values[i] - 1.0) < 1e-9) // сравнение с учетом погрешности
            {
                indices.Add(i);
            }
        }

        return indices;
    }

    private static List<int> GetNumbersByIndices(List<int> indices, List<int> defenderSums)
    {
        if (indices == null || indices.Count == 0)
            throw new ArgumentException("Empty or null");
        var numbers = new List<int>();

        foreach (var index in indices)
        {
            numbers.Add(defenderSums[index]);
        }

        return numbers;
    }
    
    private static int FindMinimum(List<int> values)
    {
        if (values == null || values.Count == 0)
            throw new ArgumentException("Empty or null");

        int min = values[0];
        foreach (int val in values)
        {
            if (val < min)
                min = val;
        }

        return min;
    }
    private static List<int> ReverseList(List<int> input)
    {
        var result = new List<int>(input);
        result.Reverse();
        return result;
    }
}