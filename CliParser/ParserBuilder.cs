namespace CliParsing.Parsers
{

    public class ParseCollectionBuilder<T>
    {
        private readonly Func<IEnumerable<T>, object> func;
        public ParseCollectionBuilder(Func<IEnumerable<T>, object> func)
        {
            this.func = func;
        }

        public Func<object?, string, object> SeparatedBy(string splitter) => CollectionParsers.Compose<T>(splitter, func);
        public Func<object?, string, object> CommaSeparated() => SeparatedBy(",");
    }

    public class ParseList
    {
        public static ParseCollectionBuilder<T> Of<T>() => new ParseCollectionBuilder<T>((enumerable) => enumerable.ToList());
    }

    public class ParseArray
    {
        public static ParseCollectionBuilder<T> Of<T>() => new ParseCollectionBuilder<T>((enumerable) => enumerable.ToArray());
    }

    public static class CollectionParsers
    {
        public static Func<object?, string, object> Compose<T>(string splitter, Func<IEnumerable<T>, object> processEnumerable)
        {
            Type listItemType = typeof(T);
            var valueParser = DefaultParsers.DefaultParserFor(listItemType)
                ?? throw new ParseException($"Could not create parser to parse array of [{listItemType.Name}]. No parser for type [{listItemType.Name}] could be found.");
            return (instance, value) => processEnumerable.Invoke(value.Split(splitter).Select(x => (T) valueParser.Invoke(null, x)));
        }
    }

}