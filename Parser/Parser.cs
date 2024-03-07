using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Arc;

public static partial class Parser
{
    public static string Preprocessor(string input)
    {
        List<(string OldValue, string NewValue)> replaces = new();

        Regex Replace = PreprocessorReplace();
        input = Replace.Replace(input, delegate (Match m)
        {
            replaces.Add((m.Groups[1].Value, m.Groups[2].Value));
            return "";
        });

        foreach ((string OldValue, string NewValue) in replaces)
        {
            input = input.Replace(OldValue, NewValue);
        }

        return input;
    }
    public static Block ParseFile(string filename)
    {
        return ParseCode(File.ReadAllText(filename), filename);
    }
    public static Block ParseCode(string str, string fileName)
    {
        int line = 1;
        str = CommentRemover().Replace(str, "");
        str = str.Replace("\r\n", "\n");

        Block retval = new();
        if (string.IsNullOrWhiteSpace(str)) return retval;
        int ndx = 0;
        StringBuilder s = new();
        bool insideDoubleQuote = false;
        bool insideSpecialQuote = false;
        int circleIndent = 0;
        int squareIndent = 0;

        while (ndx < str.Length)
        {
            if (str[ndx] == '\n') line++;
            if (!insideDoubleQuote && !insideSpecialQuote && circleIndent == 0 && squareIndent == 0)
            {
                if(str[ndx] == '{' || str[ndx] == '}')
                {
                    string a = s.ToString();
                    if (!string.IsNullOrWhiteSpace(a)) retval.AddLast(new Word(a, line, fileName));
                    s.Clear();
                    s.Append(str[ndx]);
                    a = s.ToString();
                    if (!string.IsNullOrWhiteSpace(a)) retval.AddLast(new Word(a, line, fileName));
                    s.Clear();
                    incr();
                    continue;
                }
                else if (str[ndx] == '=')
                {
                    string a = s.ToString();
                    if (a == "+" || a == "-" || a == ":") goto end;
                    if (!string.IsNullOrWhiteSpace(a)) retval.AddLast(new Word(a, line, fileName));
                    s.Clear();
                }
                else if (char.IsWhiteSpace(str[ndx]))
                {
                    string a = s.ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(a)) retval.AddLast(new Word(a, line, fileName));
                    s.Clear();
                    incr();
                    continue;
                }
            }
            if (str[ndx] == '`') insideSpecialQuote = !insideSpecialQuote;
            if (str[ndx] == '"') insideDoubleQuote = !insideDoubleQuote;
            if (str[ndx] == '(') circleIndent++;
            if (str[ndx] == '[') squareIndent++;
            if (str[ndx] == ')') circleIndent--;
            if (str[ndx] == ']') squareIndent--;
            end:
            s.Append(str[ndx]);
            incr();
            void incr()
            {
                ndx++;
            }
        }
        string v = s.ToString();
        if (!string.IsNullOrWhiteSpace(v)) retval.AddLast(new Word(v, line, fileName));
        return retval;
    }
    public static string FormatCode(string str)
    {
        if (str == null)
            return "";
        MatchCollection matches = formatter.Matches(str);

        List<string> vs = matches.Select(m => m.Value).ToList();
        int indent = 0;
        for (int i = 0; i < vs.Count; i++)
        {
            if (vs[i].StartsWith("#"))
            {
                vs[i] = vs[i].Prepend(indent, '\t');
            }
            else if (vs[i].EndsWith('{'))
            {
                vs[i] = vs[i].Prepend(indent, '\t');
                indent++;
            }
            else if (vs[i].EndsWith('}'))
            {
                indent--;
                if (indent < 0)
                    throw new Exception();
                vs[i] = vs[i].Prepend(indent, '\t');
            }
            else
            {
                vs[i] = vs[i].Prepend(indent, '\t');
            }
        }

        return string.Join(Environment.NewLine, vs);
    }

    [GeneratedRegex("/replace (\\S+) with (\\S+)", RegexOptions.Compiled)]
    private static partial Regex PreprocessorReplace();
    [GeneratedRegex("#.*")]
    private static partial Regex CommentRemover();
}