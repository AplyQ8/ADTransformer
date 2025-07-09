using System;

namespace PrismCodeGenerator.Utils;

public class NodeIsNotAnActionException: Exception
{
    public NodeIsNotAnActionException(){}
    
    public NodeIsNotAnActionException(string message)
        : base(message)
    {
    }
}