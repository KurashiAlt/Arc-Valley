using Arc;
public class ArcException : Exception
{
    public static string CreateMessage(params object[] args)
    {
        List<string> exp = new();
        foreach (object arg in args)
        {
            if (arg is Walker walker) exp.Add($"at line {walker.Current.Line} in file {walker.Current.GetFile()} found '{walker.Current.Value}'");
            else if (arg is Word word) exp.Add($"at line {word.Line} in file {word.GetFile()} found '{word.Value}'");
            else
            {
                exp.Add(arg.ToString());
            }
        }
        return string.Join('\n', exp);
    }
    public static ArcException Create(params object[] args)
    {
        return new ArcException(CreateMessage(args));
    }
    private ArcException(string s) : base(s) { }
}