using System;

namespace CommandExecutor;

public class CommandExecutorException : Exception
{
    public CommandExecutorException() { }

    public CommandExecutorException(string message) : base(message) { }

    public CommandExecutorException(string message, Exception innerException)
        : base(message, innerException) { }
}