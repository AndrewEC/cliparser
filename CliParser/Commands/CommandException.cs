namespace CliParsing.Commands
{
    public sealed class CommandException : Exception
    {
        public CommandException(string message) : base(message) { }
    }
}