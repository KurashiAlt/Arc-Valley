using System.Text.RegularExpressions;

namespace Arc;
public static partial class Parser
{
    public readonly static Regex formatter = new("#.*|}|(\"[^\"]+\"|\\S+)\\s+=\\s+(\"[^\"]+\"|\\S+)|\"[^\"]+\"|\\S+", RegexOptions.Compiled);
    public readonly static Regex open = new("^({|\\[)$", RegexOptions.Compiled);
    public readonly static Regex close = new("^(}|\\])$", RegexOptions.Compiled);
    public readonly static Regex equal = new("^=$", RegexOptions.Compiled);
    public readonly static Regex inQuotes = new("^\"(.*)\"$", RegexOptions.Compiled);
    private readonly static Regex validVariableKey = new("^[:$a-zA-Z0-9_-]+$", RegexOptions.Compiled);
    public static bool IsVariableKey(string s)
    {
        return validVariableKey.IsMatch(s);
    }
    public static bool HasEnclosingBrackets(Block s, string open = "{", string close = "}")
    {
        if (s.Count < 2)
            return false;
        return s.ElementAt(0) == open && s.ElementAt(s.Count - 1) == close;
    }
}