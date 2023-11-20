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
    public static Walker GetValue(Walker i, out Block value)
    {
        value = new();
        if (i.Current.value.EndsWith("[]"))
        {
            value.AddLast(i.Current);
            i.MoveNext();
            i = GetScope(i, out Block v);
            value.Add(v);
        }
        else if (!Parser.open.IsMatch(i.Current))
        {
            value.AddLast(i.Current);
        }
        else
        {
            i = GetScope(i, out Block scope);
            value = scope;
        }
        return i;
    }
    public static Walker TryGetKeyValue(Walker i, out string key, out Block? value, out bool Copy)
    {
        key = i.Current;

        i.MoveNext();

        Copy = (i.Current == "~");
        if (!Parser.equal.IsMatch(i.Current) && i.Current != "~")
        {
            i.MoveBack();
            value = null;
            return i;
        }

        i.MoveNext();

        i = GetValue(i, out value);

        return i;
    }
    public static Walker GetKeyValue(Walker i, out string key, out Block? value, out bool Copy)
    {
        i = TryGetKeyValue(i, out key, out value, out Copy);
        if (value == null)
            throw new Exception();
        return i;
    }
    public static string StringListSum(List<string> list)
    {
        return string.Join(" ", list);
    }

    //	public (Dictionary<string, IValue> dict, string key) GetNewVariable(string locator)
    //	{
    //		return GetNewVariable(variables, locator);
    //	}
    //	public static (Dictionary<string, IValue> dict, string key) GetNewVariable(Dictionary<string, IValue> vars, string locator)
    //	{
    //		if (locator.Contains(':'))
    //		{
    //			string[] KeyLocator = locator.Split(':');
    //			int f = 0;
    //			Dictionary<string, IValue> currentDict = vars;
    //			string currentKey;
    //			do
    //			{
    //				currentKey = KeyLocator[f];
    //				if (currentDict.ContainsKey(currentKey))
    //				{
    //					if (currentDict[currentKey].IsObject())
    //					{
    //						currentDict = currentDict[currentKey].AsObject().Properties;
    //					}
    //					else
    //					{
    //						if (KeyLocator.Length > f + 1)
    //							throw new Exception($"Tried assigning to a key in a nonobjectKey");
    //						else
    //						{
    //							f++;
    //							continue;
    //						}
    //					}
    //				}
    //				else if(currentKey == "global")
    //				{
    //					currentDict = global.Properties;
    //				}
    //				f++;
    //			} while (KeyLocator.Length > f);
    //		
    //			if (!currentDict.ContainsKey(currentKey))
    //#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    //				currentDict.Add(currentKey, null);
    //#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference ty e.
    //			return (currentDict, currentKey);
    //		}
    //		else
    //		{
    //			if (vars.ContainsKey(locator))
    //				throw new Exception("Trying to assign with datatype to existing variable");
    //#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    //			vars.Add(locator, null);
    //#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
    //			return (vars, locator);
    //		}
    //	}
    public static bool TryGetVariable(string locator, out IVariable? var)
    {
        return TryGetVariable(locator, out var, new Func<string, IVariable>((string indexer) => global[indexer]), global.CanGet);
    }
    public static bool TryGetVariable(string locator, out IVariable? var, Func<string, IVariable> Get, Func<string, bool> CanGet)
    {
        if (locator.StartsWith("trigger_value", "event_target") || locator.Contains(' '))
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