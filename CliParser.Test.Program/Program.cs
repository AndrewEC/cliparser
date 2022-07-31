namespace CliParsing.Test.Program
{
    using CliParsing.Commands;

    public class Program
    {
        public static void Main(string[] args)
        {
            Exception? error = CliProgram.Create(
                Command.Group(
                    Command.From<Add>("add", (args) => Console.WriteLine($"{args.First} + {args.Second} = {args.First + args.Second}")),
                    Command.Group(
                        "print",
                        Command.From<PersonName>("name", (args) => Console.WriteLine(args.Name)),
                        Command.From<PersonName>("greeting", (args) => Console.WriteLine($"Hello {args.Name}!!!")),
                        Command.Later<FirstNameCommand>()
                    )
                )
            ).TryExecute(args);

            if (error != null)
            {
                Console.WriteLine(error.Message);
            }
        }
    }

    internal sealed class FirstNameCommand : BaseCommand<PersonName>
    {
        public override void Execute(PersonName args)
        {
            if (args.Name.Contains(" "))
            {
                Console.WriteLine($"First name is: {args.Name.Split(" ")[0]}");
                return;
            }
            Console.WriteLine($"Provided name is: {args.Name}");
        }

        public override string GetName() => "firstname";
    }

    [ProgramDescriptor("Print the name of a person.")]
    internal sealed class PersonName
    {
        [Argument("name", position: 1, helpText: "The name of the person.")]
        public string Name { get; set; } = "";
    }

    [ProgramDescriptor("Add two number together.")]
    internal sealed class Add
    {
        [Argument("first", position: 1, helpText: "The first number to add.")]
        public int First = 0;

        [Argument("second", position: 2, helpText: "The second number to add.")]
        public int Second = 0;
    }
}