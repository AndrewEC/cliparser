namespace CliParsing.Parsers
{

    public class CollectionParserBuilder<T>
    {
        private readonly Func<IEnumerable<T>, object> func;
        public CollectionParserBuilder(Func<IEnumerable<T>, object> func)
        {
            this.func = func;
        }

        public Func<object?, string, object> SeparatedBy(string splitter) => CollectionParsers.Compose<T>(splitter, func);
        public Func<object?, string, object> CommaSeparated() => SeparatedBy(",");
    }

    public class ListParser
    {
        public static CollectionParserBuilder<T> Of<T>() => new CollectionParserBuilder<T>((enumerable) => enumerable.ToList());
    }

    public class ArrayParser
    {
        public static CollectionParserBuilder<T> Of<T>() => new CollectionParserBuilder<T>((enumerable) => enumerable.ToArray());
    }

    public static class CollectionParsers
    {
        public static Func<object?, string, object> Compose<T>(string splitter, Func<IEnumerable<T>, object> processEnumerable)
        {
            Type listItemType = typeof(T);
            var valueParser = DefaultParsers.DefaultParserFor(listItemType)
                ?? throw new ParseException($"Could not create parser to parse collection of [{listItemType.Name}]. No parser for type [{listItemType.Name}] could be found.");
            return (instance, value) => processEnumerable.Invoke(value.Split(splitter).Select(x => (T) valueParser.Invoke(null, x)));
        }
    }

}