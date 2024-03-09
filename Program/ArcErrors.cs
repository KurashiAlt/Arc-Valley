using Arc;
public class ArcException : Exception
{
    public static string CreateMessage(params object[] args)
    {
        List<string> exp = new()
        {
            ""
        };
        foreach (object arg in args)
        {
            if (arg is Walker walker) LogWord(walker.Current);
            else if (arg is Word word) LogWord(word);
            else if (arg is Args nArgs)
            {
                LogBlock(nArgs.block);

                if (nArgs.keyValuePairs != null)
                {
                    foreach (KeyValuePair<string, Block> nArg in nArgs.keyValuePairs)
                    {
                        exp.Add($"Key = {nArg.Key}");
                        LogBlock(nArg.Value);
                    }
                }
            }
            else
            {
                exp.Add(arg.ToString());
            }
        }
        exp.Add("");
        return string.Join('\n', exp);

        void LogBlock(Block block)
        {
            LinkedListNode<Word>? first = block.First;
            if (first != null)
            {
                exp.Add($"starting at line {first.Value.Line} in file {first.Value.GetFile()} found '{block}'");
            }
        }
        void LogWord(Word word)
        {
            exp.Add($"at line {word.Line} in file {word.GetFile()} found '{word.Value}'");
        }
    }
    public static ArcException Create(params object[] args)
    {
        return new ArcException(CreateMessage(args));
    }
    private ArcException(string s) : base(s) { }
}