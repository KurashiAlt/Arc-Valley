using System.Text;

namespace Arc;
public partial class Compiler
{
    public static Walker GetScope(Walker i, out Block scope)
    {
        scope = i.GetScope();

        return i;
    }
    public static bool TryTrimOne(string value, char s, out string? newValue)
    {
        newValue = null;
        if (value == null)
            return false;
        if (value.Length < 2)
            return false;
        if (value[0] != s)
            return false;
        if (value[^1] != s)
            return false;
        newValue = value[1..^1];
        return true;
    }
    public static Block RemoveEnclosingBrackets(Block scope)
    {
        scope.RemoveFirst();
        scope.RemoveLast();
        return scope;
    }
    public static bool TryGetVariable(Word locator, out IVariable? var)
    {
        return TryGetVariable(locator, out var, new Func<string, IVariable?>(global.Get), global.CanGet);
    }
    public static string[] GetSubparts(string str)
    {
        List<string> retval = new();
        if (string.IsNullOrWhiteSpace(str)) return retval.ToArray();
        int ndx = 0;
        StringBuilder s = new();
        bool insideDoubleQuote = false;
        bool insideSpecialQuote = false;
        int circleIndent = 0;
        int squareIndent = 0;

        while (ndx < str.Length)
        {
            if (!insideDoubleQuote && !insideSpecialQuote && circleIndent == 0 && squareIndent == 0)
            {
                if (str[ndx] == '{' || str[ndx] == '}')
                {
                    string a = s.ToString();
                    if (!string.IsNullOrWhiteSpace(a)) retval.Add(a);
                    s.Clear();
                    s.Append(str[ndx]);
                    a = s.ToString();
                    if (!string.IsNullOrWhiteSpace(a)) retval.Add(a);
                    s.Clear();
                    incr();
                    continue;
                }
                else if (str[ndx] == ':')
                {
                    string a = s.ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(a)) retval.Add(a);
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

            s.Append(str[ndx]);
            incr();
            void incr()
            {
                ndx++;
            }
        }
        string v = s.ToString();
        if (!string.IsNullOrWhiteSpace(v)) retval.Add(v);
        return retval.ToArray();
    }
    public static bool TryGetVariable(string locator, out IVariable? var, Func<string, IVariable?> Get, Func<string, bool> CanGet)
    {
        if (locator.StartsWith("trigger_value:", "event_target:", "modifier:") || locator.Contains(' ') || locator.EnclosedBy('`'))
        {
            var = null;
            return false;
        }

        if (locator.Contains(':'))
        {
            string[] KeyLocator = GetSubparts(locator);
            int f = 0;
            string currentKey;
            do
            {
                currentKey = GetId(KeyLocator[f]);
                if (KeyLocator.Length > f + 1)
                {
                    try
                    {
                        IVariable? v = Get(currentKey) ?? throw ArcException.Create(currentKey, locator, Get, CanGet);
                        if (v is IArcObject n)
                        {
                            Get = n.Get;
                            CanGet = n.CanGet;
                        }
                    }
                    catch
                    {
                        throw ArcException.Create(currentKey, locator, Get, CanGet);
                    }
                }
                else
                {
                    if (CanGet(currentKey))
                    {
                        var = Get(currentKey);
                        return true;
                    }
                    else
                    {
                        var = null;
                        return false;
                    }
                }
                f++;
            } while (KeyLocator.Length > f);
            if (!CanGet(currentKey))
            {
                var = null;
                return false;
            }
            var = Get(currentKey);
            return true;
        }
        else
        {
            if (!CanGet(locator))
            {
                var = null;
                return false;
            }
            var = Get(locator);
            return true;
        }
    }
}