namespace CliParsing
{
    using System;

    public class ParseException : Exception
    {
        public ParseException(string message) : base(message) { }
        public ParseException(string message, Exception cause) : base(message, cause) { }
    }
}