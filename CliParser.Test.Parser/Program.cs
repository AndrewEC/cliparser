namespace CliParsing.Test.Parser
{
    using CliParsing.Parsers;

    public class Program
    {
        public static void Main(string[] args)
        {
            var intListParser = ParseList.Of<int>().CommaSeparated();
            var additionalParsers = AdditionalParsers.Builder()
                .ForFieldNamed(nameof(Person.Numbers), intListParser)
                .ForType(typeof(List<int>), intListParser)
                .Build();

            var parser = new CliParser();
            if (!parser.TryParseArgs(out Person person, args, additionalParsers))
            {
                Console.WriteLine(parser.LastError.Message);
                return;
            }
            Console.WriteLine(person.ToString());
        }
    }

    [ProgramDescriptor("Print out some random parsed information.")]
    internal sealed class Person
    {
        [Argument("name", position: 1, helpText: "A random person's name.")]
        public string Name = "";

        [Argument("number", position: 2, helpText: "A random number.")]
        public int Number = 0;

        [Argument("--position", shortName: "-p", helpText: "A random enum.")]
        public Position Position = Position.Unknown;

        [Argument("--names", shortName: "-na", helpText: "A comma delimited list of names.", parser: nameof(ParseReferences))]
        public List<string> Names = new List<string>();

        [Argument("--numbers", shortName: "-n", helpText: "A comma delimited list of numbers.")]
        public List<int> Numbers = new List<int>();

        public static object ParseReferences(object? target, string value) => ParseList.Of<string>().CommaSeparated().Invoke(target, value);

        public override string ToString() => $"Name: {Name}, Number: {Number}, Position: {Position}, Names: {string.Join(", ", Names)}, Numbers: {string.Join(", ", Numbers.Select(x => x.ToString()))}";
    }

    internal enum Position
    {
        First,
        Second,
        Third,
        Unknown
    }
}