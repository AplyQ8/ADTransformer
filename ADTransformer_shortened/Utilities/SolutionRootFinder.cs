using System;
using System.IO;
using System.Linq;

namespace Utilities;

public class SolutionRootFinder
{
    private static SolutionRootFinder? _instance;
    private static readonly object _lock = new();
    
    public static SolutionRootFinder Instance
    {
        get
        {
            lock (_lock)
            {
                return _instance ??= new SolutionRootFinder();
            }
        }
    }

    public string GetSolutionRoot()
    {
        var dir = AppContext.BaseDirectory;

        while (dir != null && !Directory.GetFiles(dir, "*.sln").Any())
        {
            dir = Directory.GetParent(dir)?.FullName;
        }

        if (dir == null)
        {
            Console.WriteLine("[Warning] Solution root not found. Using AppContext.BaseDirectory instead.");
            dir = AppContext.BaseDirectory;
        }

        return dir;
    }

}