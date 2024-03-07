using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Arc;
public partial class Compiler
{
    public static Walker GetScope(Walker i, out Block scope)
    {
        scope = new();

        int indent = 0;
        do
        {
            if (Parser.open.IsMatch(i.Current))
                indent++;
            if (Parser.close.IsMatch(i.Current))
                indent--;
            scope.AddLast(i.Current);
            if (indent > 0)
                i.MoveNext();
        } while (indent > 0);
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
    public static bool TryGetVariable(string locator, out IVariable? var)
    {
        return TryGetVariable(locator, out var, new Func<string, IVariable>((string indexer) => global[indexer]), global.CanGet);
    }
    public static bool TryGetVariable(string locator, out IVariable? var, Func<string, IVariable> Get, Func<string, bool> CanGet)
    {
        if (locator.StartsWith("trigger_value", "event_target") || locator.Contains(' ') || locator.EnclosedBy('`'))
        {
            var = null;
            return false;
        }

        if (locator.Contains(':'))
        {
            string[] KeyLocator = locator.Split(':');
            int f = 0;
            string currentKey;
            do
            {
                currentKey = KeyLocator[f];
                if (KeyLocator.Length > f + 1)
                {
                    IVariable v = Get(currentKey);
                    if (v is IArcObject)
                    {
                        IArcObject n = (IArcObject)v;
                        Get = n.Get;
                        CanGet = n.CanGet;
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