﻿using System;

namespace ADTransformer;

public class ParserException : Exception
{
    public ParserException() { }

    public ParserException(string message) : base(message) { }

    public ParserException(string message, Exception innerException)
        : base(message, innerException) { }
}