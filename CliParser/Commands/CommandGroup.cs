namespace CliParsing.Commands
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    public abstract class BaseCommandGroup : ICommand
    {
        public abstract ICommand[] SubCommands();

        public void Execute(CliProgram program, string[] args)
        {
            ICommand[] subCommands = SubCommands();
            ValidateCommandNames(subCommands);

            if (args.Length == 0)
            {
                string expectedList = FormExpectedCommandNameList(subCommands);
                throw new CommandException($"No command found. Expected to one of: [{expectedList}]");
            }

            string nextCommandName = args[0].ToLowerInvariant();
            if (nextCommandName == "-h" || nextCommandName == "--help")
            {
                string expectedList = FormExpectedCommandNameList(subCommands);
                Console.WriteLine($"Specify one of the following subcommands to execute: [{expectedList}]");
                System.Environment.Exit(0);
            }

            string[] nextArgs = args.Skip(1).ToArray();
            ICommand? nextCommand = subCommands.Where(command => command.GetName().ToLowerInvariant() == nextCommandName).FirstOrDefault();
            if (nextCommand == null)
            {
                string expectedList = FormExpectedCommandNameList(subCommands);
                throw new CommandException($"Could not find command with name [{nextCommandName}] to execute. Expected command to be one of: [{expectedList}]");
            }
            nextCommand.Execute(program, nextArgs);
        }

        private void ValidateCommandNames(ICommand[] commands)
        {
            List<string> names = new List<string>();
            foreach (ICommand command in commands)
            {
                string name = command.GetName();
                if (names.Contains(name))
                {
                    throw new CommandException($"Two or more commands attempted to register with the same name. Found at least two commands with the name: [{name}]");
                }
                names.Add(name);
            }
        }

        private string FormExpectedCommandNameList(ICommand[] subCommands) => string.Join(", ", subCommands.Select(command => command.GetName().ToLowerInvariant()));

        public abstract string GetName();
    }

    public class GenericCommandGroup : BaseCommandGroup
    {
        private readonly ImmutableArray<ICommand> commands;
        private readonly string name;

        public GenericCommandGroup(string? name, ImmutableArray<ICommand> commands)
        {
            this.commands = commands;
            this.name = name ?? GetType().Name.ToLowerInvariant();
        }    

        public override ICommand[] SubCommands() => commands.ToArray();

        public override string GetName() => name;
    }

    public static partial class Command
    {
        public static ICommand Group(params ICommand[] commands) => new GenericCommandGroup(null, commands.ToImmutableArray());
        public static ICommand Group(string name, params ICommand[] commands) => new GenericCommandGroup(name, commands.ToImmutableArray());
    }
}