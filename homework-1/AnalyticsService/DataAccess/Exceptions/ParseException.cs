﻿namespace DataAccess.Exceptions;

public class ParseException : Exception
{
    public ParseException(string message) : base(message)
    {
    }
}