using System;

namespace PrismRunner;

public class NoAppropriateResult : Exception
{
    public NoAppropriateResult()
    {
    }

    public NoAppropriateResult(string message)
        : base(message)
    {
    }

    public NoAppropriateResult(string message, Exception inner)
        : base(message, inner)
    {
    }
}