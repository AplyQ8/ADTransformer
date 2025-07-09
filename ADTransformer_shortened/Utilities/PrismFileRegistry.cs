using System.Collections.Generic;

namespace Utilities;

public static class PrismFileRegistry
{
    /// <summary>
    /// Stores the mapping from budget identifier to full path of .prism file.
    /// </summary>
    public static Dictionary<string, string> FilePathsByBudget { get; } = new();
}