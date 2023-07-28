namespace Arc
{
    public enum LogicalScope
    {
        AND,
        OR,
        NOT
    }
    public partial class Compiler
    {
        //public bool Logic(Block code, Defines? def = null, LogicalScope scope = LogicalScope.AND)
        //{
        //	if (def == null)
        //		def = Defines.Global;
        //
        //	if(code.Count == 0) 
        //		return true;
        //
        //	bool current = false;
        //	if (scope == LogicalScope.AND)
        //		current = true;
        //
        //	Dictionary<string, Func<Walker, Walker>> keywords = new()
        //	{
        //		{ "AND", (Walker g) => NewLogic(LogicalScope.AND, g) },
        //		{ "OR" , (Walker g) => NewLogic(LogicalScope.OR , g) },
        //		{ "NOT", (Walker g) => NewLogic(LogicalScope.NOT, g) },
        //		{ "var_exists", var_exists },
        //	};
        //
        //	Walker g = new(code);
        //	do
        //	{
        //		if (scope == LogicalScope.OR && current)
        //			return true;
        //		if(scope == LogicalScope.NOT && current)
        //			return false;
        //		if(scope == LogicalScope.AND && !current)
        //			return false;
        //
        //		if (keywords.ContainsKey(g.Current))
        //		{
        //			g = keywords[g.Current].Invoke(g);
        //			continue;
        //		}
        //		else if (TryGetVariable(g.Current, out IValue? variable))
        //		{
        //			if (variable == null)
        //				throw new Exception();
        //
        //			g = Var(g, variable.ThisConstruct, false, "args");
        //
        //			LogicalScopeAssign(variables["args"].Fulfills(variable));
        //
        //			variables.Remove("args");
        //
        //			continue;
        //		}
        //		else
        //		{
        //			throw new Exception();
        //		}
        //	} while (g.MoveNext());
        //
        //	return current;
        //
        //	Walker var_exists(Walker i)
        //	{
        //		if (!i.MoveNext())
        //			throw new Exception();
        //		
        //		if(i.Current != "=")
        //			throw new Exception();
        //
        //		if (!i.MoveNext())
        //			throw new Exception();
        //
        //		if(TryGetVariable(i.Current, out IValue? variable))
        //		{
        //			LogicalScopeAssign(true);
        //		}
        //		else
        //		{
        //			LogicalScopeAssign(false);
        //		}
        //		return i;
        //	}
        //	Walker NewLogic(LogicalScope scope, Walker i)
        //	{
        //		if (!i.MoveNext())
        //			throw new Exception();
        //
        //		if (i.Current != "=")
        //			throw new Exception();
        //
        //		if(!i.MoveNext())
        //			throw new Exception();
        //
        //		i = GetScope(i, out Block newscope);
        //
        //		if(Parser.HasEnclosingBrackets(newscope))
        //			RemoveEnclosingBrackets(newscope);
        //
        //		LogicalScopeAssign(Logic(newscope, def, scope));
        //
        //		return i;
        //	}
        //	void LogicalScopeAssign(bool b)
        //	{
        //		switch (scope)
        //		{
        //			case LogicalScope.AND:
        //				{
        //					current = current && b;
        //				}
        //				break;
        //			case LogicalScope.OR: 
        //				{ 
        //					current = current || b;
        //				}
        //				break;
        //			case LogicalScope.NOT:
        //				{
        //					current = current == false && b == false;
        //				}
        //				break;
        //		}
        //	}
        //}
    }
}
