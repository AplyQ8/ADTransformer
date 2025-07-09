using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PrismRunner;

public class PrismOutputParser
{
    /// <summary>
    /// Parses the PRISM output, extracting pairs of (AttackerCost, DefenderCost) values.
    /// Ignores pairs if one of the values is Infinity.
    /// </summary>
    /// <param name="output">PRISM text output</param>
    /// <returns>List of model results</returns>
    public static List<double> ParseModelResults(string output)
    {
        var results = new List<double>();

        // Ищем все строки вида "Result: <число>"
        var resultRegex = new Regex(@"Result:\s*(?<value>[+-]?([0-9]*[.])?[0-9]+|Infinity|infinity)", RegexOptions.IgnoreCase);
        var matches = resultRegex.Matches(output);

        foreach (Match match in matches)
        {
            string valStr = match.Groups["value"].Value;

            if (double.TryParse(valStr, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedVal))
            {
                results.Add(parsedVal);
            }
        }

        return results;
    }

    public static int ResultReturner(string output)
    {
        var resultRegex = new Regex(@"Result:\s*(?<value>[+-]?([0-9]*[.])?[0-9]+|Infinity|infinity)", RegexOptions.IgnoreCase);
        var match = resultRegex.Match(output);
        string valStr = match.Groups["value"].Value;
        if (int.TryParse(valStr, NumberStyles.Float, CultureInfo.InvariantCulture, out int parsedVal))
        {
            return parsedVal;
        }

        throw new NoAppropriateResult();
    }
    private static double ExtractAttackerCost(string input)
    {
        var match = Regex.Match(input, @"attacker(\d+)");
        if (match.Success && double.TryParse(match.Groups[1].Value, out double result))
        {
            return result;
        }
        throw new ArgumentException("Input string does not contain a valid attacker cost.");
    }

    private static double ExtractDefenderCost(string input)
    {
        var match = Regex.Match(input, @"defender(\d+)");
        if (match.Success && double.TryParse(match.Groups[1].Value, out double result))
        {
            return result;
        }
        throw new ArgumentException("Input string does not contain a valid defender cost.");
    }
}

